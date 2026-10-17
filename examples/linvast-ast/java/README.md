# Java → LINVAST-style AST mapping

[`java.ast`](java.ast) approximates the structural reductions in LINVAST's
`JavaASTBuilder` partials. The copied `Java.g4` and `JavaLexer.g4` are
unchanged. The example checks a class with a method and branch, two source
types, a field, parameters, arithmetic, and array indexing. The result is a
normal `.pt` bundle, so it can be queried with XQuery4.

Mapped kinds include `SourceNode`, `TypeDeclNode`, `FuncNode`, `FuncDeclNode`,
`FuncParamsNode`, `FuncParamNode`, `DeclStatNode`, `VarDeclNode`, `BlockStatNode`,
`IfStatNode`, `WhileStatNode`, `JumpStatNode`, `ArrAccessExprNode`, and
arithmetic, relational, logical, assignment, call, and literal expressions.
Repeated class members, block statements, imports, and top-level types use
the `.ast` repetition syntax.

This is not a typed LINVAST AST serializer. Package/import names and some
operator metadata are approximated. Advanced generics, annotations, lambdas,
method references, most `new` forms, multi-element comma lists, complex
class/interface/enum bodies, `try`, `switch`, and `for` are not fully mapped.
Unmapped multi-child constructs fail explicitly; no raw CST subtree is
silently included.
