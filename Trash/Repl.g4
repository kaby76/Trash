grammar Repl;

cmd :
  alias
  | analyze
  | bang
  | convert
  | dot
  | empty
  | find
  | fold
  | history
  | parse
  | print
  | quit
  | read
  | rename
  | rotate
  | stack
  | unalias
  | unfold
  | write
  | anything
  ;
alias : ALIAS (id '=' (StringLiteral | id_keyword))? ';' ;
analyze : ANALYZE ';' ;
anything : id rest? ';' ;
bang : BANG (BANG | int | id_keyword) ';' ;
combine : COMBINE ';' ;
convert : CONVERT type ';' ;
dot : DOT ';' ;
find : FIND StringLiteral ';' ;
fold : FOLD StringLiteral ';' ;
history : HISTORY ';' ;
parse : PARSE type ';' ;
print : PRINT ';' ;
quit : (QUIT | EXIT) ';' ;
read : READ ffn ';' ;
rename : RENAME StingLiteral StringLiteral ';' ;
split : SPLIT ';' ;
stack : STACK ';' ;
rotate : ROTATE ';' ;
unalias : UNALIAS id ';' ;
unfold : UNFOLD StringLiteral ';' ;
write : WRITE ';' ;

empty : ';' ;
ffn : StringLiteral ;
int : INT ;
id : ID ;
id_keyword : id
  | ALIAS
  | ANALYZE
  | ANTLR3
  | ANTLR2
  | BISON
  | COMBINE
  | CONVERT
  | EXIT
  | FIND
  | FOLD
  | HISTORY
  | PARSE
  | PRINT
  | QUIT
  | READ
  | RENAME
  | ROTATE
  | SPLIT
  | STACK
  | UNALIAS
  | UNFOLD
  | WRITE
  ;
rest : .+;
type : ANTLR4 | ANTLR3 | ANTLR2 | BISON;

ALIAS : 'alias';
ANALYZE : 'analyze';
ANTLR2 : 'antlr2';
ANTLR3 : 'antlr3';
ANTLR4 : 'antlr4';
BANG : '!';
BISON : 'bison';
COMBINE : 'combine';
CONVERT : 'convert';
DOT : '.';
EXIT : 'exit';
FIND : 'find';
FOLD : 'fold';
HISTORY : 'history';
INT: [0-9]+;
READ : 'read';
PARSE : 'parse' ;
PRINT : 'print';
QUIT : 'quit';
RENAME : 'rename';
SPLIT : 'split';
STACK : 'stack';
ROTATE : 'rotate';
StringLiteral : ('\'' Lca Lca* '\'') | ('"' Lcb Lcb* '"') ;
UNALIAS : 'unalias' ;
UNFOLD : 'unfold';
WRITE : 'write';

fragment Lca : Esc | ~ ('\'' | '\\') ;
fragment Lcb : Esc | ~ ('"' | '\\') ;
fragment Esc : '\\' ('n' | 'r' | 't' | 'b' | 'f' | '"' | '\'' | '\\' | '>' | 'u' XDIGIT XDIGIT XDIGIT XDIGIT | .) ;
fragment XDIGIT : '0' .. '9' | 'a' .. 'f' | 'A' .. 'F' ;
fragment BlockComment : '/*' ( ('/' ~'*') | ~'/' )* '*/' ;
fragment LineComment : '//' ~[\r\n]* ;
fragment LineCommentExt : '//' ~'\n'* ( '\n' Hws* '//' ~'\n'* )* ;
BLOCK_COMMENT : BlockComment -> skip ;
LINE_COMMENT : LineComment -> skip ;
WS : ( Hws | Vws )+ -> skip ;
fragment Ws : Hws | Vws ;
fragment Hws : [ \t] ;
fragment Vws : [\r\n\f] ;
ID: Id;
fragment Id : NameStartChar NameChar* ;
fragment Underscore : '_' ;
fragment NameStartChar : 'A'..'Z' | 'a'..'z' | '_'
	|	'\u00C0'..'\u00D6'
	|	'\u00D8'..'\u00F6'
	|	'\u00F8'..'\u02FF'
	|	'\u0370'..'\u037D'
	|	'\u037F'..'\u1FFF'
	|	'\u200C'..'\u200D'
	|	'\u2070'..'\u218F'
	|	'\u2C00'..'\u2FEF'
	|	'\u3001'..'\uD7FF'
	|	'\uF900'..'\uFDCF'
	|	'\uFDF0'..'\uFFFD'
	| '$' // For PHP
	;	// ignores | ['\u10000-'\uEFFFF] ;
fragment NameChar
	:	NameStartChar
	|	'0'..'9'
	|	Underscore
	|	'\u00B7'
	|	'\u0300'..'\u036F'
	|	'\u203F'..'\u2040'
	| '.'
	| '-'
	;
