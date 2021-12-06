grammar Expression;
e : e ('*' | '/') e
  | e ('+' | '-') e
  | '(' e ')'
  | ('-' | '+')* a
  ;
atom : INT ;
INT : ('0' .. '9')+ ;
MUL : '*' ;
DIV : '/' ;
ADD : '+' ;
SUB : '-' ;
LP : '(' ;
RP : ')' ;
WS : [ \r\n\t] + -> skip ;
