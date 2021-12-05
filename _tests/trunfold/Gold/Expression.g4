grammar Expresion;
s : ( e '*' e | INT ) ;
e : e '*' e       # Mult
    | INT         # primary
    ;
INT : [0-9]+ ;
WS : [ \t\n]+ -> skip ;
