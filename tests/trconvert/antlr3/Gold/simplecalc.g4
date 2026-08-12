grammar SimpleCalc;
 
tokens {
    PLUS      ,
    MINUS     ,
    MULT      ,
    DIV   
}

// Token string literals converted to explicit lexer rules.
// Reorder these rules accordingly.

PLUS: '+';
MINUS: '-';
MULT: '*';
DIV: '/';
//


 
/*------------------------------------------------------------------
 * PARSER RULES
 *------------------------------------------------------------------*/
 
expr    : term ( ( PLUS | MINUS )  term )* ;
 
term    : factor ( ( MULT | DIV ) factor )* ;
 
factor  : NUMBER ;
 
 
/*------------------------------------------------------------------
 * LEXER RULES
 *------------------------------------------------------------------*/
 
NUMBER  : (DIGIT)+ ;
 
WHITESPACE : ( '\t' | ' ' | '\r' | '\n'| '\u000C' )+ ;
 
fragment DIGIT  : '0'..'9' ;

