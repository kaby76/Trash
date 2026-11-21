grammar Words;

document : wordGroup EOF;
wordGroup : WORD COMMA WORD COMMA WORD
 | WORD COMMA WORD
 ;
WORD : [a-zA-Z]+;
COMMA : ',';
WS : [ \t\r\n]+ -> skip;
