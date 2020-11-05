grammar Literals;

p : Hello World ;
Hello : 'hello';
World : 'world';
Foo : 'w_kdf';
WS : (' ' | '\n' | '\r')+ -> skip;
