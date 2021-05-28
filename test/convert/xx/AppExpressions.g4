grammar AppExpressions;

//tokens { INTEGER_LITERAL, REAL_LITERAL, DOT }

//kalacak
expr : expression EOF;
//kalacak
exprList 
    : LPAREN expression (SEMI expression)+ RPAREN
    ;
//bakacaz
expression	:	logicalOrExpression 
				(
					(ASSIGN  logicalOrExpression) 
				|   (DEFAULT  logicalOrExpression) 
				|	(QMARK expression COLON expression)
				)?
			;
//kalacak
parenExpr
    : LPAREN expression RPAREN;
//kalacak
logicalOrExpression : logicalAndExpression (OR logicalAndExpression)* ;
//kalacak               
logicalAndExpression : relationalExpression (AND  relationalExpression)* ;                        
//bakacaz
relationalExpression
    :sumExpr 
          (relationalOperator sumExpr 
          )?
    ;
//kalacak
sumExpr  : prodExpr (
                        (PLUS  
                        | MINUS ) prodExpr)* ; 
//kalacak
prodExpr : powExpr (
                        (STAR  
                        | DIV  
                        | MOD ) powExpr)* ;

//kalacak
powExpr  : unaryExpression (POWER  unaryExpression)? ;
//kalacak
unaryExpression 
	:	(PLUS 
	    | MINUS 
	    | BANG ) unaryExpression	
	|	primaryExpression
	;
//kalacak
unaryOperator
	: PLUS | MINUS | BANG
    ;
	
//bakacaz
primaryExpression : startNode (node)?;

startNode 
    : 
    ( exprList 
    |   parenExpr
	|	property
	|	var
	| 	indexer
    |   literal 
    |   constructor
	|   listInitializer
    |   mapInitializer
    )
    ;

node : 
    ( property  
	| indexer
    | exprList
    |	DOT 
    )+
    ;

var : POUND ID ;

property
    :  ID 
    ;
indexer
	:	
      LBRACKET  argument (COMMA argument)* RBRACKET
	;
//kalacak
constructor
	: 'new' qualifiedId ctorArgs
	|   arrayConstructor
	;
//kalacak
arrayConstructor
	:	 'new' qualifiedId arrayRank (listInitializer)?
	;
//kalacak
arrayRank
    :   LBRACKET (expression (COMMA expression)*)? RBRACKET
    ;
//kalacak
listInitializer
    :   LCURLY  expression (COMMA expression)* RCURLY
    ;
//kalacak
mapInitializer
    :   POUND LCURLY  mapEntry (COMMA mapEntry)* RCURLY
    ;
//kalacak
mapEntry
    :   expression COLON expression
    ;
 //kalacak    
ctorArgs : LPAREN (namedArgument (COMMA namedArgument)*)? RPAREN;
  //kalacak          
argument : expression;
  //kalacak 
namedArgument 
    : ID ASSIGN expression 
    |   argument 
    ;

//kalacak
qualifiedId : ID  (DOT ID)*
    ;
  
//kalacak
literal
	:	NULL_LITERAL 
	|   INTEGER_LITERAL 
	|   HEXADECIMAL_INTEGER_LITERAL 
	|   REAL_LITERAL
	|	STRING_LITERAL 
	|   boolLiteral
	|   dateLiteral
	;
//kalacak
boolLiteral
    :   TRUE 
    |   FALSE 
    ;
//kalacak
dateLiteral
    :   'date' 
            LPAREN STRING_LITERAL (COMMA STRING_LITERAL)? RPAREN
    ;
//kalacak
relationalOperator
    :   EQUAL 
    |   NOT_EQUAL
    |   LESS_THAN
    |   LESS_THAN_OR_EQUAL      
    |   GREATER_THAN            
    |   GREATER_THAN_OR_EQUAL 
    |   IN   
    |   IS   
    |   BETWEEN   
    |   LIKE   
    |   MATCHES   
	|	FOREACH
    ; 
    

FALSE : 'false';
TRUE : 'true';
AND : 'and';
OR : 'or';
IN : 'in';
IS : 'is';
BETWEEN : 'between';
LIKE : 'like';
MATCHES : 'matches';
FOREACH : 'foreach';
NULL_LITERAL : 'null';


WS	:	(' '
	|	'\t'
	|	'\n'
	|	'\r')
	;


AT: '@'
  ;

PIPE: '|'
  ;

BANG: '!'
  ;

QMARK: '?'
  ;

DOLLAR: '$'
  ;

POUND: '#'
  ;
    
LPAREN:	'('
	;

RPAREN:	')'
	;

LBRACKET:	'['
	;

RBRACKET:	']'
	;

LCURLY:	'{'
	;

RCURLY:	'}'
	;

COMMA : ','
   ;

SEMI: ';'
  ;

COLON: ':'
  ;

ASSIGN: '='
  ;

DEFAULT: '??'
  ;
  
PLUS: '+'
  ;

MINUS: '-'
  ;
   
DIV: '/'
  ;

STAR: '*'
  ;

MOD: '%'
  ;

POWER: '^'
  ;
  
EQUAL: '=='
  ;

NOT_EQUAL: '!='
  ;

LESS_THAN: '<'
  ;

LESS_THAN_OR_EQUAL: '<='
  ;

GREATER_THAN: '>'
  ;

GREATER_THAN_OR_EQUAL: '>='
  ;
  
PROJECT: '!{'
  ;
  
SELECT: '?{'
  ;

SELECT_FIRST: '^{'
  ;
  
SELECT_LAST: '${'
  ;

TYPE: 'T('
  ;

LAMBDA: '{|'
  ;

DOT_ESCAPED: '\\.'
  ;
  
STRING_LITERAL
	:	'\'' (APOS|~'\'')* '\''
	;

APOS : '\'' '\''
    ;
  
ID
	:	('a'..'z'|'A'..'Z'|'_') ('a'..'z'|'A'..'Z'|'_'|'0'..'9'|DOT_ESCAPED)*
	;


	// real
REAL_LITERAL	:
	'.' (DECIMAL_DIGIT)+ (EXPONENT_PART)? (REAL_TYPE_SUFFIX)?
	| (DECIMAL_DIGIT)+ '.' (DECIMAL_DIGIT)+ (EXPONENT_PART)? (REAL_TYPE_SUFFIX)?
	| (DECIMAL_DIGIT)+ (EXPONENT_PART) (REAL_TYPE_SUFFIX)?
	| (DECIMAL_DIGIT)+ (REAL_TYPE_SUFFIX)
	;

// integer
INTEGER_LITERAL :
	(DECIMAL_DIGIT)+ (INTEGER_TYPE_SUFFIX)?
	;

DOT:
	'.'
	;

	
HEXADECIMAL_INTEGER_LITERAL
	:	'0x'   (HEX_DIGIT)+   (INTEGER_TYPE_SUFFIX)?
	;

DECIMAL_DIGIT
	: 	'0'..'9'
	;

INTEGER_TYPE_SUFFIX
	:
	(
		'UL'	| 'LU' 	| 'ul'	| 'lu'
		|	'UL'	| 'LU' 	| 'uL'	| 'lU'
		|	'U'		| 'L'	| 'u'	| 'l'
	)
	;
		
HEX_DIGIT
	:	'0' | '1' | '2' | '3' | '4' | '5' | '6' | '7' | '8' | '9' | 
		'A' | 'B' | 'C' | 'D' | 'E' | 'F'  |
		'a' | 'b' | 'c' | 'd' | 'e' | 'f'
	;	
	
EXPONENT_PART
	:	'e'  (SIGN)*  (DECIMAL_DIGIT)+
	|	'E'  (SIGN)*  (DECIMAL_DIGIT)+
	;	
	

SIGN
	:	'+' | '-'
	;
	

REAL_TYPE_SUFFIX
	: 'F' | 'f' | 'D' | 'd' | 'M' | 'm'
	;
