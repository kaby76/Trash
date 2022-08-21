// Derived from https://www.python.org/dev/peps/pep-0617/
// Tokens assumed to be derived from https://raw.githubusercontent.com/python/cpython/3.10/Grammar/Tokens
// Ken Domino, 2 Sep 2021

lexer grammar pegen_v3_10Lexer;

options { superClass = Adaptor; }

MEMO: 'memo';

//ENDMARKER : '\n';
NAME: LETTER('_'|(DIGIT|LETTER))*;
fragment LETTER  : CAPITAL | SMALL ;
fragment CAPITAL : [A-Z\u00C0-\u00D6\u00D8-\u00DE] ;
fragment SMALL   : [a-z\u00DF-\u00F6\u00F8-\u00FF] ;
fragment DIGIT   : [0-9] ;
NUMBER : DIGIT+;
STRING : '"' -> more, mode(STRINGMODE);
CHAR : '\''   -> more, mode(CHARMODE);
//NEWLINE : '\r\n';
//INDENT
//DEDENT

LPAR :                  '(';
RPAR :                  ')';
LSQB :                  '[';
RSQB :                  ']';
COLON :                 ':';
COMMA :                 ',';
SEMI :                  ';';
PLUS :                  '+';
MINUS :                 '-';
STAR :                  '*';
SLASH :                 '/';
VBAR :                  '|';
AMPER :                 '&';
LESS :                  '<';
GREATER :               '>';
EQUAL :                 '=';
DOT :                   '.';
PERCENT :               '%';
LBRACE :                '{'; // -> pushMode(ACTION) ;
RBRACE :                '}';
EQEQUAL :               '==';
NOTEQUAL :              '!=';
LESSEQUAL :             '<=';
GREATEREQUAL :          '>=';
TILDE :                 '~';
CIRCUMFLEX :            '^';
LEFTSHIFT :             '<<';
RIGHTSHIFT :            '>>';
DOUBLESTAR :            '**';
PLUSEQUAL :             '+=';
MINEQUAL :              '-=';
STAREQUAL :             '*=';
SLASHEQUAL :            '/=';
PERCENTEQUAL :          '%=';
AMPEREQUAL :            '&=';
VBAREQUAL :             '|=';
CIRCUMFLEXEQUAL :       '^=';
LEFTSHIFTEQUAL :        '<<=';
RIGHTSHIFTEQUAL :       '>>=';
DOUBLESTAREQUAL :       '**=';
DOUBLESLASH :           '//';
DOUBLESLASHEQUAL :      '//=';
AT :                    '@';
ATEQUAL :               '@=';
RARROW :                '->';
ELLIPSIS :              '...';
COLONEQUAL :            ':=';
DOLLAR: '$';
BANG: '!';
QUESTION: '?';


//OP
//AWAIT
//ASYNC
//TYPE_IGNORE
//TYPE_COMMENT
//SOFT_KEYWORD
//ERRORTOKEN

// These aren't used by the C tokenizer but are needed for tokenize.py
COMMENT : '#' ~[\n\r]* -> channel(HIDDEN) ;
//NL
//ENCODING

WS_INLINE: Space+ -> channel(HIDDEN) ;
fragment Space : (' '| '\t' | '\n' | '\r' | '\f' | 'u2B7F' );

ErrorToken : . ;


// Escapable sequences
fragment
Escapable : ('"' | '\\' | 'n' | 't' | 'r' | 'f' | '\n' | '\r');

mode STRESCAPE;
STRESCAPED : Escapable  -> more, popMode ;

mode STRINGMODE;
STRINGESC : '\\' -> more , pushMode(STRESCAPE);
STRINGEND : '"' ->  type(STRING), mode(DEFAULT_MODE);
STRINGTEXT : (~["\\] | '""') -> more;

mode CHARESCAPE;
CHARESCAPED : Escapable  -> more, popMode ;

mode CHARMODE;
CHARESC : '\\' -> more , pushMode(CHARESCAPE);
CHAREND : '\'' ->  type(STRING), mode(DEFAULT_MODE);
CHARTEXT : ~['\\] -> more;

mode ACTION;
NESTED_ACTION : LBRACE -> type (ACTION_CONTENT) , pushMode (ACTION) ;
ACTION_ESCAPE : EscAny -> type (ACTION_CONTENT) ;
//ACTION_STRING_LITERAL : DQuoteLiteral -> type (ACTION_CONTENT) ;
//ACTION_CHAR_LITERAL : SQuoteLiteral -> type (ACTION_CONTENT) ;
//ACTION_DOC_COMMENT : DocComment -> type (ACTION_CONTENT) ;
//ACTION_BLOCK_COMMENT : BlockComment -> type (ACTION_CONTENT) ;
//ACTION_LINE_COMMENT : LineComment -> type (ACTION_CONTENT) ;
END_ACTION : RBRACE { this.handleEndAction(); } ;
UNTERMINATED_ACTION : EOF -> popMode ;
ACTION_CONTENT : . ;
fragment EscAny : Esc . ;
fragment Esc : '\\' ;

