lexer grammar XPath3MetaLexer;

channels {
    OFF_CHANNEL
}

CCEQ: '::=';
Q: '?';
ALT: '|';
M: '-';
P: '+';
S: '*';
OP: '(';
CP: ')';

SET: '[' ~'['* ']';
STRING: '"' ~'"'* '"' | '\'' ~'\''* '\'';
CONSTRAINT: '/*' [ \t\n]+ ('xgc' | 'ws' | 'gn') ':' .*? '*/';
COMMENT: '/*' .*? '*/' -> channel(HIDDEN);
SYMBOL: Symbol;
WS: Ws -> channel(OFF_CHANNEL);

fragment Symbol: [a-zA-Z0-9_.\-] [a-zA-Z0-9_.\-]*;
fragment Ws: [ \t\r\n]+;
fragment Url:
    ~[\u005D:/?#]+ '://' ~[\u005D#]+ ('#' [a-zA-Z_%0-9.]+)?
;