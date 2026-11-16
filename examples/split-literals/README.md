# Declaring parser literals before `trsplit`

ANTLR4 cannot create implicit tokens for string literals in a standalone
parser grammar. `declare-literals.xq` runs on the combined grammar's parse
tree before `trsplit`. It inserts one structural lexer rule for each distinct
parser literal, ahead of existing lexer rules so keywords beat a general `ID`
rule on equal-length matches. `trsplit` then copies those rules into the lexer
grammar. The parser's literal references stay as written and resolve through
`tokenVocab`; `trfoldlit` is not needed. Existing lexer rules that match a
single literal are reused, and the generated name prefix is adjusted if it
would collide with an existing token name.

Build the Trash tools in Release configuration, then run
`bash run-example.sh`. The example applies XQuery, splits `SplitLiterals.g4`,
writes the final grammar pair, and parses `input.txt`. Its transformation is a
single `parse | xquery | split | sponge` pipeline. A second small grammar
checks reuse of an existing literal rule and avoidance of a generated-name
collision.

For the Positive Technologies HyperTalk grammar, the equivalent workflow is:

```sh
trparse HyperTalk.g4 \
  | trxquery -q /absolute/path/to/declare-literals.xq \
  | trsplit \
  | trsponge -o rewritten
```

The query normally uses generated `TRSPLIT_LITERAL_N` token names.
