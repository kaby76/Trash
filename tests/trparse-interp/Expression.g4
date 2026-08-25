grammar Expression;
s : e EOF | CARET EOF ;
e : e ('*'|'/') e | e ('+'|'-') e | ('+'|'-') e | INT ;
INT : [0-9]+ ;
CARET : [^] ;
WS : [ \t\n]+ -> skip ;
