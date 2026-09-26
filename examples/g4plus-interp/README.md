# Interpret G4Plus grammars

Run `bash run-example.sh` with the rebuilt Trash tools available through
`dotnet trash`. The script compiles the two `.g4p` grammars to `.interp` tables,
parses `input.txt` using AllStar, and checks the ANTLR-style output tree.

`ArithmeticLexer.g4p` uses lowercase lexer rules and a lowercase fragment.
`ArithmeticParser.g4p` uses uppercase parser rules and direct left recursion.
The grammar declaration determines rule kind, and `tokenVocab` binds the parser
to its lexer. No target-language parser generation or compilation is required.

The expected tree is:

```text
(Start (Expression (Expression (Expression 1) + 2) + 3) <EOF>)
```

This example leaves `interp/` and `result.tree` for inspection. The test fails
if generation, parsing, or the tree comparison fails. G4Plus exclusions and
scannerless parser character sets are not yet supported by table generation.
