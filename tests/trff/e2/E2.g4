grammar E2;

s : 'a' b d 'h' EOF;
b : 'c' c;
c : 'b' c | ;
d : e f;
e : 'g' | ;
f : 'f' | ;
WS: [ \t\n\r]+ -> channel(HIDDEN);
