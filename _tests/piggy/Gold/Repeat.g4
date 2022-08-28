grammar Repeat;
file_: (  a a a a  |   b b b b )* EOF;
a : 'a';
b : 'b';
WS: [ \t\n\r]+ -> channel(HIDDEN);

