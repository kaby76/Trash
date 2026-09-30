# Tree-sitter `grammar.json` to W3C EBNF

This example parses a Tree-sitter-generated `src/grammar.json` with the
included ANTLR JSON grammar, then uses `trxquery` to turn its JSON parse tree
into W3C EBNF text. It does **not** convert Tree-sitter's JavaScript
`grammar.js` source. The JSON grammar is based on
[`grammars-v4/json/JSON.g4`](https://github.com/antlr/grammars-v4/blob/master/json/JSON.g4).

Build the Release tools first, then run the checked example:

```sh
bash examples/tree-sitter-to-w3c-ebnf/run-example.sh
```

To convert another file from any working directory:

```sh
bash /path/to/Trash/examples/tree-sitter-to-w3c-ebnf/convert-tree-sitter.sh \
  /path/to/tree-sitter-c/src/grammar.json > c.ebnf
```

For instance, use the pinned
[tree-sitter-c `grammar.json`](https://github.com/tree-sitter/tree-sitter-c/blob/b780e47fc780ddc8da13afa35a3f4ed5c157823d/src/grammar.json)
as input. The converter resolves its own XQuery and JSON grammar paths, so it
does not require the caller to change directory.

The output includes all entries in the JSON `rules` object, in source order.
Rule names are prefixed with `r_` so leading-underscore Tree-sitter names are
valid W3C EBNF symbols. `SEQ`, `CHOICE`, `REPEAT`, `REPEAT1`, `SYMBOL`, simple
`STRING`, and `BLANK` become EBNF syntax. `FIELD`, `ALIAS`, `TOKEN`,
`IMMEDIATE_TOKEN`, and precedence wrappers emit their `content`; their
metadata/lexical behavior is discarded.

This is a **structural, lossy translation**, not a Tree-sitter-compatible
parser generator. Tree-sitter regex `PATTERN` nodes become the deliberately
undefined `TS_PATTERN` symbol. Escaped JSON strings and unrecognized node
types become the deliberately undefined `TS_UNSUPPORTED` symbol, rather than
silently changing their meaning. External scanner tokens, conflicts,
precedence, extras, supertypes, word-token behavior, and lexical priorities
cannot be faithfully represented in plain W3C EBNF. Resolve placeholders and
review those features before treating output as a working grammar. The
[Tree-sitter grammar DSL](https://tree-sitter.github.io/tree-sitter/creating-parsers/2-the-grammar-dsl.html)
describes these constructs.
