# Expanding `caseInsensitive` in an ANTLR4 lexer

`fold-ascii-case.xq` operates on a parsed ANTLR4 lexer grammar. It removes the
global `caseInsensitive` option and explicitly folds ASCII letters in lexer
string literals, character ranges, and character sets. For example, it changes
`'Select'` to `[Ss][eE][lL][eE][cC][tT]`, `'G'..'Z'` to `[G-Zg-z]`, and
`[A-F0-9]` to `[A-F0-9a-f]`. It expands negated sets too, so `~[A-Z]`
becomes `~[A-Za-z]`. Parser-rule literals are left alone.

Build Trash in Release configuration, then run `bash run-example.sh`. The
script parses `FoldLexer.g4`, applies the XQuery, extracts the updated lexer,
builds interpreter tables with `FoldParser.g4`, and parses `input.txt`.

The same query works on the Positive Technologies `MySqlLexer.g4`. It handles
plain ASCII letters and same-case ASCII ranges; escaped code points, Unicode
properties, and non-ASCII case mappings are preserved without expansion.
For a grammar relying on those forms for case-insensitive matching, additional
Unicode-aware rewriting would be necessary before removing the option.
