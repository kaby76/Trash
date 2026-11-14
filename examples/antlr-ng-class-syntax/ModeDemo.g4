lexer grammar ModeDemo;

Quote : '"' -> pushMode(StringMode);
Identifier : [a-z]+;

mode StringMode;
End : '"' -> popMode;
Text : ~["]+;
