# ANTLR4 grammar to class-like antlr-ng sketch

[`convert.xq`](convert.xq) rewrites the parse tree of one ANTLR4 `.g4` file into the class-like layout [sketched for antlr-ng](https://github.com/antlr-ng/antlr-ng/issues/129#issuecomment-6771744669):

```text
grammar ClassSyntaxDemo {
  lexer {
    If : 'if' ;
    Identifier : Letter + ;
    order {
      If before Identifier;
    }
  }
  parser {
    start : ( If Identifier | Identifier ) EOF ;
  }
}
```

Lexer rules go under `lexer {}`, parser rules under `parser {}`, and modes become nested `mode Name {}` blocks. The query emits `order` statements for adjacent non-fragment lexer rules to express their original ANTLR4 priority. It retains grammar prequels such as `options`, and copies rule token contents and comments while normalizing whitespace. Combined, lexer-only, and parser-only inputs are covered.

This is **illustrative output**. The proposed antlr-ng syntax is not finalized or accepted by an existing parser. Prequel semantics, implicit literal tokens, imports, actions, and all lexical-priority interactions are not validated for a future dialect. This converter does not claim behavioral equivalence.

Run from any directory, passing one input grammar:

```sh
bash /path/to/examples/antlr-ng-class-syntax/convert-antlr4.sh /path/to/Grammar.g4 > Grammar.class.txt
```

`bash run-example.sh` prints the combined-grammar conversion. `bash test-example.sh` compares it and the lexer-mode and parser-only fixtures with the files in [`expected/`](expected/).
