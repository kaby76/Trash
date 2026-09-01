# Context-aware lexing with overlapping Decaf tokens

This example demonstrates `trparse --context-aware-lexing` with the lexer-rule
overlap described in [Parse Rules Decaf grammar antlr4][question].

Both rules below match the input `10`:

```antlr
DECIMAL_LITERAL: DIGIT+;
INT_LITERAL: DECIMAL_LITERAL;
```

Ordinary ANTLR lexer semantics select `DECIMAL_LITERAL`, because it precedes
`INT_LITERAL` and both matches have equal length. The parser rule, however,
requires `INT_LITERAL` inside an array declaration:

```antlr
field_decl: INT ID LSQUARE INT_LITERAL RSQUARE SEMI;
```

Consequently, ordinary ALL(*) interp parsing rejects `int i[10];`.
Context-aware lexing supplies the parser's valid-lookahead set to the lazy
lexer. At that position it prefers `INT_LITERAL`, and the input parses.

## Running

Build/install the current Trash tools and run:

```sh
bash run-example.sh
```

The script generates `interp/`, verifies that ordinary ALL(*) parsing fails,
then parses successfully with:

```sh
dotnet trash parse --context-aware-lexing -L interp input.decaf
```

The runner also specifies `--lexer-overlaps`. Its stderr report shows that
`DECIMAL_LITERAL` would win under ordinary rule priority, while parser context
selects `INT_LITERAL`, followed by aggregate overlap statistics.

It writes the successful parsing result to `result.pt` and its indented tree to
`result.tree`.

Run the golden-output test with:

```sh
bash test-example.sh
```

[question]: https://stackoverflow.com/q/61644244/4779853
