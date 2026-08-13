grammar Expresion;
s : e '*' e
    | INT ;
e : e '*' e
    | INT
    ;
INT : [0-9]+ ;
WS : [ \t\n]+ -> skip ;
