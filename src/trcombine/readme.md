# trcombine

## Summary

Combine a split Antlr4 grammar

## Description

Combine two grammars into one.
One grammar must be a lexer grammar, the other a parser grammar,
order is irrelevant. The output is parse tree data.

## Usage

    trcombine <grammar1> <grammar2>

## Details

`trcombine` combines grammars that are known as "split grammars"
(separate Antlr4 lexer and parser grammars)
into one grammar, known as a "combined grammar". This refactoring is
useful if a simplified grammar grammar is wanted, and if possible if
the split grammar does not use the "superClass" option in one or the other
grammars. The opposite refactoring is implemented by
[trsplit](https://github.com/kaby76/Trash/tree/main/trsplit).

The split refactoring performs several operations:

* Combine the two files together, parser grammar first, then lexer grammar.
* Remove the `grammarDecl` for the lexer rules, and change the `grammarDecl`
for the parser rules to be a combined grammar declaration. Rename the name
of the parser grammar to not have "Parser" at the tail of the name.
* Remove the `optionsSpec` for the lexer section.
* Remove any occurrence of "tokenVocab" from the `optionsSpec` of the parser section.
* If the `optionsSpec` is empty, it is removed.

The order of the two grammars is ignored: the parser rules always will appear
before the lexer rules.

Lexer modes will require manual fix-up.

## Example

Consider the following grammar that is split.

_Input to command_

Lexer grammar in ExpressionLexer.g4:

    lexer grammar ExpressionLexer;
    VARIABLE : VALID_ID_START VALID_ID_CHAR* ;
    fragment VALID_ID_START : ('a' .. 'z') | ('A' .. 'Z') | '_' ;
    fragment VALID_ID_CHAR : VALID_ID_START | ('0' .. '9') ;
    INT : ('0' .. '9')+ ;
    MUL : '*' ;
    DIV : '/' ;
    ADD : '+' ;
    SUB : '-' ;
    LP : '(' ;
    RP : ')' ;
    WS : [ \r\n\t] + -> skip ;

Parser grammar in ExpressionParser.g4:

    parser grammar ExpressionParser;
    e : e ('*' | '/') e
     | e ('+' | '-') e
     | '(' e ')'
     | ('-' | '+')* a
     ;
    a : number | variable ;
    number : INT ;
    variable : VARIABLE ;

_Command_

    trcombine ExpressionLexer.g4 ExpressionParser.g4 | trprint > Expression.g4

Combined grammar in Expression.g4:

    grammar Expression;
    e : e ('*' | '/') e
     | e ('+' | '-') e
     | '(' e ')'
     | ('-' | '+')* a
     ;
    a : number | variable ;
    number : INT ;
    variable : VARIABLE ;
    VARIABLE : VALID_ID_START VALID_ID_CHAR* ;
    fragment VALID_ID_START : ('a' .. 'z') | ('A' .. 'Z') | '_' ;
    fragment VALID_ID_CHAR : VALID_ID_START | ('0' .. '9') ;
    INT : ('0' .. '9')+ ;
    MUL : '*' ;
    DIV : '/' ;
    ADD : '+' ;
    SUB : '-' ;
    LP : '(' ;
    RP : ')' ;
    WS : [ \r\n\t] + -> skip ;

The original grammars are left unchanged.

## Current version

0.23.18 Remove old trst.

## License

The MIT License

Copyright (c) 2024 Ken Domino

Permission is hereby granted, free of charge, 
to any person obtaining a copy of this software and 
associated documentation files (the "Software"), to 
deal in the Software without restriction, including 
without limitation the rights to use, copy, modify, 
merge, publish, distribute, sublicense, and/or sell 
copies of the Software, and to permit persons to whom 
the Software is furnished to do so, 
subject to the following conditions:

The above copyright notice and this permission notice 
shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR 
ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
