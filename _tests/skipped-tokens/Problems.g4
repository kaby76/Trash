grammar Problems; 
s: a* EOF ;
a: 'a' | COMMENT;
COMMENT: '//' ~[\n\r]* -> skip;
WS: [ \t\r\n]+ -> skip;
