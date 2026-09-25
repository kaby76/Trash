# G4Plus set-difference syntax

`JlsIdentifiers.g4p` demonstrates G4Plus's `-` operator with two rules based
on the [JLS 27 identifier syntax][jls]. The left expression supplies candidate
matches; the parenthesized right expression lists matches to exclude:

```antlr
Identifier : IdentifierChars - (ReservedKeyword | BooleanLiteral | NullLiteral);
TypeIdentifier : Identifier - ('permits' | 'record' | 'sealed' | 'var' | 'yield');
```

The character and keyword definitions in this example are deliberately small,
not a complete Java lexer. Currently, G4Plus **parses and records** the
exclusion clauses but does not yet enforce them during lexing or generate an
ANTLR parser from them.

Run from any directory with `bash /path/to/examples/g4plus-exclusion/run-example.sh`.
The script parses the `.g4p` file and prints its exclusion subtrees. It does
not try to parse Java source code.

[jls]: https://docs.oracle.com/javase/specs/jls/se27/html/jls-19.html
