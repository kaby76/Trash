grammar tst;

root: expression EOF;

finAmount:
    '$' currencyExpression? expression ('\'' expression)?
;

expression:
    finAmount
    | expression op = (MUL | DIV) expression
    | expression ADD expression
    | expression SUB expression
    | expression op = (
        BIGGER
        | BIGGER_OR_EQUAL
        | SMALLER
        | SMALLER_OR_EQUAL
    ) expression
    | expression op = (EQUAL | NOT_EQUAL) expression
    | expression op = (AND | OR) expression
    | numberLiteral
    | stringLiteral
    | trueLiteral
    | falseLiteral
    | '(' expression ')'
;

trueLiteral: TRUE;

falseLiteral: FALSE;

numberLiteral: floatLiteral | wholeLiteral;

floatLiteral: FLOATNUM;

wholeLiteral: WHOLENUM;

stringLiteral: STRING;

currencyExpression: '(' expression ')' | CURRENCY_LIT;

//---------------------------------------------------------------------------------
//-- Lexical Rules
//---------------------------------------------------------------------------------
STRING: '"' .*? '"';

AND: 'and';
OR: 'or';

NOT: 'not';

TRUE: 'true';
FALSE: 'false';

FLOATNUM: '-'? DIGIT+ '.' DIGIT+;

WHOLENUM: '-'? DIGIT+;

NULL: 'null';

MUL: '*'; // assigns token name to '*' used above in grammar
DIV: '/';
ADD: '+';
SUB: '-';

BIGGER: '>';
BIGGER_OR_EQUAL: '>=';
SMALLER: '<';
SMALLER_OR_EQUAL: '<=';
EQUAL: '==';
NOT_EQUAL: '!=';

EQUALASSIGN: '=';

CURRENCY_LIT:
    '(' [A-Z][A-Z][A-Z] ')'
    | '(' [0-9][0-9][0-9] ')'
;

IDENTIFIER: IDENTIFIERSTART IDENTIFIERPART*;

WS: [ \t\r\n]+ -> channel(HIDDEN); // toss out whitespace

fragment DIGIT: [0-9]; // match single digit      ;

fragment IDENTIFIERSTART: [a-zA-Z];

fragment IDENTIFIERPART: [a-zA-Z0-9_];