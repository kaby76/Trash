(: Step 7: Convert Lark /regex/ literals to Antlr4 lexer rule syntax.
   Delegates to convert-regexp.py via exec(cmd, input) so the pattern is
   piped to the script's stdin — no shell quoting issues with backslashes,
   parens, or brackets.
   Patterns containing (? (lookaheads/lookbehinds) are left unchanged —
   they have no Antlr4 equivalent and must be rewritten by hand:
     /(?<!\\)(\\\\)*?/           (used in _STRING_ESC_INNER)
     /\/\*(\*(?!\/)|[^*])*\*\//  (block comment, rewrite as '/*' .*? '*/')
:)
(for $r in //REGEXP
 where not(contains(string($r), "(?"))
 return replace value of node $r with exec("python3 convert-regexp.py", string($r)))
