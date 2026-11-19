# ANTLR4 Words to CongoCC

This example runs `trgen -t CongoCC` on [Words.g4](Words.g4). The generated
`build.sh` converts the parsed ANTLR4 grammar with `convert.xq` using
`dotnet trash parse | dotnet trash xquery`, runs CongoCC on `Words.ccc`, and
compiles its Java parser and the generated driver.

The alternatives of `wordGroup` deliberately put the three-word form before
the two-word form. This first pass aims to produce a syntactically valid
CongoCC grammar and run the three-word input. It does not translate ANTLR4
alternative prediction into CongoCC `SCAN`; the two-word input is reserved
for the next step.

Set `CONGOCC_JAR` if CongoCC is not at
`$HOME/congo-parser-generator/bin/congocc.jar`. Java 21 and `javac` must be
on `PATH` (or set `JAVA` and `JAVAC`). Run `bash run-example.sh`.
