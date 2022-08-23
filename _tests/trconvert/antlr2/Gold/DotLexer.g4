
lexer grammar DotLexer;

LITERALS
    :  ( GRAPH_LITERAL
        | DIGRAPH_LITERAL
        | STRICT_LITERAL
        | NODE_LITERAL
        | EDGE_LITERAL
        | EDGEOP_LITERAL
        | EDGEOP_LITERAL
        | O_BRACKET
        | C_BRACKET
        | O_SQR_BRACKET
        | C_SQR_BRACKET
        | SEMI_COLON
        | EQUAL
        | COMMA
        | COLON
        | ID)
    ; GRAPH_LITERAL : 'graph'; DIGRAPH_LITERAL : 'digraph'; STRICT_LITERAL : 'strict'; NODE_LITERAL : 'node'; EDGE_LITERAL : 'edge'; O_BRACKET : '{'; C_BRACKET : '}'; O_SQR_BRACKET : '['; C_SQR_BRACKET : ']'; SEMI_COLON : ';'; EQUAL : '='; COMMA : ','; COLON : ':'; EDGEOP_LITERAL
    :  ( '->'
        | '--'
       )
    ; ID
    :  (  VALIDSTR
        | NUMBER
        | QUOTEDSTR
        | HTMLSTR
       ); COMPASS_PT
    :  ( 'ne'
        | 'nw'
        | NODE_LITERAL
        | 'n'
        | 'e'
        | 'se'
        | 'sw'
        | 's'
        | 'w'
       ); ALPHACHAR
    :  (   'a'..'z'
        |  'A'..'Z'
        |  '_'
       ); VALIDSTR
    :  ALPHACHAR
        (  ALPHACHAR
         |  '0'..'9'
        )*
    ; NUMBER
    :  ('-')? ('0'..'9')+ ('.' ('0'..'9')+)?
    ; QUOTEDSTR
    :  '"'
       ( '\\\"'
        | ~('"')
       )*
       '"'
    ; HTMLSTR
    :  '<' (~'>')* '>'
    ;

WS
    :
       (   ' '
        |  '\t'
        |  '\r' '\n'
        |  '\n'
       ) //ignore this token
    ;

// Single-line comments
COMMENT
    :  ( ML_COMMENT
        | SL_COMMENT)
    ; SL_COMMENT
    :  '//'
       (~('\n'|'\r'))* ('\n'|'\r'('\n')?)
    ; ML_COMMENT
    :  '/*'
       (
               {LA(2)!='/'}? '*'
             | '\r' '\n'
             | '\r'
             | '\n'
             | ~('*'|'\n'|'\r')
       )*
       '*/'
    ;