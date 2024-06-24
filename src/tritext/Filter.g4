grammar Filter;

start : (expr (';' expr)* | ) EOF;
expr : expr ('<' | '>' | '<=' | '>=') expr
  | ID
  | NUM
  ;
ID : 'l1' | 'l2';
NUM : [0-9]+;
LT: '<';
GT: '>';
LE: '<=';
GE: '>=';
WS : [ \t\n\r] -> channel(HIDDEN);
