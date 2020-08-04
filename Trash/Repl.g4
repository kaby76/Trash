grammar Repl;

cmd :
  alias
  | anything
  | bang
  | dot
  | empty
  | find
  | fold
  | history
  | import_
  | print
  | quit
  | read
  | rename
  | unfold
  ;
alias : ALIAS (id '=' (StringLiteral | id_keyword))?;
anything : id rest? ';' ;
bang : BANG (BANG | int | id) ;
combine : COMBINE ';' ;
dot : DOT ';' ;
find : FIND StringLiteral ';' ;
fold : FOLD StringLiteral ';' ;
history : HISTORY ;
import_ : IMPORT type? ffn ';' ;
print : PRINT ';' ;
quit : (QUIT | EXIT) ';' ;
read : READ ffn ';' ;
rename : RENAME StingLiteral StringLiteral ';' ;
split : SPLIT ';' ;
unfold : UNFOLD StringLiteral ';' ;

empty : ';' ;
ffn : StringLiteral ;
int : INT ;
id : ID ;
id_keyword : id
  | ALIAS
  | ANTLR3
  | ANTLR2
  | BISON
  | COMBINE
  | EXIT
  | FIND
  | FOLD
  | HISTORY
  | IMPORT
  | PRINT
  | QUIT
  | READ
  | RENAME
  | SPLIT
  | UNFOLD
  ;
rest : (id_keyword | int | StringLiteral )+;
type : ANTLR3 | ANTLR2 | BISON;

ALIAS : 'alias';
ANTLR2 : 'antlr2';
ANTLR3 : 'antlr3';
BANG : '!';
BISON : 'bison';
COMBINE : 'combine';
DOT : '.';
EXIT : 'exit';
FIND : 'find';
FOLD : 'fold';
HISTORY : 'history';
IMPORT : 'import';
INT: [0-9]+;
PRINT : 'print';
QUIT : 'quit';
READ : 'read';
RENAME : 'rename';
SPLIT : 'split';
StringLiteral : ('\'' Lca Lca* '\'') | ('"' Lcb Lcb* '"') ;
UNFOLD : 'unfold';

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
