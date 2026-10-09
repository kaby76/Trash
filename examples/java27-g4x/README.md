# Java 27 G4X interpreter

This example copies `Java27Lexer.g4x`, `Java27Parser.g4x`, and the sample
inputs from `~/issues/g4-java-27/java/java27`. It compiles the grammars to
`.interp` tables and parses Java source with the interpreted ALL(*) parser;
it does not generate a target-language parser.

The parser grammar declares `contextAwareLexing=true` in its `options` block.
`trinterp` records the setting in `Java27Parser.interp`, so `trparse` selects
context-aware ALL(*) automatically. The grammar also has indirect left
recursion, which still requires `--indirect-left-recursion` at parse time.

Run `bash run-example.sh` to display the `helloworld.java` tree and parse
three larger examples. Run `bash test-example.sh` after building Trash in
Release mode to verify table generation, the grammar option, six successful
inputs, and continuation after one rejected input. Generated tables go in
`interp/`.

The copied corpus includes files not used in the smoke test. In particular,
`AllInOne8.java` still rejects, `ManyStringsConcat.java` is a performance
stress case, and `Escapes.java` needs Java Unicode-escape translation before
lexing. They remain available here for further interpreter work.
