grammar RuleNames;

document
    : header section* EOF
    ;

header
    : TITLE
    ;

section
    : ITEM+
    ;

TITLE
    : 'title'
    ;

ITEM
    : 'item'
    ;

WS
    : [ \t\r\n]+ -> skip
    ;
