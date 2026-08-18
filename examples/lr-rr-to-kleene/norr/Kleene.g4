grammar Kleene;
s : a ;
a : a ';' e | e ;
b :( e ';' )* ( e ) ;
e : INT ;
INT : [0-9]+ ;
WS : [ \t\n]+ -> skip ;
