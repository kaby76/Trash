/*
 * Minimal example lexer grammar used by the strip-leading-attrs example.
 * The block comment and line comments above the 'lexer' keyword are stored
 * as hidden-channel token attributes on grammarType by the Antlr4 parser —
 * this example shows how to strip them out via an XQuery.
 */

// $antlr-format alignTrailingComments true, columnLimit 100

lexer grammar ExampleLexer;

ID  : [a-zA-Z_] [a-zA-Z0-9_]* ;
INT : [0-9]+ ;
STR : '"' (~["\r\n] | '\\' .)* '"' ;
WS  : [ \t\r\n]+ -> skip ;
