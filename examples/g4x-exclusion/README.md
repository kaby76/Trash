# G4X lexer set difference

`SetDiffLexer.g4x` puts `Identifier` before `Keyword` and `NullLiteral`:

```antlr
Identifier : [a-z]+ - (Keyword | 'null');
Keyword : 'if' | 'class';
NullLiteral : 'null';
```

Without the exclusion, ANTLR-style equal-length rule priority would classify
every word as `Identifier`. With set difference, `if` and `class` are `Keyword`,
`null` is `NullLiteral`, and other words remain `Identifier`. `input.txt` contains
all three cases. `SetDiffParser.g4x` checks the resulting token sequence.

Run `bash run-example.sh` to compile the grammars with `trinterp`, parse the
input with AllStar, and display its tree. `bash test-example.sh` uses the local
Release binaries and checks the exact token types without a network download.

`JlsIdentifiers.g4x` is a separate, illustrative subset of the
[JLS 27 identifier rules][jls]. It is not a complete Java grammar.

`ContextParser.g4x` demonstrates parser-rule exclusion. Its `typeIdentifier`
accepts an `Identifier` except when the token text is `permits` or `record`.
`ContextLexer.g4x` deliberately puts `PERMITS` before `Identifier`;
`ContextParser.g4x` declares `contextAwareLexing=true`, so `trparse` selects
ALL(*) with context-aware lexing without command-line flags. This can still
select `Identifier` for `permits`, but the
parser exclusion rejects it. `context-ok.txt` parses and
`context-excluded.txt` is rejected. `test-example.sh` checks both.

G4X set difference works on top-level lexer alternatives with exclusions
that are named lexer rules or string literals. Both sides must match the
**same complete candidate text**. In parser grammars, AllStar also supports
string-literal exclusions on alternatives proven to consume one token. For
example, `typeIdentifier : identifier - 'permits';` rejects a token whose
text is `permits`, including when `--context-aware-lexing` gives that token
the `Identifier` type. Parser alternatives that may consume multiple tokens,
named parser-rule exclusions, exclusions inside blocks, character-set/range
operands, and lexer exclusions with `caseInsensitive=true` are diagnosed as
unsupported.

[jls]: https://docs.oracle.com/javase/specs/jls/se27/html/jls-19.html
