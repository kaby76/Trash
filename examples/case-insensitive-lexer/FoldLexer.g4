lexer grammar FoldLexer;

options { caseInsensitive = true; }

SELECT : 'Select';
HEX : [A-F0-9]+;
RANGE : 'G'..'Z';
NOT_LETTER : '~' ~[A-Z];
WS : [ \t\r\n]+ -> skip;
