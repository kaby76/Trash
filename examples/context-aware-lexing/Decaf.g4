grammar Decaf;

program
    : CLASS ID LCURLY field_decl* RCURLY EOF
    ;

field_decl
    : INT ID LSQUARE INT_LITERAL RSQUARE SEMI
    ;

CLASS: 'class';
INT: 'int';
LCURLY: '{';
RCURLY: '}';
LSQUARE: '[';
RSQUARE: ']';
SEMI: ';';

fragment DIGIT: [0-9];

// These two token rules deliberately overlap. With ordinary ANTLR priority,
// DECIMAL_LITERAL always wins an equal-length match because it appears first.
DECIMAL_LITERAL: DIGIT+;
INT_LITERAL: DECIMAL_LITERAL;

ID: [a-zA-Z_] [a-zA-Z_0-9]*;
WS: [ \t\r\n]+ -> skip;
