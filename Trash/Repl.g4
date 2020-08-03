grammar Repl;

cmd
	:
	  alias
	| anything
	| bang
	| dot
	| empty
	| find
	| history
	| import_
	| print
	| quit
	| read
	;
anything : id rest? ';' ;
rest : (id_keyword | int | StringLiteral )+;
bang : BANG (BANG | int | id) ;
dot : DOT ';' ;
print : PRINT ';' ;
quit : (QUIT | EXIT) ';' ;
history : HISTORY ;
alias : ALIAS (id '=' (StringLiteral | id_keyword))?;
read : READ ffn ';' ;
import_ : IMPORT type? ffn ';' ;
find : FIND StringLiteral ';' ;

type : ANTLR3 | ANTLR2 | BISON;
ffn : StringLiteral ;
empty : ';' ;
int : INT ;
id_keyword : id
  | ALIAS
  | ANTLR3
  | ANTLR2
  | BISON
  | EXIT
  | FIND
  | HISTORY
  | IMPORT
  | PRINT
  | QUIT
  | READ
  ;

ALIAS : 'alias';
ANTLR3 : 'antlr3';
ANTLR2 : 'antlr2';
BISON : 'bison';
EXIT : 'exit';
FIND : 'find';
HISTORY : 'history';
IMPORT : 'import';
PRINT : 'print';
QUIT : 'quit';
READ : 'read';
DOT : '.';
BANG : '!';
INT: [0-9]+;
StringLiteral : ('\'' Lca Lca* '\'') | ('"' Lcb Lcb '"') ;
fragment Lca : Esc | ~ ('\'' | '\\') ;
fragment Lcb : Esc | ~ ('"' | '\\') ;
fragment Esc
   : '\\' ('n' | 'r' | 't' | 'b' | 'f' | '"' | '\'' | '\\' | '>' | 'u' XDIGIT XDIGIT XDIGIT XDIGIT | .)
   ;

fragment XDIGIT
   : '0' .. '9'
   | 'a' .. 'f'
   | 'A' .. 'F'
   ;

fragment BlockComment
	: '/*' 
     (
	   ('/' ~'*')
	   | ~'/'
	 )*
	 '*/'
	;

fragment LineComment
	: '//' ~[\r\n]*
	;

fragment LineCommentExt
	: '//' ~'\n'* ( '\n' Hws* '//' ~'\n'* )*
	;

BLOCK_COMMENT
	:	BlockComment -> skip
	;

LINE_COMMENT
	:	LineComment -> skip
	;

WS
    : ( Hws | Vws )+ -> skip
    ;

fragment Ws
	: Hws
	| Vws
	;

fragment Hws
	: [ \t]
	;

fragment Vws
	: [\r\n\f]
	;

id : ID;

ID:                Id;

fragment Id
	: NameStartChar NameChar*
	;

fragment Underscore
	: '_'
	;

fragment NameStartChar
	:	'A'..'Z'
	|	'a'..'z'
    | '_'
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
