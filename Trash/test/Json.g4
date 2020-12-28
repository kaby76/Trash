grammar Json;

json:   object
    |   array
    ;

object
    :   '{' pair (',' pair)* '}'
    |   '{' '}' // empty object
    ;
pair:   STRING ':' value ;

array
    :   '[' value (',' value)* ']'
    |   '[' ']' // empty array
    ;

value
    :   STRING
    |   NUMBER
    |   object  // indirect recursion
    |   array   // indirec recursion
    |   'true'
    |   'false'
    |   'null'
    ;
