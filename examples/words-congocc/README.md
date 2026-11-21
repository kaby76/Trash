# ANTLR4 Words to CongoCC

This example runs `trgen -t CongoCC` on [Words.g4](Words.g4). The generated
`build.sh` converts the parsed ANTLR4 grammar with `convert.xq` using
`dotnet trash parse | dotnet trash xquery`, runs CongoCC on `Words.ccc`, and
compiles its Java parser and the generated driver.

The alternatives of `wordGroup` deliberately share the prefix `WORD COMMA
WORD`. The C# choice analyzer detects the overlap and emits a CongoCC `SCAN`
that checks the following `EOF` token before committing to the first
alternative. Both the two-word and three-word inputs parse.

Set `CONGOCC_JAR` if CongoCC is not at
`$HOME/congo-parser-generator/bin/congocc.jar`. Java 21 and `javac` must be
on `PATH` (or set `JAVA` and `JAVAC`). Run `bash run-example.sh`.
