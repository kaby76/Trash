grammar T3;
a : 'a'*;
b : 'b'+;
c : ('c' | 'd')*;
s : (a | b | c) EOF;