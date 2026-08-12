# Java Antlr Target Example

Demonstrates generating a Java-target Antlr4 parser from the
[Java grammar](https://github.com/antlr/grammars-v4/tree/master/java/java),
building it, and running it over example `.java` files.

Grammar and example files sourced from
[antlr/grammars-v4](https://github.com/antlr/grammars-v4).

## Prerequisites

- `dotnet trash` installed (`dotnet tool install -g trash`)
- Java JDK (for compiling and running the generated parser)

## Run

```bash
bash run.sh
```

## How it works

1. `dotnet trash gen -t Java` generates a complete Java project (lexer,
   parser, and test driver) in `Generated-Java/`.
2. `make` (inside `Generated-Java/`) downloads the Antlr4 runtime jar and
   compiles the generated sources.
3. `bash run.sh -input <file>` parses each example `.java` file and prints
   the parse tree to stdout.
