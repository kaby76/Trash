# Java 27 G4X interpreter

This example copies `Java27Lexer.g4x`, `Java27Parser.g4x`, and the sample
inputs from `~/issues/g4-java-27/java/java27`. It compiles the grammars to
`.interp` tables and parses Java source with the interpreted ALL(*) parser;
it does not generate a target-language parser.

The parser grammar declares `contextAwareLexing=true` in its `options` block.
`trinterp` records the setting in `Java27Parser.interp`, so `trparse` selects
context-aware ALL(*) automatically. The grammar also has indirect left
recursion, which still requires `--indirect-left-recursion` at parse time.

Run `bash run-example.sh` to display the `helloworld.java` tree, parse
three larger examples, and parse `Escapes.java` with the source hook.
`hooks.json` binds `java-unicode.xq` to a pre-lex XQuery4 source hook.
Its `ctx:java-unicode-edits($input)` function implements JLS Unicode-escape
translation and retains an offset map to the original input. Source-only
hooks work with `--indirect-left-recursion`; stateful parser/lexer hooks
still do not. Run `bash test-example.sh` after building Trash in Release
mode to verify table generation and parsing, including the long
`ManyStringsConcat.java` expression. Generated tables go in `interp/`.

`ManyStringsConcat.java` contains 900 string operands and exercises the
interpreter's indirect-left-recursion seed-growth path.
