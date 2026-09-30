// From https://github.com/antlr/grammars-v4/tree/master/json/json
// The grammar is also present in ~/issues/g4-current/json/JSON.g4.
grammar JSON;

json: value EOF;
obj: '{' pair (',' pair)* '}' | '{' '}';
pair: STRING ':' value;
arr: '[' value (',' value)* ']' | '[' ']';
value: STRING | NUMBER | obj | arr | 'true' | 'false' | 'null';

STRING: '"' (ESC | SAFECODEPOINT)* '"';
fragment ESC: '\\' (["\\/bfnrt] | UNICODE);
fragment UNICODE: 'u' HEX HEX HEX HEX;
fragment HEX: [0-9a-fA-F];
fragment SAFECODEPOINT: ~ ["\\\u0000-\u001F];
NUMBER: '-'? INT ('.' [0-9]+)? EXP?;
fragment INT: '0' | [1-9] [0-9]*;
fragment EXP: [Ee] [+-]? [0-9]+;
WS: [ \t\n\r]+ -> skip;
