grammar ClassSyntaxDemo;

options { language=CSharp; }

// The parser uses both a keyword and a general identifier.
start : (If Identifier | Identifier) EOF;

fragment Letter : [a-zA-Z];
If : 'if';
Identifier : Letter+;
Whitespace : [ \t\r\n]+ -> skip;
