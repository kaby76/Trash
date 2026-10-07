# Java grammar with AllStar and XQuery4 predicates

This example uses the [grammars-v4 `java/java` grammar](https://github.com/antlr/grammars-v4/tree/master/java/java)
without generating target-language parser code. `trinterp` creates the Java
lexer and parser tables, and `trparse --allstar` reads them. The lexer grammar
has no semantic predicates. The parser's two C# `JavaParserBase` predicates
are replaced by target-neutral XQuery4 files:

| Parser rule | Query | Purpose |
|---|---|---|
| `annotationFieldValue` | `is-not-identifier-assign.xq` | Use the positional annotation value alternative unless the next two tokens are an identifier-like token and `ASSIGN`. |
| `recordComponentList` | `last-record-component.xq` | Reject a varargs (`...`) component unless it is the final record component. |

`hooks.json` binds the queries by parser rule and predicate index. The first
query participates in prediction (`"predict": true`); the second is checked
after the components have been parsed, when the partial parse tree exists.

Run `bash test-example.sh` for a small, offline test. It generates interpreter
tables, accepts `input.java`, rejects `invalid-record.java`, and verifies that
bundle parsing adds `.pt` and `.errors` sidecars under `src/`.

Run `bash run-example.sh` to download the
[OpenJDK 21 GA source archive](https://github.com/openjdk/jdk/tree/jdk-21-ga),
unpack it, and parse its `src/**/*.java` files. This is a large workload:
allow time and free disk space for the extracted sources and resulting PAX/tar
bundle. The archive, extracted sources, tables, and output are ignored by Git.
The final bundle is `data/jdk21-parsed.tar`.
