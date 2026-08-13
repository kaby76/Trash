# Strip Leading Attributes from grammarDecl

Demonstrates removing hidden-channel token attributes (block comments, line
comments, whitespace) that precede the first real keyword (`lexer` or `parser`)
inside a `grammarType` node, using an XQuery element constructor.

## Background

The Antlr4 grammar parser stores *all* tokens — including off-channel ones
such as block comments, line comments, and whitespace — as XDM attributes on
the enclosing rule context.  In a `grammarDecl`, this means that any comments
appearing before the `lexer`/`parser` keyword end up as attributes on
`grammarType`:

```
grammarDecl
└── grammarType
    ├── Attribute BLOCK_COMMENT Value '/* … */' chnl:COMMENT
    ├── Attribute LINE_COMMENT  Value '// …'    chnl:COMMENT
    ├── Attribute WS            Value '\n\n'    chnl:OFF_CHANNEL
    ├── LEXER
    │   └── "lexer"
    └── GRAMMAR
        └── "grammar"
```

Because XDM attributes have no document-order position relative to child
elements, they cannot be filtered using `following-sibling::LEXER`.  The
solution is to reconstruct the `grammarDecl` element from scratch, keeping
only the child *nodes* of `grammarType` that do not precede `LEXER`/`PARSER`.

## Prerequisites

- `dotnet trash` installed (`dotnet tool install -g trash`)

## Run

```bash
bash run-example.sh
```

## How it works

1. `dotnet trash parse ExampleLexer.g4` parses the grammar and emits a JSON
   parse tree (`grammar.json`).
2. `dotnet trash xpath '//grammarDecl'` shows the original `grammarDecl`
   with its leading `Attribute` children.
3. `dotnet trash xquery -q strip-leading-attrs.xq` applies the XQuery, which:
   - Iterates over every `grammarDecl` in the tree.
   - Reconstructs `grammarType` keeping only child nodes that do **not** have
     a following `LEXER` or `PARSER` sibling (i.e. the keyword and everything
     after it).
   - Appends the remaining children of `grammarDecl` (`identifier`, `SEMI`,
     …) unchanged.
4. `dotnet trash tree` displays the cleaned result.
