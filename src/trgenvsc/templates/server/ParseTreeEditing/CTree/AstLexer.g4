lexer grammar AstLexer;
channels { COMMENTS_CHANNEL }

SINGLE_LINE_DOC_COMMENT: '///' InputCharacter*    -> channel(COMMENTS_CHANNEL);
DELIMITED_DOC_COMMENT:   '/**' .*? '*/'           -> channel(COMMENTS_CHANNEL);
SINGLE_LINE_COMMENT:     '//'  InputCharacter*    -> channel(COMMENTS_CHANNEL);
DELIMITED_COMMENT:       '/*'  .*? '*/'           -> channel(COMMENTS_CHANNEL);

OPEN_PAREN      :       '(';
CLOSE_PAREN     :       ')';
OPEN_CURLY      :       '{';
CLOSE_CURLY     :       '}';
EQUALS          :       '=';
StringLiteral   :       '"' ( Escape | ~('"' | '\n' | '\r') )* '"';
ID              :       [a-zA-Z_1234567890.]+ ;

fragment InputCharacter:       ~[\r\n\u0085\u2028\u2029];
fragment Escape : '\\' '"';
WS:    [ \t\r\n] -> skip;
