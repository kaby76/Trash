lexer grammar SplitLexer;

options { caseInsensitive = true; }

FLOAT : [0-9]+ '.' [0-9]+;
INT : [0-9]+;
PLUS : '+';
MINUS : '-';
COMMA : ',';
SEMI : ';';
WS : [ \t\r\n]+ -> skip;
