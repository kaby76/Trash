grammar simple;
hello: 'hello' 'world' ;
WS: [ \t\n\r]+ -> channel(HIDDEN);
