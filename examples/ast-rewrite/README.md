# External AST rewrite prototype

This is an executable proof of concept for specifying CST-to-AST rewrites in a
separate `.ast` file, without embedding target-language actions or `-> ^(...)`
syntax in the grammar. Run `bash run-example.sh` after installing the current
Trash tools. It builds the small prototype rewriter, compiles `Arithmetic.g4`
with `trinterp`, parses two expressions with AllStar, and compares the emitted
JSON ASTs against checked-in expected results.

The mapping is deliberately close to ANTLR 3 rewrite notation:

```text
ast Arithmetic {
    default single-child -> child;
    default otherwise -> error;

    start(root:expr, EOF) -> root;
    expr(left:term, op:'+', right:expr) -> ^(Binary[op] left right);
    term(left:atom, op:'*', right:term) -> ^(Binary[op] left right);
    atom(value:NUMBER) -> ^(Number[value]);
    atom('(', inner:expr, ')') -> inner;
}
```

The left side matches the **direct element children of a CST rule node**, in
order. `name:ruleOrToken` binds the transformed child AST to `name`; quoted
strings match exact terminal text and can be captured as `name:'text'`.
The right side either returns a capture or
constructs an AST node with `^(Kind children...)`. `[capture]` copies captured
token text into the new node. Rewrites are bottom-up, and the first matching
rule wins. The defaults unwrap unmatched one-child rules and reject any other
unmatched rule shape. That rejection is intentional: a grammar change cannot
silently produce a misleading AST.

The prototype reads Trash's `.pt` stream or PAX/tar parse-result bundle and
prints JSON. It is an example-local .NET application rather than a new Trash
command, and the supported pattern language is intentionally small: exact
child sequences, names, captures, and terminal literals. There is not yet an
alternative selector, repetition/fold operator, list capture, XQuery4 escape
hatch, source-span retention, or incremental on-exit construction. Those would
need design and tests before making this a production pipeline stage.
