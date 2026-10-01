# Interpret G4X grammars

Run `bash run-example.sh` with the rebuilt Trash tools available through
`dotnet trash`. The script compiles the two `.g4x` grammars to `.interp` tables,
parses `input.txt` using AllStar, and checks the ANTLR-style output tree.

`ArithmeticLexer.g4x` uses lowercase lexer rules and a lowercase fragment.
`ArithmeticParser.g4x` uses uppercase parser rules and direct left recursion.
The grammar declaration determines rule kind, and `tokenVocab` binds the parser
to its lexer. No target-language parser generation or compilation is required.

The expected tree is:

```text
(Start (Expression (Expression (Expression 1) + 2) + 3) <EOF>)
```

The example streams tables and the tree between commands without writing them
to this directory. The test fails if generation, parsing, or the tree
comparison fails. G4X exclusions and
scannerless parser character sets are not yet supported by table generation.
