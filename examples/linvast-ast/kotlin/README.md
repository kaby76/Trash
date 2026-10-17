# Kotlin → LINVAST-style AST mapping

[`kotlin.ast`](kotlin.ast) approximates LINVAST's `KotlinASTBuilder` partials.
The executable fixtures cover source files, empty-parameter functions,
return expressions, a property initializer, a class, arithmetic, and jumps.
The mapping also sketches conditionals, loops, comparisons, and simple
typed declarations. `SourceNode`, `FuncNode`, `TypeDeclNode`, `DeclStatNode`,
`BlockStatNode`, `JumpStatNode`, `ArithmExprNode`, and their component nodes
are produced during the ALL(*) parse.

The original LINVAST Unicode lexer is preserved in `upstream/`. The active
lexer is an ASCII-only demo copy. A typed function parameter such as
`fun f(x: Int) {}` is currently rejected by the interpreter even without
`--ast-map`; so is a minimal function containing `if (1) {}`. Parameter and
conditional rewrites are therefore not exercised by the whole-file tests.
Complex Kotlin syntax such as type parameters, lambdas,
receiver functions, string interpolation, `when`, `try`, destructuring, and
multiline separator combinations remains incomplete. No source metadata or
typed LINVAST objects are generated. Unmapped multi-child forms fail rather
than passing a CST through the AST.
