grammar Arithmetic;

start : expr EOF ;
expr : term ('+' expr)? ;
term : atom ('*' term)? ;
atom : NUMBER | '(' expr ')' ;

NUMBER : [0-9]+ ;
WS : [ \t\r\n]+ -> skip ;
