grammar t4;
xx : 'a' xx | 'a';
yy : yy 'b' | 'b' ;
zz : | 'a' | 'a' zz;
z2 : | 'b' | z2 'b';
