# Kotlin lexer import regression

These grammar files and `examples/kotlinFile/fix-3218.kt` were copied from
`grammars-v4/kotlin/kotlin`. `KotlinLexer.g4` imports `UnicodeClasses.g4`.
The regression test checks that `trinterp` includes the imported lexer rules
and that the AllStar interpreter parses the input without emitting an empty
`Identifier` token. See `LICENSE` for the grammar's license.
