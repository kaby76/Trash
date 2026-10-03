lexer grammar FooLexer;
VAR_ : 'var';
SEMI : ';';
IDENTIFIER : [a-zA-Z] ([a-zA-Z0-9] | { xq("hyphen.xq") }? '-')* ;
MINUS : '-' ;
PLUS : '+';
NUMBER : [0-9]+ ;
WS : [ \t\r\n]+ -> skip ;
