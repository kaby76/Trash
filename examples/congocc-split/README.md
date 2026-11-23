# Split ANTLR4 grammar to CongoCC

This example generates a CongoCC parser from a separate parser and lexer
grammar. The generated build pipes their Trash parse result through
`dotnet trash combine` before the XQuery4 converter. The lexer declares
`caseInsensitive = true`, which the converted `.ccc` retains as
`IGNORE_CASE=true`.

The parser has two decisions that require more than a one-token choice:
the repeated `value COMMA` before a final `value SEMI`, and the optional sign
shared by the `FLOAT` and `INT` alternatives. The C# choice analysis emits
syntactic `SCAN` guards for these bounded patterns. `input.txt` exercises both.
It also places two `/* ... */` comments around a value. The converter uses a
CongoCC lazy `SKIP` token for ANTLR4's `.*?`, so the first comment does not
consume through the second one.

Run `bash run-example.sh`. Set `CONGOCC_JAR` if CongoCC is not at
`~/congo-parser-generator/bin/congocc.jar`.

To print the parse tree in ANTLR `ToStringTree()` form, run:

```sh
cd Generated-CongoCC
bash run.sh -tree --no-output --input '+1, -2.5;'
```

The output is `(document (value + 1) , (value - 2.5) ; <EOF>)`.
