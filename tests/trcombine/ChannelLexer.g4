lexer grammar ChannelLexer;

channels { SPECIAL }

WORD : [a-z]+ -> channel(SPECIAL) ;
