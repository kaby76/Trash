grammar RuleBoundary;

document : definition+ EOF;
definition : ID '=' contents;
contents : item+;
item : ID | STRING;

ID : [a-z]+;
STRING : '"' ~["\r\n]* '"';
WS : [ \t\r\n]+ -> channel(HIDDEN);
