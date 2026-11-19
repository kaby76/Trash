# CongoCC target

`trgen -t CongoCC` generates a Java parser application. `build.sh` uses the
Trash pipeline to convert a combined ANTLR4 grammar with `convert.xq`, runs
CongoCC, and compiles the generated Java code.

Set `CONGOCC_JAR` to the CongoCC generator JAR. Its default is
`$HOME/congo-parser-generator/bin/congocc.jar`. Java and `javac` must be on
`PATH` (or set `JAVA` and `JAVAC`).

This is an initial converter for named lexer tokens, character sets, string
literals, `-> skip`, lexer fragments and ranges, grouped alternatives, basic repetition, and simple direct left recursion.
ANTLR4 alternative order and CongoCC `SCAN` decisions are not translated yet. The left-recursion rewrite does not preserve ANTLR4 precedence or associativity. Parser rules are prefixed with `ParserRule_` to avoid case-insensitive AST class collisions with lexer tokens.
See `examples/words-congocc` for a runnable example.
