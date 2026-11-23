# CongoCC target

`trgen -t CongoCC` generates a Java parser application. `build.sh` uses the
Trash pipeline to convert an ANTLR4 grammar with `convert.xq`, runs
CongoCC, and compiles the generated Java code.
For separate parser and lexer grammars, the build first runs `dotnet trash combine`
on their parse-result bundle. The converter requires action-free grammar rules.

Set `CONGOCC_JAR` to the CongoCC generator JAR. Its default is
`$HOME/congo-parser-generator/bin/congocc.jar`. Java and `javac` must be on
`PATH` (or set `JAVA` and `JAVAC`).

Use `bash run.sh -tree FILE` to print a parenthesized ANTLR
`ToStringTree()`-style parse tree to stderr. `--no-output` suppresses the timing
line while retaining the tree. The generated grammar sets
`SMART_NODE_CREATION=false` so single-child parser rules remain visible.
Direct left-recursion lowering may still produce a different tree shape from
an ANTLR parser.

This is an initial converter for named lexer tokens, character sets, string
literals, `-> skip`, `-> channel(HIDDEN)`, lexer fragments and ranges, grouped alternatives, basic repetition, and simple direct left recursion. The converter maps `channel(HIDDEN)` to CongoCC `SKIP`; other channels remain unsupported.
`build.sh` asks trinterp's C# choice analyzer for SCAN facts, then passes them
to the XQuery renderer. It handles a tail repetition that could consume the
start of the next enclosing rule (VMF), and overlapping alternatives made only
of terminals when the containing rule's following tokens can decide the choice
(Words). It also uses a syntactic `SCAN` for bounded, prefix-disjoint terminal
alternatives and for a repeated `rule separator` before `rule terminator`
(ABB). These are conservative cases, not a general translation of ANTLR4
adaptive prediction. For a repeated outer rule with a distinctive two-token
header, a nested repeated item can use a negative syntactic check to stop
before the next outer rule (ABNF). The left-recursion rewrite does not preserve ANTLR4 precedence or associativity. Parser rule names are preserved by default. `convert.xq` accepts the external `$prefix` parameter; `build.sh` passes `CONGOCC_RULE_PREFIX`, which defaults to the empty string. If the generated Java build encounters an AST class collision with a lexer token, rebuild with `CONGOCC_RULE_PREFIX=ParserRule_ bash build.sh`. The generated driver reads the prefix saved by the build, so `run.sh` needs no matching environment setting.
Unresolved overlapping top-level alternatives produce a warning during build.
The converter accepts negated lexer character sets and expands supported
ANTLR Unicode properties into explicit ranges. It rejects recursive lexer-rule
references before invoking CongoCC, whose NFA builder overflows on such cycles.
For example, the VBA6 grammar has `WS -> LINE_CONTINUATION -> WS`.
See `examples/words-congocc` and `examples/congocc-split` for runnable examples.
