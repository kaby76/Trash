# Interpret a W3C EBNF grammar

This example compiles [`Message.ebnf`](Message.ebnf) with `trinterp` and parses
[`input.txt`](input.txt) with AllStar. It exercises string literals, a `#x`
character, character sets and ranges, alternatives, and `+` repetition.

```sh
dotnet trash parse -t W3CEBNF Message.ebnf | dotnet trash interp |
  dotnet trash parse --allstar input.txt | dotnet trash tree --text -a
```

Build the Release tools, then run `bash run-example.sh` to test the pipeline.
The script invokes this checkout's binaries; the `dotnet trash` commands above
require an installed build containing the new front end. The first production is the
entry point; the generated `w3c_start` wrapper requires EOF. Every input
character is lexed as one disjoint token, preserving scannerless EBNF
matching. Whitespace is significant unless the grammar describes it.

This basic front end accepts strings, `#x` BMP characters, character sets,
references, grouping, alternatives, empty sequences, and `?`, `*`, `+`.
Production difference (`-`) and validity constraints (`[wfc: ...]` and
`[vc: ...]`) are explicitly rejected. Surrogates and code points beyond the
BMP are not supported. See [trinterp's documentation](../../src/trinterp/readme.md#basic-w3c-ebnf-interpretation)
for details.
