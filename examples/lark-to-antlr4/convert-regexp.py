#!/usr/bin/env python3
"""Convert a Lark /regex/ literal to Antlr4 lexer rule syntax.

Usage:
  python3 convert-regexp.py '/pattern/'   # argument
  echo '/pattern/' | python3 convert-regexp.py  # stdin

Handles:
  - strips / delimiters
  - character classes [abc], [a-z], [^\n]  →  [abc], [a-z], ~[\n]
  - \\s \\d \\w categories inside classes
  - string literals merged from adjacent chars  →  'abc'
  - . * + ? *? +? quantifiers
  - alternation and groups  (. | '\\n')
  - anchors ^ $ are dropped (Antlr4 lexer rules are implicitly anchored)

Patterns containing (? (lookahead/lookbehind) are NOT converted here —
filter those out in XQuery with: where not(contains(string($r), "(?"))
"""

import sys
try:
    from re import _parser as sre_parse, _constants as sc   # Python 3.11+
except ImportError:
    import sre_parse                                          # Python < 3.11
    import sre_constants as sc

MAXREPEAT = sc.MAXREPEAT


def _antlr4_char(code: int) -> str:
    """Render a Unicode code point for use inside an Antlr4 string literal or char class."""
    return {
        0x09: r'\t',
        0x0A: r'\n',
        0x0C: r'\f',
        0x0D: r'\r',
        0x5C: r'\\',
        0x27: r"\'",
    }.get(code, chr(code))


def _emit_char_class(items) -> str:
    """Emit an Antlr4 character class from a parsed sre IN node's item list."""
    negated = bool(items) and items[0][0] == sc.NEGATE
    body = items[1:] if negated else items

    CAT_MAP = {
        sc.CATEGORY_SPACE:     r'\s',
        sc.CATEGORY_NOT_SPACE: r'\S',
        sc.CATEGORY_DIGIT:     r'\d',
        sc.CATEGORY_NOT_DIGIT: r'\D',
        sc.CATEGORY_WORD:      r'\w',
        sc.CATEGORY_NOT_WORD:  r'\W',
    }

    parts = []
    for op, val in body:
        if op == sc.LITERAL:
            parts.append(_antlr4_char(val))
        elif op == sc.RANGE:
            lo, hi = val
            parts.append(f'{_antlr4_char(lo)}-{_antlr4_char(hi)}')
        elif op == sc.CATEGORY:
            parts.append(CAT_MAP.get(val, f'?cat{val}'))
        else:
            parts.append(f'?cc{op}')

    prefix = '~[' if negated else '['
    return prefix + ''.join(parts) + ']'


def _quant_suffix(min_: int, max_: int, greedy: bool) -> str:
    if   min_ == 0 and max_ == MAXREPEAT: s = '*'
    elif min_ == 1 and max_ == MAXREPEAT: s = '+'
    elif min_ == 0 and max_ == 1:         s = '?'
    elif min_ == max_:                    s = f'{{{min_}}}'
    else:                                 s = f'{{{min_},{max_}}}'
    return s if greedy else s + '?'


def _is_simple(body_str: str) -> bool:
    """True if body_str is a single Antlr4 atom — no outer grouping needed for a quantifier."""
    return (body_str == '.'
            or body_str.startswith('[')
            or body_str.startswith('~[')
            or (body_str.startswith('(') and body_str.endswith(')'))
            or (body_str.startswith("'") and body_str.endswith("'")))


def _emit(parsed) -> str:
    """Recursively emit Antlr4 tokens for a parsed sre sequence."""
    tokens: list[str] = []
    lit_buf: list[int] = []

    def flush_lits():
        if lit_buf:
            s = ''.join(_antlr4_char(c) for c in lit_buf)
            tokens.append(f"'{s}'")
            lit_buf.clear()

    for op, av in parsed:
        if op == sc.LITERAL:
            lit_buf.append(av)
            continue
        flush_lits()

        if op == sc.NOT_LITERAL:
            # sre optimises [^x] (single negated literal) to NOT_LITERAL
            tokens.append(f'~[{_antlr4_char(av)}]')

        elif op == sc.IN:
            tokens.append(_emit_char_class(av))

        elif op == sc.ANY:
            tokens.append('.')

        elif op in (sc.MAX_REPEAT, sc.MIN_REPEAT):
            min_, max_, body = av
            greedy = (op == sc.MAX_REPEAT)
            body_str = _emit(body)
            suffix = _quant_suffix(min_, max_, greedy)
            if _is_simple(body_str):
                tokens.append(f'{body_str}{suffix}')
            else:
                tokens.append(f'({body_str}){suffix}')

        elif op == sc.SUBPATTERN:
            # Python 3.7+: (group_id, add_flags, del_flags, body)
            body = av[3] if len(av) >= 4 else av[1]
            inner = _emit(body)
            # BRANCH already emits with parens; avoid double-wrapping
            if inner.startswith('(') and inner.endswith(')'):
                tokens.append(inner)
            else:
                tokens.append('(' + inner + ')')

        elif op == sc.BRANCH:
            _, branches = av
            alts = ' | '.join(_emit(b) for b in branches)
            tokens.append(f'({alts})')

        elif op == sc.AT:
            pass  # drop anchors ^ and $

        else:
            tokens.append(f'?op{op}')

    flush_lits()
    return ' '.join(tokens)


def convert(pattern: str) -> str:
    pattern = pattern.strip()
    if not (pattern.startswith('/') and pattern.endswith('/')):
        raise ValueError(f"expected /regex/ pattern, got: {pattern!r}")
    inner = pattern[1:-1]
    return _emit(sre_parse.parse(inner))


if __name__ == '__main__':
    if len(sys.argv) == 2:
        src = sys.argv[1]
    else:
        src = sys.stdin.read()
    print(convert(src))
