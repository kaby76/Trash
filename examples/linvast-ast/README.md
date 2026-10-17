# A common AST from four LINVAST grammars

This proof of concept copies LINVAST's action-free C, Java, Kotlin, and Lua
grammars and uses a separate `.ast` mapping for each language. The mapping is
applied **during** `trparse --allstar`, so the `.pt` file in its default PAX/tar
output contains `UnvParseTreeNode` AST nodes rather than a CST. All four
languages produce the same shapes for the tested expression fragments:

```text
(IdNode x)
(LitExprNode 42)
```

These names follow LINVAST's `IdNode` and `LitExprNode` vocabulary. They are
DOM nodes in Trash's parse-result format, **not** instantiated LINVAST C#
objects. The example does not claim to implement LINVAST's complete AST or
all constructs in the four languages. C covers representative expressions,
declarations, functions, blocks, branches, and loops. Java now covers simple
source files, classes, fields, methods, parameters, expressions, and control
flow. Kotlin covers simple top-level functions, properties, classes,
expressions, and jumps. Lua covers blocks, local declarations, functions,
calls, arithmetic, branches, loops, and table fragments. See the mapping
notes for [C](c/README.md), [Java](java/README.md),
[Kotlin](kotlin/README.md), and [Lua](lua/README.md) for exact coverage and
limitations. Indirect left recursion and stateful hooks still cannot be
combined with `--ast-map`.

Run `bash run-example.sh` from any directory. The script compiles each grammar
to interpreter tables, parses identifier and integer inputs, compares all
eight shared leaf ASTs, checks whole-source rewrites in each language, and
confirms that XQuery4 can consume the resulting `.pt` bundle. Java/Kotlin/Lua
checks include repeated and optional children in source and block rules.
For example, after producing interpreter files:

```sh
dotnet trash parse --allstar -L c/interp --start-rule primaryExpression \
  --ast-map c/c.ast input/identifier.txt | dotnet trash tree --text -a
```

The grammar files come from `LINVAST.Imperative/Builders/{C,Java,Kotlin,Lua}/ANTLR`
in [LINVAST](https://github.com/LINVAST/LINVAST); their upstream notices are
retained in the copied files. C, Java, and Lua are unmodified. The Kotlin
parser is unmodified, but its full Unicode lexer stalls in the current
interpreter even for one-character input. The original lexer is preserved at
`kotlin/upstream/KotlinLexer.g4`; the active `kotlin/KotlinLexer.g4` is an
explicitly limited ASCII-only demo copy (letters and decimal digits). The
copied `UnicodeClasses.g4` remains for reference but is not compiled by this
example. This adaptation is unsuitable for general Kotlin source; in
particular, typed function-parameter and minimal `if` fixtures are currently
rejected by the interpreter even without AST rewriting. The tested Kotlin
functions therefore use empty parameter lists and no conditional bodies.
