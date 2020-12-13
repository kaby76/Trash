grammar A;
s : x ;
x : e ;
e : e '*' e | INT ;
INT : [0-9]+ ;
WS : [ \t\n]+ -> skip ;
