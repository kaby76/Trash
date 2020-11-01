grammar Expr;
e : e ('*' | '/') e
  | e ('+' | '-') e
  | '(' e ')'
  | ('-' | '+')* a
  ;
a : INT ;
id : ID ;
INT : ('0' .. '9')+ ;
MUL : '*' ;
DIV : '/' ;
ADD : '+' ;
SUB : '-' ;
LP : '(' ;
RP : ')' ;
ID : ( ('a' .. 'z') | ('A' .. 'Z') | '_' )+ ;
WS : [ \r\n\t] + -> skip ;
