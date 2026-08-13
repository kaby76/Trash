grammar Expression;
s : e EOF ;
e : e ('*'|'/') e | e ('+'|'-') e | ('+'|'-') e | INT ;
INT : [0-9]+ ;
WS : [ \t\n]+ -> skip ;
