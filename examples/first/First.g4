grammar First;

start
    : prefix? choice terminator*
    ;

prefix
    : 'prefix'
    ;

choice
    : recursiveA
    | nullablePrefix 'after'
    | ('red' | 'blue') ID
    | ~('x' | 'y')
    | .
    ;

recursiveA
    : recursiveB
    | ID
    ;

recursiveB
    : '(' recursiveA ')'
    | NUMBER
    ;

nullablePrefix
    : 'maybe'?
    ;

terminator
    : ';'
    ;

ID
    : [a-zA-Z]+
    ;

NUMBER
    : [0-9]+
    ;

WS
    : [ \t\r\n]+ -> skip
    ;
