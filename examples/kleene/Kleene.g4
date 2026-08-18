grammar Kleene;
s : a ;
a : a ';' c d e | e ;
b : e ';' b | e ;
c : 'c';
d : 'd';
e : INT ;
INT : [0-9]+ ;
WS : [ \t\n]+ -> skip ;
