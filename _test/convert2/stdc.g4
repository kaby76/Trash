

                     
grammar StdC;



translationUnit
        :       externalList

        |
        ;


externalList
        :       ( externalDef )+
        ;


externalDef
        : declaration
        |       functionDef
        |       asm_expr
        ;


asm_expr
        :       'asm' 
                ('volatile')? LCURLY expr RCURLY SEMI
        ;


declaration
        :declSpecifiers
                (                       
                    initDeclList
                )?
                SEMI
                
        ;


declSpecifiers
        :       (storageClassSpecifier
                | typeQualifier
                | typeSpecifier
                )+
        ;

storageClassSpecifier
        :       'auto'                  
        |       'register'              
        |       'typedef'               
        |       functionStorageClassSpecifier
        ;


functionStorageClassSpecifier
        :       'extern'
        |       'static'
        ;


typeQualifier
        :       'const'
        |       'volatile'
        ;

typeSpecifier
        :
        (       'void'
        |       'char'
        |       'short'
        |       'int'
        |       'long'
        |       'float'
        |       'double'
        |       'signed'
        |       'unsigned'
        |       structOrUnionSpecifier
        |       enumSpecifier
        |       { specCount == 0 }? typedefName
        )
        ;


typedefName
        :       { isTypedefName ( LT(1).getText() ) }?ID
        ;

structOrUnionSpecifier
        :structOrUnion
                (IDLCURLY
                        structDeclarationList
                        RCURLY
                |LCURLY
                    structDeclarationList
                    RCURLY
                | ID
                )
        ;


structOrUnion
        :       'struct'
        |       'union'
        ;


structDeclarationList
        :       ( structDeclaration )+
        ;


structDeclaration
        :       specifierQualifierList structDeclaratorList ( SEMI )+
        ;


specifierQualifierList
        :       ( typeSpecifier
                | typeQualifier
                )+
        ;


structDeclaratorList
        :       structDeclarator ( COMMA structDeclarator )*
        ;


structDeclarator
        :
        (       COLON constExpr
        |       declarator ( COLON constExpr )?
        )
        ;


enumSpecifier
        :       'enum'
                (ID LCURLY enumList RCURLY
                | LCURLY enumList RCURLY
                | ID
                )
        ;


enumList
        :       enumerator ( COMMA enumerator )*  
        ;

enumerator
        :ID
                (ASSIGN constExpr)?
        ;


initDeclList
        :       initDecl 
                ( COMMA initDecl )*
        ;


initDecl
        :declarator
                ( ASSIGN initializer
                | COLON expr
                )?

        ;

pointerGroup
        :       ( STAR ( typeQualifier )* )+
        ;



idList
        :       ID ( COMMA ID )*
        ;


initializer
        :       ( assignExpr
                |       LCURLY initializerList ( COMMA )? RCURLY
                )
        ;


initializerList
        :       initializer ( COMMA initializer )*
        ;


declarator
        :
                ( pointerGroup )?               

                (ID
                | LPAREN declarator RPAREN
                )

                ( !  LPAREN
                    (parameterTypeList

                        | (idList)?
                    )    
                  RPAREN                        
                | LBRACKET ( constExpr )? RBRACKET
                )*
        ;
 
parameterTypeList
        :       parameterDeclaration
                ( 
                  COMMA
                  parameterDeclaration
                )*
                ( COMMA
                  VARARGS
                )?
        ;


parameterDeclaration
        :declSpecifiers
                (declarator
                | nonemptyAbstractDeclarator
                )?
        ;

/* JTC:
 * This handles both new and old style functions.
 * see declarator rule to see differences in parameters
 * and here (declaration SEMI)* is the param type decls for the
 * old style.  may want to do some checking to check for illegal
 * combinations (but I assume all parsed code will be legal?)
 */

functionDef
        :       (functionDeclSpecifiers
                |  //epsilon
                )declarator
                ( declaration )* (VARARGS)? ( SEMI )*
                compoundStatement
        ;

functionDeclSpecifiers
        :       (
                  functionStorageClassSpecifier
                | typeQualifier
                | typeSpecifier
                )+
        ;

declarationList
        :       ( declaration
                )+
        ;

declarationPredictor
        :       (
                'typedef'
                | declaration
                )
        ;


compoundStatement
        :       LCURLY
                ( declarationList )?
                ( statementList )?
                RCURLY
        ;

    
statementList
        :       ( statement )+
        ;
statement
        :       SEMI                    // Empty statements

        |       compoundStatement       // Group of statements

        |       expr SEMI // Expressions

// Iteration statements:

        |       'while' LPAREN expr RPAREN statement
        |       'do' statement 'while' LPAREN expr RPAREN SEMI
        |!       'for'
                LPAREN (expr )? SEMI (expr )? SEMI (expr )? RPARENstatement


// Jump statements:

        |       'goto' ID SEMI
        |       'continue' SEMI
        |       'break' SEMI
        |       'return' ( expr )? SEMI


// Labeled statements:
        |       ID COLON (statement)?
        |       'case' constExpr COLON statement
        |       'default' COLON statement



// Selection statements:

        |       'if'
                 LPAREN expr RPAREN statement  
                (
                'else' statement )?
        |       'switch' LPAREN expr RPAREN statement
        ;






expr
        :       assignExpr (COMMA assignExpr         
                            )*
        ;


assignExpr
        :       conditionalExpr (assignOperator assignExpr )?
        ;

assignOperator
        :       ASSIGN
        |       DIV_ASSIGN
        |       PLUS_ASSIGN
        |       MINUS_ASSIGN
        |       STAR_ASSIGN
        |       MOD_ASSIGN
        |       RSHIFT_ASSIGN
        |       LSHIFT_ASSIGN
        |       BAND_ASSIGN
        |       BOR_ASSIGN
        |       BXOR_ASSIGN
        ;


conditionalExpr
        :       logicalOrExpr
                ( QUESTION expr COLON conditionalExpr )?
        ;


constExpr
        :       conditionalExpr
        ;

logicalOrExpr
        :       logicalAndExpr ( LOR logicalAndExpr )*
        ;


logicalAndExpr
        :       inclusiveOrExpr ( LAND inclusiveOrExpr )*
        ;

inclusiveOrExpr
        :       exclusiveOrExpr ( BOR exclusiveOrExpr )*
        ;


exclusiveOrExpr
        :       bitAndExpr ( BXOR bitAndExpr )*
        ;


bitAndExpr
        :       equalityExpr ( BAND equalityExpr )*
        ;



equalityExpr
        :       relationalExpr
                ( ( EQUAL | NOT_EQUAL ) relationalExpr )*
        ;


relationalExpr
        :       shiftExpr
                ( ( LT | LTE | GT | GTE ) shiftExpr )*
        ;



shiftExpr
        :       additiveExpr
                ( ( LSHIFT | RSHIFT ) additiveExpr )*
        ;


additiveExpr
        :       multExpr
                ( ( PLUS | MINUS ) multExpr )*
        ;


multExpr
        :       castExpr
                ( ( STAR | DIV | MOD ) castExpr )*
        ;


castExpr
        :
                LPAREN typeName RPAREN ( castExpr )

        |       unaryExpr
        ;


typeName
        :       specifierQualifierList (nonemptyAbstractDeclarator)?
        ;

nonemptyAbstractDeclarator
        :   (
                pointerGroup
                (   (LPAREN  
                    (   nonemptyAbstractDeclarator
                        | parameterTypeList
                    )?
                    RPAREN)
                | (LBRACKET (expr)? RBRACKET)
                )*

            |   (   (LPAREN  
                    (   nonemptyAbstractDeclarator
                        | parameterTypeList
                    )?
                    RPAREN)
                | (LBRACKET (expr)? RBRACKET)
                )+
            )
                                
        ;

/* JTC:

LR rules:

abstractDeclarator
        :       nonemptyAbstractDeclarator
        |       // null
        ;

nonemptyAbstractDeclarator
        :       LPAREN  nonemptyAbstractDeclarator RPAREN
        |       abstractDeclarator LPAREN RPAREN
        |       abstractDeclarator (LBRACKET (expr)? RBRACKET)
        |       STAR abstractDeclarator
        ;
*/

unaryExpr
        :       postfixExpr
        |       INC unaryExpr
        |       DEC unaryExpr
        |unaryOperator castExpr

        |       'sizeof'
                ( LPAREN typeName RPAREN
                | unaryExpr
                )
        ;


unaryOperator
        :       BAND
        |       STAR
        |       PLUS
        |       MINUS
        |       BNOT
        |       LNOT
        ;

postfixExpr
        :       primaryExpr
                ( 
                postfixSuffix 
                )?
        ;
postfixSuffix
        :
                ( PTR ID
                | DOT ID
                | functionCall
                | LBRACKET expr RBRACKET
                | INC
                | DEC
                )+
        ;

functionCall
        :
                LPAREN (argExprList)? RPAREN
        ;
    

primaryExpr
        :       ID
        |       charConst
        |       intConst
        |       floatConst
        |       stringConst

// JTC:
// ID should catch the enumerator
// leaving it in gives ambiguous err
//      | enumerator
        |       LPAREN expr RPAREN
        ;

argExprList
        :       assignExpr ( COMMA assignExpr )*
        ;
charConst
        :       CharLiteral
        ;
stringConst
        :       (StringLiteral)+
        ;
intConst
        :       IntOctalConst
        |       LongOctalConst
        |       UnsignedOctalConst
        |       IntIntConst
        |       LongIntConst
        |       UnsignedIntConst
        |       IntHexConst
        |       LongHexConst
        |       UnsignedHexConst
        ;
floatConst
        :       FloatDoubleConst
        |       DoubleDoubleConst
        |       LongDoubleConst
        ;




    

dummy
        :       NTypedefName
        |       NInitDecl
        |       NDeclarator
        |       NStructDeclarator
        |       NDeclaration
        |       NCast
        |       NPointerGroup
        |       NExpressionGroup
        |       NFunctionCallArgs
        |       NNonemptyAbstractDeclarator
        |       NInitializer
        |       NStatementExpr
        |       NEmptyExpression
        |       NParameterTypeList
        |       NFunctionDef
        |       NCompoundStatement
        |       NParameterDeclaration
        |       NCommaExpr
        |       NUnaryExpr
        |       NLabel
        |       NPostfixExpr
        |       NRangeExpr
        |       NStringSeq
        |       NInitializerElementLabel
        |       NLcurlyInitializer
        |       NAsmAttribute
        |       NGnuAsmExpr
        |       NTypeMissing
        ;
Vocabulary
        :       '\3'..'\377'
        ;


/* Operators: */

ASSIGN          : '=' ;
COLON           : ':' ;
COMMA           : ',' ;
QUESTION        : '?' ;
SEMI            : ';' ;
PTR             : '->' ;
DOT:;
VARARGS:;


LPAREN          : '(' ;
RPAREN          : ')' ;
LBRACKET        : '[' ;
RBRACKET        : ']' ;
LCURLY          : '{' ;
RCURLY          : '}' ;

EQUAL           : '==' ;
NOT_EQUAL       : '!=' ;
LTE             : '<=' ;
LT              : '<' ;
GTE             : '>=' ;
GT              : '>' ;

DIV             : '/' ;
DIV_ASSIGN      : '/=' ;
PLUS            : '+' ;
PLUS_ASSIGN     : '+=' ;
INC             : '++' ;
MINUS           : '-' ;
MINUS_ASSIGN    : '-=' ;
DEC             : '--' ;
STAR            : '*' ;
STAR_ASSIGN     : '*=' ;
MOD             : '%' ;
MOD_ASSIGN      : '%=' ;
RSHIFT          : '>>' ;
RSHIFT_ASSIGN   : '>>=' ;
LSHIFT          : '<<' ;
LSHIFT_ASSIGN   : '<<=' ;

LAND            : '&&' ;
LNOT            : '!' ;
LOR             : '||' ;

BAND            : '&' ;
BAND_ASSIGN     : '&=' ;
BNOT            : '~' ;
BOR             : '|' ;
BOR_ASSIGN      : '|=' ;
BXOR            : '^' ;
BXOR_ASSIGN     : '^=' ;


Whitespace
        :       ( ( '\003'..'\010' | '\t' | '\013' | '\f' | '\016'.. '\037' | '\177'..'\377' | ' ' )
                | '\r\n'
                | ( '\n' | '\r' )
                )
        ;


Comment
        :       '/*'
                ( { LA(2) != '/' }? '*'
                | '\r\n'
                | ( '\r' | '\n' )
                | ~( '*'| '\r' | '\n' )
                )*
                '*/'
        ;


CPPComment
        :
                '//' ( ~('\n') )*
        ;

PREPROC_DIRECTIVE

        :
        '#'
        ( LineDirective      
            | (~'\n')*
        )
        ;  Space:
        ( ' ' | '\t' | '\014')
        ; LineDirective
:
        ('line')?  //this would be for if the directive started "#line", but not there for GNU directives
        (Space)+Number 
        (Space)+
        (StringLiteral
                |ID
        )?
        (Space)*
        ('1' )?
        (Space)*
        ('2' )?
        (Space)*
        ('3' )?
        (Space)*
        ('4' )?
        (~('\r' | '\n'))*
        ('\r\n' | '\r' | '\n')
        ;



/* Literals: */

/*
 * Note that we do NOT handle tri-graphs nor multi-byte sequences.
 */


/*
 * Note that we can't have empty character constants (even though we
 * can have empty strings :-).
 */
CharLiteral
        :       '\'' ( Escape | ~( '\'' ) ) '\''
        ;


/*
 * Can't have raw imbedded newlines in string constants.  Strict reading of
 * the standard gives odd dichotomy between newlines & carriage returns.
 * Go figure.
 */
StringLiteral
        :       '"'
                ( Escape
                | ( 
                    '\r'
                  | '\n'
                  | '\\' '\n'
                  )
                | ~( '"' | '\r' | '\n' | '\\' )
                )*
                '"'
        ; BadStringLiteral
        :       // Imaginary token.
        ;
Escape  
        :       '\\'
                (
                  'a'
                | 'b'
                | 'f'
                | 'n'
                | 'r'
                | 't'
                | 'v'
                | '"'
                | '\''
                | '\\'
                | '?'
                | ('0'..'3') ( Digit ( Digit )? )?
                | ('4'..'7') ( Digit )?
                | 'x' ( Digit | 'a'..'f' | 'A'..'F' )+
                )
        ;
Digit
        :       '0'..'9'
        ;
LongSuffix
        :       'l'
        |       'L'
        ;
UnsignedSuffix
        :       'u'
        |       'U'
        ;
FloatSuffix
        :       'f'
        |       'F'
        ;
Exponent
        :       ( 'e' | 'E' ) ( '+' | '-' )? ( Digit )+
        ;
DoubleDoubleConst:;
FloatDoubleConst:;
LongDoubleConst:;
IntOctalConst:;
LongOctalConst:;
UnsignedOctalConst:;
IntIntConst:;
LongIntConst:;
UnsignedIntConst:;
IntHexConst:;
LongHexConst:;
UnsignedHexConst:;




Number
        : ( Digit )+
                ( '.' ( Digit )* ( Exponent )?
                | Exponent
                )
                ( FloatSuffix
                | LongSuffix
                )?

        | '...'

        |       '.'
                ( ( Digit )+ ( Exponent )?
                  ( FloatSuffix
                  | LongSuffix
                  )?
                )?

        |       '0' ( '0'..'7' )*
                ( LongSuffix
                | UnsignedSuffix
                )?

        |       '1'..'9' ( Digit )*
                ( LongSuffix
                | UnsignedSuffix
                )?

        |       '0' ( 'x' | 'X' ) ( 'a'..'f' | 'A'..'F' | Digit )+
                ( LongSuffix
                | UnsignedSuffix
                )?
        ;


ID
        :       ( 'a'..'z' | 'A'..'Z' | '_' )
                ( 'a'..'z' | 'A'..'Z' | '_' | '0'..'9' )*
        ;

