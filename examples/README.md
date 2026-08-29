# Examples

Runnable examples demonstrating common Trash toolchain workflows.

| Directory | Description |
|-----------|-------------|
| [`abnf-interp/`](abnf-interp/) | Parse ABNF/BNF files using generated `.interp` files (no code generation step) |
| [`first/`](first/) | Compute nullable parser rules and grammar-theoretic FIRST sets with an XQuery4 fixed-point analysis |
| [`ixml-to-antlr4/`](ixml-to-antlr4/) | Convert an iXML grammar to Antlr4 syntax using a five-pass XQuery Update pipeline (structural syntax, encoded characters, character sets, inline-set extraction, separator quantifiers) |
| [`java-antlr/`](java-antlr/) | Generate a Java-target Antlr4 parser for the Java grammar, build, and run |
| [`lark-to-antlr4/`](lark-to-antlr4/) | Convert a Lark grammar to Antlr4 syntax using a multi-pass XQuery Update pipeline |
| [`kleene/`](kleene/) | Eliminate direct left and right recursion from parser rules using XQuery Update scripts (`kleene-lr.xq`, `kleene-rr.xq`), replacing recursive alternatives with Kleene-star EBNF |
| [`rule-names/`](rule-names/) | Construct XML containing one element for every parser-rule name selected from an ANTLR4 grammar parse tree |
| [`ungroup/`](ungroup/) | Expand a plain grouped alternative `(X \| Y) B` into distributed top-level alternatives `X B \| Y B` using an XQuery Update script with an external variable parameter |
| [`strip-leading-attrs/`](strip-leading-attrs/) | Remove hidden-channel token attributes before `lexer`/`parser` in `grammarDecl` using an XQuery element constructor |
| [`xpath31-to-antlr4/`](xpath31-to-antlr4/) | Parse the XPath 3.1 EBNF grammar using generated `.interp` files built from the XPath 3.1 meta-grammar (work in progress) |

Each example directory contains a `run-example.sh` and its own `README.md`.
