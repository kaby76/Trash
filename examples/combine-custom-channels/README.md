# Combining grammars with custom channels

`trcombine` emits a PAX/tar bundle containing a structurally parsed
`Channel.g4.pt`. The XQuery4 update in `channel-to-hidden.xq` changes each
`channel(NAME)` lexer command to `channel(HIDDEN)` and deletes the custom
`channels { ... }` declaration. Build the Trash tools in Release configuration,
then run `bash run-example.sh` to print the resulting combined grammar.

This converts distinct custom channels into the single built-in `HIDDEN`
channel. ANTLR4 accepts `channel(HIDDEN)` in a combined grammar, whereas its
code generator rejects a custom `channels` declaration there. The XQuery
operates on the `.pt` parse tree in the bundle, so it can be extended with
other grammar changes before extracting source with `dotnet trash sponge`.
