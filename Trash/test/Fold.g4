grammar A;
s : a ;
a : e ( ';' e )+ ;
e : e '*' e | INT ;
SEMI : ';' ;
MUL : '*' ;
INT : [0-9]+ ;
WS : [ \t\n]+ -> skip ;
