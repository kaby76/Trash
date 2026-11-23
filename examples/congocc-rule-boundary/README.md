# Nested CongoCC rule boundary

`definition` starts with `ID '='`, while `contents` repeats `item`, which can
also start with `ID`. Without a boundary check, CongoCC consumes the first
identifier of the next definition as an `item` and then fails at `=`.

`trgen -t CongoCC` emits a syntactic `SCAN` with an `ENSURE` that excludes the
next definition's two-token header while still checking that `item` matches.
This preserves the ANTLR4 parse of [input.txt](input.txt). The example also
checks that `channel(HIDDEN)` becomes CongoCC `SKIP`.

Run `bash run-example.sh`. Set `CONGOCC_JAR` if CongoCC is not at
`$HOME/congo-parser-generator/bin/congocc.jar`.
