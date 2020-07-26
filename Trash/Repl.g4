grammar Repl;

cmd
	: import_
	;

import_ : 'import' 'antlr3'? ffn ';' ;
ffn : StringLiteral ;
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
