lexer grammar MessageLexer;

NAME: [a-zA-Z]+;
BANG: '!';
WS: [ \t\r\n]+ -> skip;
