# ABC lexer modes and parser-directed tokenization

The original [ABC grammar](https://github.com/antlr/grammars-v4/tree/master/abc)
uses a `BODY` lexer mode. `KFIELD` enters that mode; `XFIELD2` returns to the
default mode and emits the `XFIELD` token type. The original grammar is in
[`source/`](source/). The XQuery4 file
[`rewrite-abc-modes.xq`](rewrite-abc-modes.xq) moves the distinct body token
rules into the default mode, removes the duplicate skip and field rules, and
removes the `KFIELD` mode switch. The resulting lexer is shown by the checked-in
[`abcLexer.g4`](abcLexer.g4).

AllStar accepts the mode-free grammar with `--context-aware-lexing`. It also
accepts these 14 examples **without** that switch: the lexer rule order and
longer field matches already distinguish `XFIELD`, `KFIELD`, and `FIELD` here.
The switch is tested to show that the rewritten grammar remains compatible
with parser-directed lexing, not because this corpus requires it. This does
**not** imply that all lexer modes can be removed: modes can encode state that
the parser's expected tokens cannot recover.

CongoCC's `CONTEXTUAL` tokens require literal strings, whereas ABC fields are
patterns. The mode-free grammar nevertheless parses the ABC examples in CongoCC
with ordinary tokenization. The CongoCC converter also translates the original
grammar's mode switches into [CongoCC lexical states](https://github.com/congo-cc/congo-docs/blob/main/source/docs/reference/lexical.rst),
and expands `type(...)` aliases at parser token references. The examples use
the build's automatic `ParserRule_` prefix to avoid Java class-name collisions
between parser rules and tokens that differ only by case.

Build Trash in Release mode, then run `bash run-example.sh`. The script checks
all 14 ABC samples with the rewritten AllStar grammar, a CongoCC parser built
from the mode-free grammar, and a CongoCC parser built from the original
mode-based grammar. It skips CongoCC checks if its JAR or Java is unavailable.
