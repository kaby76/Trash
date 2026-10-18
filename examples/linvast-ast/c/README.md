# C → LINVAST-style AST mapping

[`c.ast`](c.ast) approximates the reductions in LINVAST's
`CASTBuilder.cs` and its `Expressions`, `Declarations`, `Functions`, and
`Statements` partials. It operates on committed ALL(*) events during the
parse; `C.g4` is copied unchanged. Compile `C.g4` and `c.ast` together with
`trinterp`; `trparse --allstar` then emits the AST as an
ordinary `.pt` bundle, which can be piped to `dotnet trash tree --text -a` or
`dotnet trash xquery`.

The executable examples cover these LINVAST-named structural nodes:

| Area | AST kinds exercised |
|------|---------------------|
| Source/functions | `SourceNode`, `FuncNode`, `FuncDeclNode`, `FuncParamsNode`, `FuncParamNode`, `TypeNameListNode` |
| Expressions | `IdNode`, `LitExprNode`, `ArithmExprNode`, `ArithmOpNode`, `RelExprNode`, `RelOpNode`, `AssignExprNode`, `AssignOpNode`, `ArrAccessExprNode`, `IncExprNode`, `DecExprNode` |
| Declarations | `DeclSpecsNode`, `TypeNameNode`, `DeclStatNode`, `DeclListNode`, `VarDeclNode`, `ArrDeclNode`, `ArrInitExprNode` |
| Statements | `BlockStatNode`, `ExprStatNode`, `IfStatNode`, `WhileStatNode`, `ForStatNode`, `JumpStatNode` |

Other mapping rules approximate logical and bitwise expressions, conditional
expressions, calls, empty statements, `switch`, and labels. `run-example.sh`
checks the arithmetic and function trees exactly; additional whole-source
fixtures check control flow, arrays, and flattening of two functions in one
`SourceNode` through XQuery4.

Three mapping features make the larger example possible:

- Direct left recursion is reduced at each `PushRecursionContext` event, so
  `1+2*3` becomes nested `ArithmExprNode` values with the correct precedence.
- `op:_` captures a child regardless of its grammar symbol. A guard such as
  `decl:declarator[ArrDeclNode]` inspects the already-reduced AST kind, allowing
  arrays and ordinary variables to take different rewrite rules.
- `...left` splices children from an existing node of the same AST kind. It
  turns left-recursive translation units, parameter lists, block lists, and
  initializer lists into flat AST lists. When kinds differ, it appends `left`
  as one child.

This remains a structural approximation, **not** a LINVAST AST serializer:
DOM nodes do not encode LINVAST's C# class instances, line numbers, resolved
types, modifiers, pointer levels, or enum-valued operators. The mapping does
not cover every `CASTBuilder` branch. Notable unsupported forms include
struct/union/enum declarations, typedefs, pointer declarators, complex casts,
GNU extensions, generic selections, `do` loops, and optional `for` clauses.
Unmapped multi-child forms fail explicitly because `default otherwise ->
error` is set; they are not silently passed through as CST fragments.
