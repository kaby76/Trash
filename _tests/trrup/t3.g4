grammar t3;
a : ((a));
b : ((a b)) c;
c : (a b)* | c;
d : ((a b) | c);
