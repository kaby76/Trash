# XQuery4-driven contextual lexing

This example distinguishes a hyphenated identifier from subtraction using
declarations already parsed. Before a declaration, `a-2` lexes as `a`, `-`,
`2`. After `var c-4;`, `c-4-5` lexes as `c-4`, `-`, `5`.

Run `bash run-example.sh` to see the significant token sequence, or
`bash test-example.sh` to compare it with `expected.tokens`. The pipeline is:

```sh
dotnet trash parse -t ANTLRv4 FooLexer.g4 FooParser.g4 \
  | dotnet trash interp -o interp
dotnet trash parse --allstar -L interp --xquery-hooks hooks.json \
  --tokens --no-output input.txt
```

No target-language parser is generated. The `xq("hyphen.xq")` predicate in
`FooLexer.g4` marks where the lexer asks whether a hyphen may belong to the
identifier. The current interpreter binds that predicate through `hooks.json`
by lexer rule name and predicate index; the marker alone does not load the
query. `hyphen.xq` consults per-parse declaration state. The manifest also
binds `on-decl-enter.xq` and `on-decl-exit.xq` to committed entry and exit of
the parser's `decl` rule. Those queries can inspect the partial `$tree` and
the complete `$input` character stream. The exit query returns the name to
add to the declaration state before lexing subsequent input.

This is an experimental vertical slice. The `ctx:declared-prefix` function
and `$candidate` convention are specific to this example; arbitrary state
updates, parser semantic predicates, and speculative-action rollback are not
yet implemented.
