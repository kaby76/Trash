# Lark to Antlr4 Example

Demonstrates converting a [Lark](https://github.com/lark-parser/lark) grammar
to Antlr4 syntax using a multi-pass XQuery Update pipeline.

The example converts `verilog.lark` (with shared rules imported from
`common.lark`) into an Antlr4 grammar.

## Prerequisites

- `dotnet trash` installed (`dotnet tool install -g trash`)
- Python 3 (for regex conversion in pass 4)

## Run

```bash
bash run-example.sh
```

## How it works

1. `dotnet trash parse verilog.lark common.lark` parses both Lark grammar
   files and emits a combined JSON parse-tree (`o.pt`).

2. **Pass 1** (`lark-to-antlr4.xq`) — structural syntax transforms:
   - Appends ` ;` after each parser rule and lexer token definition.
   - Strips the Lark inline/transparent marker `?` from rule names.
   - Converts string literals from double-quotes to single-quotes (escaping
     any embedded `'` as `\'`).

3. **Pass 2** (`import-common.xq`) — resolves imports:
   - Inlines the required `common.lark` token rules (already transformed in
     pass 1) into the `verilog.lark` document using `doc("transformed.pt")`
     for cross-document lookup.
   - Deletes all `%import` statements.

4. **Pass 3** (`ignore-to-skip.xq`) — maps `%ignore` to Antlr4 skip:
   - For each `%ignore TERMINAL`, appends ` -> skip` to the corresponding
     lexer token rule and removes the `%ignore` item.

5. **Pass 4** (`regexp-to-antlr4.xq`) — converts regex literals:
   - Rewrites Lark `/regex/` literals to Antlr4 character-class syntax by
     piping each pattern to `convert-regexp.py` via `exec(cmd, input)`.
   - Patterns using lookaheads/lookbehinds (`(?`) are left unchanged — they
     have no Antlr4 equivalent and must be rewritten by hand.
   - Final output is written to `xxx/` via `dotnet trash sponge`.
