
grammar mumathParser;
program : ((functionDefinition|assignment|functionDesignator)
   (SEMI|DOLLAR) )* EOF ;

// empty : /* empty */ ;
assignment : (ID COLON )+ expression ;

list : LPAREN
       (RPAREN
       |ID
           (COMMAID )*
	   RPAREN ) ;

functionDefinition : FUNCTIONID list COMMA statments (COMMA)? ENDFUN ;

actualParameter : expression|assignment ;

statments : (loop|when|block|assignment|expression|functionDesignator)
   (COMMA statments)* ;

block : BLOCK statments COMMA ENDBLOCK ;
loop : LOOP
          statments (COMMA)? ENDLOOP;
when : WHEN
        expression
        ((COMMA)? EXIT
	| COMMA statments (COMMA )?
          EXIT ) ;
expression : simpleExpression ( relationalOperator simpleExpression)* ;

relationalOperator
    : equal | NOT_EQUAL | LT | LE |
       GE | GT | EQUATION
    ;

simpleExpression
    : (MINUS )?
      term ( addingOperator term )*
    ;

addingOperator
    : PLUS | MINUS | OR
    ;

term
    : factor ( multiplyingOperator factor )*
    ;

multiplyingOperator
    : STAR | SLASH | MOD | AND |
       POWER
    ;

factor
    :ID
    | constant
    | LPAREN expression RPAREN
    | functionDesignator
    | NOT factor
    ;

constant
    :NUMBER
    |STRING
    | QUOTEID
    | QUOTESTRING
    ;

functionDesignator
    :ID LPAREN
       ((actualParameter ( COMMA actualParameter) *)
       |)
       RPAREN ;

equal : (EQF|EQC); 
BLOCK : 'BLOCK';
 
ENDBLOCK : 'ENDBLOCK';
 
FUNCTION : 'FUNCTION';
 
ENDFUN : 'ENDFUN';
 
EQF : 'EQ';
 
LOOP : 'LOOP';
 
ENDLOOP : 'ENDLOOP';
 
WHEN : 'WHEN';
 
EXIT : 'EXIT';
 
OR : 'OR';


WS : (' ' | '\t' | '\n' | '\r') ;
COMMENT : '%' ('\n' | ~('%'|'\n'))* '%';


EQUATION	: '=='	;
QUOTE		: '\''	;
PLUS		: '+'	;
MINUS		: '-'	;
STAR		: '*'	;
SLASH		: '/'	;
COMMA		: ','	;
SEMI		: ';'	;
DOLLAR		: '$'	;
COLON		: ':'	;
EQC		: '='	;
NOT_EQUAL	: '<>'	;
LT		: '<'	;
LE		: '<='	;
GE		: '>='	;
GT		: '>'	;
LPAREN		: '('	;
RPAREN		: ')'	;
POWER		: '^'	;

ID :
  ('A'..'Z' | '@' | '{' | '#') ('A'..'Z'|'0'..'9'|'#'|'}')* (ARR)? ;
ARR: '[' NUMBER ']' ;
STRING : '"' (~'"')* '"';
NUMBER : ('0'..'9')+;