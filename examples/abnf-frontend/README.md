# Compile basic ABNF directly to interpreter tables

This example parses [`Message.abnf`](Message.abnf) with Trash's built-in ABNF
parser, lowers the ABNF rules to `.interp` tables, and uses the AllStar
interpreter to parse [`input.txt`](input.txt). No generated target-language
parser is needed.

```sh
dotnet trash parse -t ABNF Message.abnf | dotnet trash interp |
  dotnet trash parse --allstar input.txt | dotnet trash tree --text -a
```

Run `bash run-example.sh` to check the pipeline. The grammar demonstrates
case-insensitive quoted strings and rule names, incremental `=/` alternatives,
and built-in `SP`/`LF` core rules. The synthesized `abnf_start` rule enforces
end-of-input while leaving the first declared rule reusable.

This is a basic [RFC 5234](https://www.rfc-editor.org/rfc/rfc5234) ABNF
frontend. See [trinterp's documentation](../../src/trinterp/readme.md#basic-abnf-interpretation)
for supported forms and limitations. Unlike [`abnf-interp/`](../abnf-interp/),
which interprets the *ANTLR grammar for ABNF*, this example interprets a
grammar *written in ABNF*.
