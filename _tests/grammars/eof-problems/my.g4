grammar my; 
st: 'H' hd | EOF ;
hd: 'D' d | 'C' c | st ;
d: hd ;
c: 'D' c | hd ;
s1: 'D' s1 | c ;
// p: hd ;
SKP: [ \t\r\n]+ -> skip;