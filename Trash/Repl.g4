grammar Repl;

cmd
	: read
	| import_
	| quit
	| empty
	;
quit : 'quit' | 'exit';
read : 'read' ffn ';' ;
import_ : 'import' type? ffn ';' ;
type : 'antlr3' | 'antlr2' | 'bison';
ffn : StringLiteral ;
empty : ';' ;
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
WS : ([ \t] | [\r\n\f])+ -> skip ;
