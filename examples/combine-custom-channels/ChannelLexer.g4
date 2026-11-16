lexer grammar ChannelLexer;

channels { SPECIAL }

WORD : [a-z]+ ;
WS : [ \t\r\n]+ -> channel(SPECIAL) ;
