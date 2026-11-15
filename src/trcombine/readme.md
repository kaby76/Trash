# trcombine

## Summary

Combine a split ANTLR4 parser and lexer grammar into one grammar.

## Usage

```sh
dotnet trash parse tests/trcombine/Gold/ArithmeticParser.g4 tests/trcombine/Gold/ArithmeticLexer.g4 \
  | dotnet trash combine \
  | dotnet trash text --text
```

`combine` reads a PAX/tar parse-result bundle from stdin, or from `-f FILE`.
It requires exactly one parser grammar and one lexer grammar, in either order.
The parser's `tokenVocab`, when present, must name the supplied lexer grammar.
The output is a PAX/tar bundle containing one `<name>.g4.pt` ANTLRv4 parse
tree. It can be modified by `dotnet trash xquery` before extracting source.
`dotnet trash sponge -o DIR` turns it into one `<name>.g4` file. Source `.g4`
files on disk are not changed. Passing `.g4` files directly to `combine` is
an error; parse them first and pipe the bundle to `combine`.

The command changes `parser grammar` to `grammar`, removes the `Parser` suffix
from the grammar name, removes `tokenVocab`, merges compatible parser and lexer
options, retains compatible lexer prequels, and appends the lexer rules
after the parser rules. It preserves the parser grammar's leading comments.
It removes the input parser and lexer `.pt`, `.errors`, and `.g4` members from
the output bundle; unrelated artifacts pass through.

To extract the combined source into a directory:

```sh
dotnet trash parse tests/trcombine/Gold/ArithmeticParser.g4 tests/trcombine/Gold/ArithmeticLexer.g4 \
  | dotnet trash combine \
  | dotnet trash text \
  | dotnet trash sponge -c -o combined
```

ANTLR4 code generation rejects custom `channels { ... }` blocks in combined
grammars. `combine` retains the block and its lexer commands in the `.pt`
tree so they can be rewritten. See
`examples/combine-custom-channels/channel-to-hidden.xq` for a transformation
that maps custom channels to `HIDDEN` and removes the declaration. The command
rejects lexer modes and conflicting parser and lexer options.

Run the regression cases with `bash tests/trcombine/test.sh` from the repository
root after building `trparse`, `trcombine`, and `trtext` in Release configuration.

## Current version

Release 4.12.0.

## License

MIT. Copyright (c) 2026 Ken Domino.
