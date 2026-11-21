grammar CongoCC;

// Parse the outer CongoCC syntax and retain target-language code as blocks.
grammarFile : (NEWLINE | grammarElement)* EOF ;
grammarElement
    : setting
    | inclusion
    | injection
    | tokenProduction
    | production
    ;

setting : identifier (EQUALS settingPart*)? SEMI ;
inclusion : INCLUDE location (BANG location)* SEMI? ;
location : STRING | identifier ;

tokenProduction : (lexicalStates NEWLINE*)? tokenKind headerPart* COLON productionBody SEMI ;
lexicalStates
    : LT NEWLINE* (STAR | identifier (COMMA NEWLINE* identifier)*)
      NEWLINE* GT
    ;
tokenKind : TOKEN | REGULAR_TOKEN | UNPARSED | SPECIAL_TOKEN
          | SKIP_ | MORE_ | CONTEXTUAL ;
production : headerPart+ COLON productionBody SEMI ;

// INJECT can contain imports and a code block, raw code, or an
// extends/implements clause with no code block.
injection
    : INJECT headerPart* COLON NEWLINE* injectionClause* codeBlock
    | INJECT headerPart* COLON injectionClause NEWLINE
    | INJECT headerPart* codeBlock
    ;
injectionClause
    : (IMPORT | EXTENDS | IMPLEMENTS) injectionPart* SEMI? NEWLINE*
    | AT identifier parenthesized? NEWLINE*
    ;
injectionPart
    : parenthesized
    | bracketed
    | ~(SEMI | NEWLINE | LBRACE | RBRACE | LPAREN | LBRACK)
    ;
headerPart
    : parenthesized
    | bracketed
    | ~(COLON | SEMI | NEWLINE | LBRACE | RBRACE | LPAREN | LBRACK)
    ;
settingPart
    : parenthesized
    | bracketed
    | ~(SEMI | LPAREN | LBRACK)
    ;
productionBody : bodyPart* ;
bodyPart
    : codeBlock
    | parenthesized
    | bracketed
    | ~(SEMI | LBRACE | LPAREN | LBRACK)
    ;
codeBlock : LBRACE (codeBlock | ~(LBRACE | RBRACE))* RBRACE ;
parenthesized
    : LPAREN (codeBlock | parenthesized | bracketed
             | ~(LPAREN | RPAREN | LBRACE | LBRACK))* RPAREN
    ;
bracketed
    : LBRACK (codeBlock | parenthesized | bracketed
             | ~(LBRACK | RBRACK | LBRACE | LPAREN))* RBRACK
    ;
identifier
    : IDENTIFIER | TOKEN | REGULAR_TOKEN | UNPARSED | SPECIAL_TOKEN
    | SKIP_ | MORE_ | CONTEXTUAL | IMPORT | EXTENDS | IMPLEMENTS
    ;

INCLUDE       : 'INCLUDE';
INJECT        : 'INJECT';
TOKEN         : 'TOKEN';
REGULAR_TOKEN : 'REGULAR_TOKEN';
UNPARSED      : 'UNPARSED';
SPECIAL_TOKEN : 'SPECIAL_TOKEN';
SKIP_         : 'SKIP';
MORE_         : 'MORE';
CONTEXTUAL    : 'CONTEXTUAL';
IMPORT        : 'import';
EXTENDS       : 'extends';
IMPLEMENTS    : 'implements';
COLON : ':';
SEMI : ';';
EQUALS : '=';
BANG : '!';
AT : '@';
COMMA : ',';
LT : '<';
GT : '>';
STAR : '*';
LBRACE : '{';
RBRACE : '}';
LPAREN : '(';
RPAREN : ')';
LBRACK : '[';
RBRACK : ']';
TRIPLE_STRING : '"""' .*? '"""' ;
STRING
    : '"' ('\\' . | ~["\\])* '"'
    | '\'' ('\\' . | ~['\\])* '\''
    ;
IDENTIFIER : [a-zA-Z_] [a-zA-Z_0-9]*;
NUMBER : [0-9]+;
// CongoCC's shipped grammars use #if false to hold non-grammar source code.
INACTIVE_BLOCK
    : '#if' [ \t]+ ('false' | '__YouAreInterested__')
      ~[\r\n]* .*? '#endif' -> skip
    ;
PREPROCESSOR
    : '#' ('if' | 'elif' | 'else' | 'endif' | 'define' | 'undef')
      ~[\r\n]* -> skip
    ;
LINE_COMMENT : '//' ~[\r\n]* -> skip;
BLOCK_COMMENT : '/*' .*? '*/' -> skip;
NEWLINE : '\r'? '\n' | '\r';
WS : [ \t\f]+ -> skip;
OTHER : .;
