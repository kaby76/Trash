# VMF ANTLR4 grammar to CongoCC

This example converts the [VMF grammar](vmf.g4) to a CongoCC grammar with the
XQuery4 converter used by `trgen -t CongoCC`, then generates and compiles a
Java parser. Its lexer exercises ANTLR4 negated sets, escaped quotes, and
escaped character-set members. `simple.vmf` checks that the resulting parser
accepts a quoted value. The generated `vmf.ccc` keeps the original parser rule
names by default; set `CONGOCC_RULE_PREFIX=ParserRule_` for a prefixed build.

Run `bash run-example.sh`. CongoCC's JAR must be at
`$HOME/congo-parser-generator/bin/congocc.jar`, or set `CONGOCC_JAR`. Java and
`javac` must be on `PATH`.

This first-pass converter does not preserve ANTLR4 alternative prediction.
Some larger VMF inputs may require CongoCC `SCAN` annotations before they parse.
