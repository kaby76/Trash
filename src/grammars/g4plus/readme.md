# G4Plus Grammar

## Description
G4Plus is Trash's fork of the ANTLR4 grammar for syntax extensions that are
not part of ANTLR4. Initially it recognizes the same syntax as the copied
ANTLR4 grammar. Use `.g4p` or `.g4+` files with `trparse`, or select it with
`trparse -t G4Plus`.

G4Plus accepts a set-difference clause after an alternative. The `-` operator
excludes matches of one named rule or literal; use parentheses for a union of
exclusions. For example:

```antlr
Identifier : IdentifierChars - (ReservedKeyword | BooleanLiteral | NullLiteral);
TypeIdentifier : Identifier - ('permits' | 'record' | 'sealed' | 'var' | 'yield');
```

The clause applies to its preceding alternative; group alternatives on the
left if their union is to be excluded. This is currently grammar syntax only:
the generated G4Plus parser records the clause, but ANTLR4 and Trash's
interpreter do not yet enforce set-difference matching semantics.

## License
[BSD](https://opensource.org/license/bsd-3-clause)

## Reference
* [pldb](http://pldb.info/concepts/antlr)
* [Wikipedia](https://en.wikipedia.org/wiki/ANTLR)
