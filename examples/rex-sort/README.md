# Sort REx rules with XQuery4

[`sort-rex.xq`](sort-rex.xq) sorts syntax productions and lexical productions
independently by rule name, case-insensitively. It moves the existing parse-tree
nodes so the rule bodies and comments attached to them are preserved. It keeps
the prolog, `<?TOKENS?>` boundary, `<?ENCORE?>` section, and non-rule lexical
directives. It may add blank lines between moved rules.

For a grammar file:

```sh
dotnet trash parse Grammar.rex \
  | dotnet trash xquery -q /absolute/path/to/sort-rex.xq \
  | dotnet trash text --text > sorted.rex
```

Use `dotnet trash text | tar -xvf - -C DIR` instead if you
want the original filename and diagnostics written to a directory. Do not write
over the input grammar before checking the result.

`bash run-example.sh` sorts `../rex-interp/Arithmetic.rex`, verifies the expected
rule order, and reparses the output. Sorting may change REx's first-declared
start rule or lexer tie-breaking priority; review those effects before using
the sorted grammar to generate a parser.
