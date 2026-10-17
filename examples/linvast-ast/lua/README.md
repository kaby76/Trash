# Lua → LINVAST-style AST mapping

[`lua.ast`](lua.ast) approximates LINVAST's `LuaASTBuilder` expression and
statement visitors. Whole-source fixtures check local declaration, `while`,
numeric `for`, array indexing, a named function, a call, arithmetic, and
`return`. The mapping also handles basic `if`/`else`, table entries,
comparisons, logical/unary expressions, and block/statement lists.

The source result contains a `BlockStatNode` inside `SourceNode`; LINVAST's
builder sometimes flattens that block and synthesizes declarations using a
scope-sensitive pass. This structural mapping cannot reproduce that symbol
table behavior. Multiple assignment/argument lists, mixed table fields,
closures with complex parameter lists, repeat-until semantics, method-name
concatenation, and variadic functions are incomplete. Operators are text
nodes rather than LINVAST enum/function instances. Unmapped multi-child
forms fail explicitly.
