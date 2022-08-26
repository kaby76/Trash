grammar Repeat;
file_: (a<1,4> | b<1,4>)* EOF;
a : 'a';
b : 'b';
WS: [ \t\n\r]+ -> channel(HIDDEN);
