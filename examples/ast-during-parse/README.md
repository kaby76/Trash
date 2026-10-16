# Build a pipeable AST during an AllStar parse

Run `bash run-example.sh`. `trparse --allstar --ast-map arithmetic.ast`
uses the separate `.ast` mapping to reduce committed parser-rule exits into
`UnvParseTreeNode` elements. It does not first construct a CST. Its default
output remains a PAX/tar bundle containing a `.pt` parse result, so subsequent
Trash commands can consume the AST directly:

```sh
dotnet trash parse Arithmetic.g4 | dotnet trash interp -o interp
dotnet trash parse --allstar -L interp --ast-map arithmetic.ast -i '1+2*3' |
  dotnet trash tree --text -a
```

The output is:

```text
(Binary + (Number 1) (Binary * (Number 2) (Number 3)))
```

The example script also checks `(1+2)*3`, verifies that XQuery4 can select
three `Number` AST nodes from the emitted `.pt` bundle, and checks rejection
of `1+`. An AST constructor's `[capture]` creates a terminal `value` child;
for example, `//Binary/value/text()` selects an operator, and
`//Number/value/text()` selects a number's spelling.

The `--ast-map` path currently supports ordinary, pre-tokenized ALL(*)
parsing. Direct or indirect left-recursion events, context-aware lexing, and
runtime XQuery hooks need additional design; those combinations are rejected.
On a parse failure, any partial AST built during parsing is discarded. The
earlier [`ast-rewrite/`](../ast-rewrite/) example remains as a post-CST
comparison using the same mapping syntax.
