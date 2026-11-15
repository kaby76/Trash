/* Lexer license header. */
lexer grammar CommentedLexer;

options { caseInsensitive = true; }

WORD : [a-z]+ ;
WS : [ \t\r\n]+ -> skip ;
