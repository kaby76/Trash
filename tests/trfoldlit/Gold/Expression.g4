grammar Expression;
e : e (MUL | DIV) e
  | e (ADD | SUB) e
  | LP e RP
  | (SUB | ADD)* a
  ;
a : INT ;
INT : ('0' .. '9')+ ;
MUL : '*' ;
DIV : '/' ;
ADD : '+' ;
SUB : '-' ;
LP : '(' ;
RP : ')' ;
WS : [ \r\n\t] + -> skip ;
