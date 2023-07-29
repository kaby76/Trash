grammar ParseTreeScript;

commands : (command ';')* EOF;
command : 'inserta' string string | 'insertb' string string | 'delete' string | 'replace' string;
string : String;
WS : [ \n\r\t]+ -> channel(HIDDEN);
String
 : '"' DoubleStringCharacter* '"'
 | '\'' SingleStringCharacter* '\''
 ;
WhiteSpaces
 : [\t\u000B\u000C\u0020\u00A0]+ -> channel(HIDDEN)
 ;
MultiLineComment
 : '/*' .*? '*/' -> channel(HIDDEN)
 ;
SingleLineComment
 : '//' ~[\r\n\u2028\u2029]* -> channel(HIDDEN)
 ;
fragment DoubleStringCharacter
 : ~["\\\r\n]
 | '\\' EscapeSequence
 ;
fragment SingleStringCharacter
 : ~['\\\r\n]
 | '\\' EscapeSequence
 ;
fragment EscapeSequence
 : CharacterEscapeSequence
 ;
fragment CharacterEscapeSequence
 : SingleEscapeCharacter
 | NonEscapeCharacter
 ;
fragment SingleEscapeCharacter
 : ['"\\bfnrtv]
 ;
fragment NonEscapeCharacter
 : ~['"\\bfnrtv0-9xu\r\n]
 ;
fragment EscapeCharacter
 : SingleEscapeCharacter
 ;
