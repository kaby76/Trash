# trconvert

## Summary

Convert a grammar from one for to another

## Description

Reads a grammar from stdin and converts the grammar to/from Antlr version 4
syntax. The original grammar must be for a supported type (Antlr2, Antlr3,
Bison, W3C EBNF, Lark). The input and output are Parse Tree Data.

## Usage

    trconvert [-t <type>]

## Details

This command converts a grammar from one type to another. Most
conversions will handle only simple syntax differences. More complicated
scenarios are supported depending on the source and target grammar types.
For example, Bison is converted to Antlr4, but the reverse is not
implemented yet.

`trconvert` takes an option target type. If it is not used, the default
is to convert the input of whatever type to Antlr4 syntax. The output
of `trconvert` is a parse tree containing the converted grammar.

## Examples

_Conversion of Antlr4 Abnf to Lark Abnf_

    grammar Abnf;

    rulelist : rule_* EOF ;
    rule_ : ID '=' '/'? elements ;
    elements : alternation ;
    alternation : concatenation ( '/' concatenation )* ;
    concatenation : repetition + ;
    repetition : repeat_? element ;
    repeat_ : INT | ( INT? '*' INT? ) ;
    element : ID | group | option | STRING | NumberValue | ProseValue ;
    group : '(' alternation ')' ;
    option : '[' alternation ']' ;
    NumberValue : '%' ( BinaryValue | DecimalValue | HexValue ) ;
    fragment BinaryValue : 'b' BIT+ ( ( '.' BIT+ )+ | ( '-' BIT+ ) )? ;
    fragment DecimalValue : 'd' DIGIT+ ( ( '.' DIGIT+ )+ | ( '-' DIGIT+ ) )? ;
    fragment HexValue : 'x' HEX_DIGIT+ ( ( '.' HEX_DIGIT+ )+ | ( '-' HEX_DIGIT+ ) )? ;
    ProseValue : '<' ( ~ '>' )* '>' ;
    ID : LETTER ( LETTER | DIGIT | '-' )* ;
    INT : '0' .. '9'+ ;
    COMMENT : ';' ~ ( '\n' | '\r' )* '\r'? '\n' -> channel ( HIDDEN ) ;
    WS : ( ' ' | '\t' | '\r' | '\n' ) -> channel ( HIDDEN ) ;
    STRING : ( '%s' | '%i' )? '"' ( ~ '"' )* '"' ;
    fragment LETTER : 'a' .. 'z' | 'A' .. 'Z' ;
    fragment BIT : '0' .. '1' ;
    fragment DIGIT : '0' .. '9' ;
    fragment HEX_DIGIT : ( '0' .. '9' | 'a' .. 'f' | 'A' .. 'F' ) ;

_Command_

    trparse Abnf.g4 | trconvert -t lark | trprint > Abnf.lark

_Output_

    rulelist :  rule_ * EOF 
    rule_ :  ID "=" "/" ? elements 
    elements :  alternation 
    alternation :  concatenation ( "/" concatenation ) * 
    concatenation :  repetition + 
    repetition :  repeat_ ? element 
    repeat_ :  INT | ( INT ? "*" INT ? ) 
    element :  ID | group | option | STRING | NUMBERVALUE | PROSEVALUE 
    group :  "(" alternation ")" 
    option :  "[" alternation "]" 
    NUMBERVALUE :  "%" ( BINARYVALUE | DECIMALVALUE | HEXVALUE ) 
    BINARYVALUE :  "b" BIT + ( ( "." BIT + ) + | ( "-" BIT + ) ) ? 
    DECIMALVALUE :  "d" DIGIT + ( ( "." DIGIT + ) + | ( "-" DIGIT + ) ) ? 
    HEXVALUE :  "x" HEX_DIGIT + ( ( "." HEX_DIGIT + ) + | ( "-" HEX_DIGIT + ) ) ? 
    PROSEVALUE :  "<" ( /(?!>)/ ) * ">" 
    ID :  LETTER ( LETTER | DIGIT | "-" ) * 
    INT :  "0" .. "9" + 
    COMMENT :  ";" /(?!\n|\r)/ * "\r" ? "\n" 
    WS :  ( " " | "\t" | "\r" | "\n" ) 
    STRING :  ( "%s" | "%i" ) ? "\"" ( /(?!")/ ) * "\"" 
    LETTER :  "a" .. "z" | "A" .. "Z" 
    BIT :  "0" .. "1" 
    DIGIT :  "0" .. "9" 
    HEX_DIGIT :  ( "0" .. "9" | "a" .. "f" | "A" .. "F" ) 

    %ignore COMMENT
    %ignore WS

## Current version

0.23.28 Updated trgen grammar analysis, for https://github.com/antlr/grammars-v4/pull/4695.

## License

The MIT License

Copyright (c) 2025 Ken Domino

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
