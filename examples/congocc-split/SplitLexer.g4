lexer grammar SplitLexer;

options { caseInsensitive = true; }

FLOAT : [0-9]+ '.' [0-9]+;
INT : [0-9]+;
PLUS : '+';
MINUS : '-';
COMMA : ',';
SEMI : ';';
NEWLINE : '\r'? '\n';
WS : [ \t]+ -> skip;
COMMENT : '#' ~[\r\n]* -> channel(HIDDEN);
BLOCK_COMMENT : '/*' .*? '*/' -> skip;
