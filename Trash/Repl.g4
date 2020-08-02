grammar Repl;

cmd
	: read
	| history
	| import_
	| quit
	| empty
	| bang
	;
bang : BANG (BANG | int | id) ;
quit : 'quit' | 'exit';
history : 'history' ;
alias : 'alias' id '=' (StringLiteral | id);
read : 'read' ffn ';' ;
import_ : 'import' type? ffn ';' ;
type : 'antlr3' | 'antlr2' | 'bison';
ffn : StringLiteral ;
empty : ';' ;
int : INT ;

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
