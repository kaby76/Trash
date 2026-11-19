# Arithmetic ANTLR4 to CongoCC

This example copies the `arithmetic.g4` grammar from grammars-v4 and runs
`trgen -t CongoCC`. The generated build converts the grammar with XQuery4,
invokes CongoCC, and compiles a Java parser. Run `bash run-example.sh`.

The grammar exercises lexer fragments, character ranges, grouped expressions,
direct left recursion, and a case-insensitive AST class collision between the
`VARIABLE` token and `variable` parser rule. The converter prefixes parser
rules with `ParserRule_` and rewrites direct left recursion to a base
alternative followed by repeated tails. This first-pass rewrite does **not**
preserve ANTLR4's operator precedence or associativity. It is for build and
basic syntax validation, not equivalent parse trees.

The source grammar retains its BSD license header. Set `CONGOCC_JAR` if the
CongoCC JAR is not at `$HOME/congo-parser-generator/bin/congocc.jar`.
