grammar IndirectLeftRecursion;

start
    : expression EOF
    ;

expression
    : additionExpression
    | INT
    ;

additionExpression
    : expression PLUS INT
    ;

PLUS : '+' ;
INT  : [0-9]+ ;
WS   : [ \t\r\n]+ -> skip ;
