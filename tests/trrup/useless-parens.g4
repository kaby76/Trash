grammar x;
//y
a1 : ('a')?;

//n
a2 : ('a' 'b')?;

//n
a3 : ('c' | 'd')?;

//y
a4 : ('e' | 'f');

//y
a5 : ('a')? a;

//n
a6 : ('a' 'b')? a;

//n
a7 : ('c' | 'd')? a;

//n
a8 : ('e' | 'f') a;

//y
a9 : b ('a')?;

//n
aa : b ('a' 'b')?;

//n
ab : b ('c' | 'd')?;

//n
ac : b ('e' | 'f');
