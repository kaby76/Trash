grammar Expression;
exp : exp ('*' | '/') exp
  | exp ('+' | '-') exp
  | '(' exp ')'
  | ('-' | '+')* atom
  ;
atom : Int ;
Int : ('0' .. '9')+ ;
OpMul : '*' ;
OpDiv : '/' ;
OpAdd : '+' ;
OpSub : '-' ;
LP : '(' ;
RP : ')' ;
WS : [ \r\n\t] + -> skip ;
