grammar g1;
a : 'a';
b : 'b' | a;
c : b | a b ;
d : (b | c) a;
