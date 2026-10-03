parser grammar FooParser;
options { tokenVocab=FooLexer; }
start: stmt* EOF;
stmt: (expr | decl) ';';
decl: 'var' IDENTIFIER;
expr: expr ('+'|'-') expr | ('-'|'+')+ expr | IDENTIFIER | NUMBER;
