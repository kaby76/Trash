# Compile a Bison parser with an ANTLR4 lexer

[`Message.y`](Message.y) supplies Bison parser productions and
[`MessageLexer.g4`](MessageLexer.g4) supplies tokenization. Trash compiles both
grammar parse trees in one batch, then AllStar parses [`input.txt`](input.txt).

```sh
dotnet trash parse Message.y MessageLexer.g4 | dotnet trash interp -o interp
dotnet trash parse --allstar -L interp \
  --pinterp Bison_Message.interp --linterp MessageLexer.interp input.txt \
  | dotnet trash tree -a
```

Run `bash run-example.sh` to verify the pipeline. The Bison grammar uses
`%start` to select a rule other than the first, `%token NAME "name"` to give
the `NAME` token a string alias, and the literal character token `'!'`. The
lexer returns `NAME` for `Ada` and `BANG` for `!`; its named `BANG: '!';` rule
supplies the literal-to-token mapping. The token alias does **not** mean the
input must contain the word `name`.

The `.y` file does not define a scanner, and Trash does not compile Lex/Flex
`.l` files. An [ANTLR4](https://github.com/antlr/antlr4/blob/master/doc/lexer-rules.md)
or G4X lexer grammar can supply the tokens instead. See the
[Bison front-end documentation](../../src/trinterp/readme.md#basic-bison-interpretation)
for supported forms and limitations.
