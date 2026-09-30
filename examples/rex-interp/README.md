# Basic REx interpretation

Run `bash run-example.sh` with the rebuilt Trash tools installed. The script
generates `.interp`/`.tokens` files from two REx grammars, parses three inputs,
checks their ANTLR-style trees, and checks rejection of incomplete arithmetic.
It requires no generated target-language parser.

```sh
dotnet trash parse Arithmetic.rex | dotnet trash interp -o interp-arithmetic
dotnet trash parse --allstar -L interp-arithmetic -i '1+2*3' | dotnet trash tree -a
```

Expected output:

```text
(Start (Expr (Expr (Term 1)) + (Term 2 * 3)) <EOF>)
```

`List.rex` demonstrates grouping, optional lists, and repeated comma-separated
items. `Arithmetic.rex` demonstrates left recursion and lexical helper rules.
Rules before `<?TOKENS?>` are parser rules; rules after it are lexical rules.
Lexical rules referenced from syntax are tokens; remaining lexical helpers are
fragments. A lexical rule consisting solely of `$` names EOF. Its use in a
syntax rule enables the usual start-rule discovery; otherwise use `--start-rule`.
Output names come from the source filename: `Arithmetic.interp` and
`ArithmeticLexer.interp`.

This is a basic tokenized subset of [REx notation](https://github.com/GuntherRademacher/rex-parser-generator/blob/main/docs/ebnf-notation.md),
using Trash's ANTLR-style maximal-munch and token-priority behavior. Supported:
references, sequences, `|`, parentheses, `?`, `*`, `+`, either style of quoted
literal, positive character classes/ranges, and hexadecimal character codes.
For class escapes use `\u0041` or `\u0041-\u005A`; the existing built-in REx
grammar does not recognize `#x` escapes inside classes. Standalone `#x41` works.
Hexadecimal character codes and range endpoints above U+FFFF are rejected:
the interpreter currently consumes UTF-16 code units, not Unicode scalar values.

Whitespace in input must be specified explicitly in syntax. These examples
intentionally use inputs without whitespace. REx whitespace directives, ordered
choice, lookahead, exclusions, token preferences, context suffixes, character
equivalence, complemented classes, custom character universes, wildcard tokens,
nongreedy token declarations, and target-language annotations are unsupported.
Recognized unsupported constructs produce errors rather than approximations.
