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
It requires one top-level parser grammar and one top-level lexer grammar, plus
all grammars they import. Input order does not matter. `combine` follows the
import graph, inlines imported rules with the importing grammar's rules taking
precedence, and removes the `import` declarations. The parser's `tokenVocab`,
when present, must name the top-level lexer grammar.
The output is a PAX/tar bundle containing one `<name>.g4.pt` ANTLRv4 parse
tree. It can be modified by `dotnet trash xquery` before extracting source.
`dotnet trash sponge -o DIR` turns it into one `<name>.g4` file. Source `.g4`
files on disk are not changed. Passing `.g4` files directly to `combine` is
an error; parse them first and pipe the bundle to `combine`.

The command changes `parser grammar` to `grammar`, removes the `Parser` suffix
from the grammar name, removes `tokenVocab`, merges compatible options from the
two top-level grammars, retains other prequels, and appends the lexer rules
after the parser rules. As in ANTLR4 import processing, options in imported
grammars are ignored. It preserves the parser grammar's leading comments.
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
preserves lexer modes in the editable `.pt` result. ANTLR4 code generation
rejects modes in combined grammars, so a grammar with modes (such as Kotlin
Formal) needs another transformation before code generation. Conflicting
parser and lexer options are rejected.

Run the regression cases with `bash tests/trcombine/test.sh` from the repository
root after building `trparse`, `trcombine`, and `trtext` in Release configuration.

## Current version

Release 5.1.0.

## License

MIT. Copyright (c) 2026 Ken Domino.
