grammar Nest;

s    : subj WHEN EOF ;
subj : value | cond ;
value: expr ;
cond : expr | expr EQ expr ;
expr : {true}? FN LP arg RP | {true}? FN LP arg RP | ID ;
arg  : expr | value | cond ;

WHEN : 'WHEN' ;
FN   : 'F' ;
EQ   : '=' ;
LP   : '(' ;
RP   : ')' ;
ID   : [A-Z] ;
WS   : ' ' -> skip ;
