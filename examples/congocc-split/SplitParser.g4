parser grammar SplitParser;

options { tokenVocab = SplitLexer; }

document : (value COMMA)* value SEMI EOF;
value : (PLUS | MINUS)? FLOAT | (PLUS | MINUS)? INT;
