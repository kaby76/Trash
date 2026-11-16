grammar SplitLiterals;

start : 'on' ID 'end' EOF ;

ID : [a-z]+ ;
WS : [ \t\r\n]+ -> skip ;
