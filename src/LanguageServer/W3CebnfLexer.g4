
// https://www.w3.org/TR/REC-xml/#sec-notation

lexer grammar W3CebnfLexer;

channels { OFF_CHANNEL }

PPEQ : '::=' ;
Q : '?' ;
VP : '|' ;
M : '-' ;
P : '+' ;
S : '*' ;
OP : '(' ;
CP : ')' ;
CONSTRAINT : '[' Ws? Symbol Ws? ':' .*? ']' -> channel(OFF_CHANNEL) ;
COMMENT : '/*' .*? '*/' -> channel(OFF_CHANNEL) ;
HEX : Hex ;
STRING : '"' .*? '"' | '\'' .*? '\'' ;
SET : '[' '^'? ( Hex | . )+? ']' ; 
SYMBOL : Symbol ;
WS : Ws -> channel(OFF_CHANNEL) ;
fragment Symbol : [a-zA-Z0-9_.\-] [a-zA-Z0-9_.\-]* ;
fragment Hex : '#x' [0-9a-fA-F]+ ;
fragment Ws : [ \t\r\n]+ ;
