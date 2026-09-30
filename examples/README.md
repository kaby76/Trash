# Examples

Runnable examples demonstrating common Trash toolchain workflows.

| Directory | Description |
|-----------|-------------|
| [`abnf-interp/`](abnf-interp/) | Parse ABNF/BNF files using generated `.interp` files (no code generation step) |
| [`abnf-frontend/`](abnf-frontend/) | Compile a grammar written in ABNF to `.interp` files and parse its input with AllStar |
| [`antlr4-sort/`](antlr4-sort/) | Sort all ANTLR4 parser rules alphabetically with XQuery4, including EOF-terminated start rules |
| [`bison-frontend/`](bison-frontend/) | Compile Bison productions with a separate ANTLR4 lexer and parse input with AllStar |
| [`context-aware-lexing/`](context-aware-lexing/) | Parse a Decaf array declaration whose overlapping integer tokens require parser-directed lexical selection |
| [`first/`](first/) | Compute nullable parser rules and grammar-theoretic FIRST sets with an XQuery4 fixed-point analysis |
| [`g4x-exclusion/`](g4x-exclusion/) | Parse JLS-style identifier rules using G4X's set-difference syntax (syntax only) |
| [`g4x-interp/`](g4x-interp/) | Generate tables from case-neutral G4X lexer/parser grammars and verify a left-recursive expression tree with AllStar |
| [`rex-interp/`](rex-interp/) | Generate tables from basic REx arithmetic and list grammars, verify parse trees, and check rejection of invalid input |
| [`rex-sort/`](rex-sort/) | Sort REx syntax and lexical rules alphabetically using XQuery4 while preserving their parse-tree content |
| [`ixml-interp/`](ixml-interp/) | Compile basic scannerless iXML directly to interpreter tables, with arithmetic and separated-list tree checks |
| [`ixml-to-antlr4/`](ixml-to-antlr4/) | Convert an iXML grammar to Antlr4 syntax using a five-pass XQuery Update pipeline (structural syntax, encoded characters, character sets, inline-set extraction, separator quantifiers) |
| [`java-antlr/`](java-antlr/) | Generate a Java-target Antlr4 parser for the Java grammar, build, and run |
| [`lark-to-antlr4/`](lark-to-antlr4/) | Convert a Lark grammar to Antlr4 syntax using a multi-pass XQuery Update pipeline |
| [`kleene/`](kleene/) | Eliminate direct left and right recursion from parser rules using XQuery Update scripts (`kleene-lr.xq`, `kleene-rr.xq`), replacing recursive alternatives with Kleene-star EBNF |
| [`rule-names/`](rule-names/) | Construct XML containing one element for every parser-rule name selected from an ANTLR4 grammar parse tree |
| [`ungroup/`](ungroup/) | Expand a plain grouped alternative `(X \| Y) B` into distributed top-level alternatives `X B \| Y B` using an XQuery Update script with an external variable parameter |
| [`strip-leading-attrs/`](strip-leading-attrs/) | Remove hidden-channel token attributes before `lexer`/`parser` in `grammarDecl` using an XQuery element constructor |
| [`tree-sitter-to-w3c-ebnf/`](tree-sitter-to-w3c-ebnf/) | Convert Tree-sitter `grammar.json` rules to structural W3C EBNF with XQuery4, explicitly marking unsupported constructs |
| [`w3cebnf-frontend/`](w3cebnf-frontend/) | Compile a basic W3C EBNF grammar to `.interp` tables and parse its input with AllStar |
| [`xpath31-to-antlr4/`](xpath31-to-antlr4/) | Parse the XPath 3.1 EBNF grammar using generated `.interp` files built from the XPath 3.1 meta-grammar (work in progress) |

Each example directory contains a `run-example.sh` and its own `README.md`.

## Testing

Run every example as an integration test with:

```sh
bash test.sh
```

The repository-wide `tests/all-tests.sh` driver also invokes this test. Every
example must complete successfully. Examples with deterministic output can add
a `test-example.sh` containing stronger assertions; `first/`, for example,
compares its output with `expected.txt`.
