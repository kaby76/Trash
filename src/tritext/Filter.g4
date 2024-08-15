grammar Filter;

start : (expr | ) EOF;
expr
  : expr ('<' | '>' | '<=' | '>=') expr
  | expr ('&&' | '||') expr
  | ID
  | NUM
  ;
ID : 's0' | 's1' | 'e0' | 'e1';
NUM : [0-9]+;
LT: '<';
GT: '>';
LE: '<=';
GE: '>=';
AND: '&&';
OR: '||';
WS : [ \t\n\r] -> channel(HIDDEN);
