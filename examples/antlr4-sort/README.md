# Sort ANTLR4 parser rules with XQuery4

[`sort-antlr4.xq`](sort-antlr4.xq) sorts all parser rules alphabetically,
case-insensitively, including EOF-terminated start rules. Whole `ruleSpec`
parse-tree nodes are moved, so their bodies and attached comments are retained.
The grammar declaration and
prequel constructs, such as `options`, are untouched. This example is intended
for ANTLR4 `parser grammar` files, not combined or lexer grammars.

```sh
dotnet trash parse Grammar.g4 \
  | dotnet trash xquery -q /absolute/path/to/sort-antlr4.xq \
  | dotnet trash text --text > sorted.g4
```

Use `dotnet trash text | tar -xvf - -C DIR` to write the
result under its original filename. Avoid overwriting the input before checking
the result.

Run `bash run-example.sh` to sort [`SortDemo.g4`](SortDemo.g4), check the rule
order, EOF-terminated start rule, and preserved prequel/comment, then reparse
the result.
