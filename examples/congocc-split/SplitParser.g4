parser grammar SplitParser;

options { tokenVocab = SplitLexer; }

document : (value COMMA)* value SEMI NEWLINE? EOF;
value : (PLUS | MINUS)? FLOAT | (PLUS | MINUS)? INT;
