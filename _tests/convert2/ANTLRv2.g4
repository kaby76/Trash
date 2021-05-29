

/* ANTLR Translator Generator
 * Project led by Terence Parr at http://www.cs.usfca.edu
 * Software rights: http://www.antlr.org/license.html
 *
 * $Id: //depot/code/org.antlr/release/antlr-2.7.7/antlr/antlr.g#2 $
 */

grammar ANTLR;

grammar_
   :
        (
                'header'
                (STRING_LITERAL)?ACTION
        )*
        ( fileOptionsSpec )?
        ( classDef )*
        EOF
        ;

classDef
        :
        (ACTION )?
        (DOC_COMMENT )?
        ( lexerSpec
        | treeParserSpec
        |       parserSpec
        )
        rules
        ;

fileOptionsSpec
        :       OPTIONS
                ( id
                        ASSIGN optionValue
                        SEMI
                )*
                RCURLY
        ;

parserOptionsSpec
        :       OPTIONS
                ( id
                        ASSIGN optionValue
                        SEMI
                )*
                RCURLY
        ;

treeParserOptionsSpec
        :       OPTIONS
                ( id
                        ASSIGN optionValue
                        SEMI
                )*
                RCURLY
        ;

lexerOptionsSpec
        :
        OPTIONS
        (       // Special case for vocabulary option because it has a bit-set
                'charVocabulary'
                ASSIGN charSet
                SEMI

        | id
                ASSIGN optionValue
                SEMI
        )*
        RCURLY
        ;

subruleOptionsSpec
        :       OPTIONS
                ( id
                        ASSIGN optionValue
                        SEMI
                )*
                RCURLY
        ;

// optionValue returns a Token which may be one of several things:
//    STRING_LITERAL -- a quoted string
//    CHAR_LITERAL -- a single quoted character
//              INT -- an integer
//              RULE_REF or TOKEN_REF -- an identifier
optionValue
        : qualifiedID
        |STRING_LITERAL
        |CHAR_LITERAL
        |INT
        ;

charSet
        : setBlockElement
                (
                        OR setBlockElement
                )*
        ;

setBlockElement
        :CHAR_LITERAL
        (
                RANGECHAR_LITERAL
        )?
        ;

tokensSpec
        :       TOKENS
                        (       (TOKEN_REF
                                        ( ASSIGNSTRING_LITERAL )?
                                        (tokensSpecOptions)?
                                |STRING_LITERAL
                                        (tokensSpecOptions)?
                                )
                                SEMI
                        )+
                RCURLY
        ;

tokensSpecOptions
        :       OPEN_ELEMENT_OPTIONid ASSIGNoptionValue
                (
                        SEMIid ASSIGNoptionValue
                )*
                CLOSE_ELEMENT_OPTION
        ;

superClass
        :       LPAREN
                        (STRING_LITERAL)
                RPAREN
        ;

parserSpec
        :       'class' id
                (       'extends' 'Parser'
                        (superClass)?
                |
                )
                SEMI
                (parserOptionsSpec)?
                (tokensSpec)?
                (ACTION )?
        ;

lexerSpec
        :   ('lexclass' id
                |       'class' id
                        'extends'
                        'Lexer'
                        (superClass)?
                )
                SEMI
                (lexerOptionsSpec)?
                (tokensSpec)?
                (ACTION )?
        ;

treeParserSpec
        :       'class' id
                'extends'
                'TreeParser'
                (superClass)?
                SEMI
                (treeParserOptionsSpec)?
                (tokensSpec)?
                (ACTION )?
        ;

rules
    :   (       rule
                )+
    ;

rule
        :
        (DOC_COMMENT
        )?
        ('protected'
        |'public'
        |'private'
        )? id
        ( BANG )?
        (ARG_ACTION  )?
        ( 'returns'ARG_ACTION )?
        ( throwsSpec )?
        ( ruleOptionsSpec )?
        (ACTION)?
        COLON block SEMI
        ( exceptionGroup )?
        ;
/*
        //
        // for now, syntax error in rule aborts the whole grammar
        //
        exception catch [ParserException ex] {
                behavior.abortRule(idTok);
                behavior.hasError();
                // Consume until something that looks like end of a rule
                consume();
                while (LA(1) != SEMI && LA(1) != EOF) {
                        consume();
                }
                consume();
        }
*/

ruleOptionsSpec
        :       OPTIONS
                ( id
                        ASSIGN optionValue
                        SEMI
                )*
                RCURLY
        ;

throwsSpec
        :       'throws'id
                ( COMMAid )*
        ;

block
    :
                alternative ( OR alternative )*
    ;

alternative
    :
                (BANG )?
                ( element )* ( exceptionSpecNoLabel )?
    ;

exceptionGroup
        :
                ( exceptionSpec )+
   ;

exceptionSpec
   :
   'exception'
   (ARG_ACTION )?
   ( exceptionHandler )*
   ;

exceptionSpecNoLabel
   :
   'exception'
   ( exceptionHandler )*
   ;

exceptionHandler
   :
   'catch'ARG_ACTIONACTION
   ;

element
        :       elementNoOptionSpec (elementOptionSpec)?
        ;

elementOptionSpec
        :       OPEN_ELEMENT_OPTIONid ASSIGNoptionValue
                (
                        SEMIid ASSIGNoptionValue
                )*
                CLOSE_ELEMENT_OPTION
        ;

elementNoOptionSpec
        :id
                ASSIGN
                (id COLON )?
                (RULE_REF
                        (ARG_ACTION )?
                        ( BANG )?
                |TOKEN_REF
                        (ARG_ACTION )?
                )
        |
                (id COLON )?
                (RULE_REF
                        (ARG_ACTION )?
                        ( BANG )?
                |
                        range
                |
                        terminal
                |
                        NOT_OP
                        (       notTerminal
                        |       ebnf
                        )
                |
                        ebnf
                )
        |ACTION
        |SEMPRED
        |
                tree
        ;

tree_ :TREE_BEGIN
        rootNode
        ( element )+
        RPAREN
        ;

rootNode
        :
                (id COLON )?
                terminal
//      |       range[null]
        ;

ebnf
        :LPAREN

                (
                        subruleOptionsSpec
                        (ACTION )?
                        COLON
                |ACTION
                        COLON
                )?

                block
                RPAREN

                (       (       QUESTION
                        |       STAR
                        |       PLUS
                        )?
                        ( BANG )?
                |
                        IMPLIES
                )
        ;

ast_type_spec
        :       (       CARET
                |       BANG
                )?
        ;

range
        :CHAR_LITERAL RANGECHAR_LITERAL
                ( BANG )?
        |
                (TOKEN_REF|STRING_LITERAL)
                RANGE
                (TOKEN_REF|STRING_LITERAL) ast_type_spec
        ;

terminal
        :CHAR_LITERAL
                ( BANG )?
        |TOKEN_REF ast_type_spec
                // Args are only valid for lexer
                (ARG_ACTION )?
        |STRING_LITERAL ast_type_spec
        |WILDCARD ast_type_spec
        ;

notTerminal
        :CHAR_LITERAL
                ( BANG )?
        |TOKEN_REF ast_type_spec
        ;

/** Match a.b.c.d qualified ids; WILDCARD here is overloaded as
 *  id separator; that is, I need a reference to the '.' token.
 */
qualifiedID
        :id
                (       WILDCARDid
                )*
        ;

id
        :TOKEN_REF
        |RULE_REF
        ; 
DUMMY_0 : 'tokens';
 
DUMMY_1 : 'options';


WS      :       (       ' '
                |       '\t'
                |       '\r' '\n'
                |       '\r'
                |       '\n'
                )
        ;

COMMENT :
        ( SL_COMMENT |ML_COMMENT )
        ;
SL_COMMENT :
        '//'
        ( ~('\n'|'\r') )*
        (       '\r' '\n'
                |       '\r'
                |       '\n'
        )
        ;
ML_COMMENT :
        '/*'
        (       { LA(2)!='/' }? '*'
        |
        )
        (       '\r' '\n'
        |       '\r'
        |       '\n'
        |       ~('\n'|'\r')
        )*
        '*/'
        ;

OPEN_ELEMENT_OPTION
        :       '<'
        ;

CLOSE_ELEMENT_OPTION
        :       '>'
        ;

COMMA : ',';

QUESTION :      '?' ;

TREE_BEGIN : '#(' ;

LPAREN: '(' ;

RPAREN: ')' ;

COLON : ':' ;

STAR:   '*' ;

PLUS:   '+' ;

ASSIGN : '=' ;

IMPLIES : '=>' ;

SEMI:   ';' ;

CARET : '^' ;

BANG : '!' ;

OR      :       '|' ;

WILDCARD : '.' ;

RANGE : '..' ;

NOT_OP :        '~' ;

RCURLY: '}'     ;

CHAR_LITERAL
        :       '\'' (ESC|~'\'') '\''
        ;

STRING_LITERAL
        :       '"' (ESC|~'"')* '"'
        ;
ESC     :       '\\'
                (       'n'
                |       'r'
                |       't'
                |       'b'
                |       'f'
                |       'w'
                |       'a'
                |       '"'
                |       '\''
                |       '\\'
                |       ('0'..'3')
                        ( '0'..'7'
                        ( '0'..'7'
                   )?
                        )?
                |       ('4'..'7')
                        ( '0'..'7' )?
                |       'u' XDIGIT XDIGIT XDIGIT XDIGIT
                )
        ;
DIGIT
        :       '0'..'9'
        ;
XDIGIT :
                '0' .. '9'
        |       'a' .. 'f'
        |       'A' .. 'F'
        ;

INT     :       ('0'..'9')+
        ;

ARG_ACTION
   :
        NESTED_ARG_ACTION
        ;
NESTED_ARG_ACTION :
        '['
        (       NESTED_ARG_ACTION
        |       '\r' '\n'
        |       '\r'
        |       '\n'
        |       CHAR_LITERAL
        |       STRING_LITERAL
        |       ~']'
        )*
        ']'
        ;

ACTION
        :       NESTED_ACTION
                (       '?' )?
        ;
NESTED_ACTION :
        '{'
        (
                (       '\r' '\n'
                |       '\r'
                |       '\n'
                )
        |       NESTED_ACTION
        |       CHAR_LITERAL
        |       COMMENT
        |       STRING_LITERAL
        |       .
        )*
        '}'
   ;

TOKEN_REF
        :       'A'..'Z'
                (
                        'a'..'z'|'A'..'Z'|'_'|'0'..'9'
                )*
        ;

// we get a warning here when looking for options '{', but it works right
RULE_REF
        :INTERNAL_RULE_REF
                (       {t==LITERAL_options}? WS_LOOP ('{')?
                |       {t==LITERAL_tokens}? WS_LOOP ('{')?
                |
                )
        ;
WS_LOOP
        :       (
                        WS
                |       COMMENT
                )*
        ;
INTERNAL_RULE_REF
        :       'a'..'z'
                (
                        'a'..'z'|'A'..'Z'|'_'|'0'..'9'
                )*
        ;
WS_OPT :
        (WS)?
        ;
