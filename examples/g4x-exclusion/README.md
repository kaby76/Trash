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

G4X set difference currently works on top-level lexer alternatives with
exclusions that are named lexer rules or string literals. Both sides must
match the **same complete candidate text**. Parser-rule exclusions, exclusions
inside lexer blocks, character-set/range exclusion operands, and
`caseInsensitive=true` are diagnosed as unsupported rather than ignored.

[jls]: https://docs.oracle.com/javase/specs/jls/se27/html/jls-19.html
