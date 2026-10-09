# G4X Grammar

## Description
G4X is Trash's fork of the ANTLR4 grammar for syntax extensions that are
not part of ANTLR4. Initially it recognizes the same syntax as the copied
ANTLR4 grammar. Use `.g4x` or `.g4x` files with `trparse`, or select it with
`trparse -t G4X`.

Rule syntax is the same in `lexer grammar`, `parser grammar`, and combined
`grammar` files. A rule's name and references may start with either uppercase
or lowercase letters. The grammar declaration determines rule kind: all rules
in a `lexer grammar` are lexer rules; all rules in a `parser grammar` or a
combined `grammar` are parser rules. This includes rules inside `mode` blocks.
`fragment` is accepted syntactically in every grammar; semantic restrictions
will be checked separately. Bracketed content is parsed by position as either
a rule argument or a character-set atom, not by capitalization.

G4X accepts a set-difference clause after an alternative. The `-` operator
excludes matches of one named rule or literal; use parentheses for a union of
exclusions. For example:

```antlr
Identifier : IdentifierChars - (ReservedKeyword | BooleanLiteral | NullLiteral);
TypeIdentifier : Identifier - ('permits' | 'record' | 'sealed' | 'var' | 'yield');
```

The clause applies to its preceding alternative; group alternatives on the
left if their union is to be excluded. `trinterp` compiles top-level lexer
alternatives with named-rule and string-literal exclusions. The interpreter
rejects a candidate only when an excluded operand matches that candidate's
complete text. See [`examples/g4x-exclusion`](../../../examples/g4x-exclusion/)
for a runnable grammar and input.
AllStar also evaluates string-literal exclusions on parser alternatives that
consume exactly one token. It compares the token text, not just the token type,
so `--context-aware-lexing` cannot admit an excluded word by selecting a
contextual `Identifier` token.

For a G4X parser grammar that always needs contextual token selection, put
`contextAwareLexing=true` in its grammar-level options block:

```antlr
parser grammar MyParser;
options { tokenVocab=MyLexer; contextAwareLexing=true; }
```

`trinterp` records this setting in the parser `.interp`. When `trparse` loads
that table, it selects ALL(*) with context-aware lexing without needing
`--allstar` or `--context-aware-lexing` on the command line. The command-line
flag still enables context-aware lexing for grammars without this option.

## Table generation

`dotnet trash parse Lexer.g4x Parser.g4x | dotnet trash interp -o interp`
compiles supported G4X syntax into lexer/parser tables. See
[`examples/g4x-interp`](../../../examples/g4x-interp/README.md) for a runnable
example. Multi-token parser alternatives, named parser-rule exclusions,
exclusions inside blocks, character-set/range exclusion operands, and
case-insensitive lexer set difference are not yet supported; they produce
compilation errors. Imports and
scannerless parser character sets also produce errors rather than being
silently discarded.

## License
[BSD](https://opensource.org/license/bsd-3-clause)

## Reference
* [pldb](http://pldb.info/concepts/antlr)
* [Wikipedia](https://en.wikipedia.org/wiki/ANTLR)
