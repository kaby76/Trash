// From https://www.geeksforgeeks.org/compiler-design/follow-set-in-syntax-analysis/
grammar E1;

s: e EOF;
e: t ep;
ep: '+' t ep | ;
t: f tp;
tp: '*' f tp | ;
f: '(' e ')' | ID;
ID: [a-z]+;
WS: [ \t\n\r]+ -> channel(HIDDEN);

