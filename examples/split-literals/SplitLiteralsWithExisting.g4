grammar SplitLiteralsWithExisting;

start : 'on' ID 'end' EOF ;

TRSPLIT_LITERAL_1 : 'not-used' ;
ON : 'on' ;
ID : [a-z]+ ;
WS : [ \t\r\n]+ -> skip ;
