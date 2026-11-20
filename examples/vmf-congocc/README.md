# VMF ANTLR4 grammar to CongoCC

This example converts the [VMF grammar](vmf.g4) to a CongoCC grammar with the
XQuery4 converter used by `trgen -t CongoCC`, then generates and compiles a
Java parser. Its lexer exercises ANTLR4 negated sets, escaped quotes, and
escaped character-set members. `simple.vmf` checks that the resulting parser
accepts a quoted value. `example1.txt` checks a full VMF sample, including
adjacent nested sections whose names could otherwise be consumed by
`atomicvalue+`. The C# choice analyzer adds a `SCAN` guard to that repetition using the
containing rule's following tokens. The generated `vmf.ccc` keeps the original parser rule
names by default; set `CONGOCC_RULE_PREFIX=ParserRule_` for a prefixed build.

Run `bash run-example.sh`. CongoCC's JAR must be at
`$HOME/congo-parser-generator/bin/congocc.jar`, or set `CONGOCC_JAR`. Java and
`javac` must be on `PATH`.

The converter handles this repeated-rule boundary, but does not implement
ANTLR4's general adaptive alternative prediction.
