# trfoldlit

## Summary

Perform fold transform on grammar with literals

## Description

Reads a parse tree from stdin, replaces a string literals on
the RHS of a rule with the lexer rule LHS symbol, and writes
the modified parsing result set to stdout. The input and
output are Parse Tree Data.

## Usage

    trfoldlit

## Examples

Before:

    grammar Expression;
    e : e ('*' | '/') e
      | e ('+' | '-') e
      | '(' e ')'
      | ('-' | '+')* a
      ;
    a : INT ;
    INT : ('0' .. '9')+ ;
    MUL : '*' ;
    DIV : '/' ;
    ADD : '+' ;
    SUB : '-' ;
    LP : '(' ;
    RP : ')' ;
    WS : [ \r\n\t] + -> skip ;

Command:

    trparse Expression.g4 | trfoldlit | trsponge -c

After:

    grammar Expression;
    e : e (MUL | DIV) e
      | e (ADD | SUB) e
      | LP e RP
      | (SUB | ADD)* a
      ;
    a : INT ;
    INT : ('0' .. '9')+ ;
    MUL : '*' ;
    DIV : '/' ;
    ADD : '+' ;
    SUB : '-' ;
    LP : '(' ;
    RP : ')' ;
    WS : [ \r\n\t] + -> skip ;

## Notes

If you are running MSYS2 on Windows, you may notice that XPaths are not being
processed by this command correctly. To avoid the Bash shell from altering
XPaths, type _export MSYS2_ARG_CONV_EXCL="*"_, then execute your command.

## Current version

0.18.2 Fix trperf, trfirst. Adding Xalan code. Fix #180. Fix crash in trgen https://github.com/antlr/grammars-v4/issues/2818. Fix #134. Add -e option to trrename. Update Antlr4BuildTasks version. Fix #197, #198. Fix trparse exit code. Add --quiet option to trparse to just get exit code. Change trgen templates to remove -file option, make file name parsing the default.
