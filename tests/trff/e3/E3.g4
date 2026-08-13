grammar E3;

ss : s EOF;
s : a c b | c 'b' 'b' | b 'a';
a : 'd' 'a' | b c;
b : 'g' | ;
c : 'h' | ;
