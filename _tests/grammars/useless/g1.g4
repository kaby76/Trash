grammar g1;
a : ('a');
b : ( 'b' ) | a;
c : b | (a);
d : (b | c);
