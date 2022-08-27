grammar Repeat;
file_: (a<4> | b<4>)* EOF;
a : 'a';
b : 'b';
WS: [ \t\n\r]+ -> channel(HIDDEN);
