lexer grammar ReplLexer;

fragment Lca : Esc | ~ ('\'' | '\\') ;
fragment Lcb : Esc | ~ ('"' | '\\') ;
fragment Esc : '\\' ('n' | 'r' | 't' | 'b' | 'f' | '"' | '\'' | '\\' | '>' | 'u' XDigit XDigit XDigit XDigit | .) ;
fragment XDigit : '0' .. '9' | 'a' .. 'f' | 'A' .. 'F' ;
fragment Digit : '0' .. '9' ;
fragment BlockComment : '/*' ( ('/' ~'*') | ~'/' )* '*/' ;
fragment LineComment : '--' ~[\r\n]* ;
fragment Ws : Hws | Vws ;
fragment Hws : ('\u0020'| '\u000B');
fragment Vws : ('\u000A' | '\u000D') ;
fragment Id : NameStartChar NameChar* ;
fragment Underscore : '_' ;
fragment NameStartChar : 'A'..'Z' | 'a'..'z' | '_'
	| '\u00C0'..'\u00D6'
	| '\u00D8'..'\u00F6'
	| '\u00F8'..'\u02FF'
	| '\u0370'..'\u037D'
	| '\u037F'..'\u1FFF'
	| '\u200C'..'\u200D'
	| '\u2070'..'\u218F'
	| '\u2C00'..'\u2FEF'
	| '\u3001'..'\uD7FF'
	| '\uF900'..'\uFDCF'
	| '\uFDF0'..'\uFFFD'
	| '$' // For PHP
	; // ignores | ['\u10000-'\uEFFFF] ;
fragment NameChar
	: NameStartChar
	| '0'..'9'
	| Underscore
	| '\u00B7'
	| '\u0300'..'\u036F'
	| '\u203F'..'\u2040'
	| '.'
	| '-'
	;

BLOCK_COMMENT : BlockComment -> skip ;
LINE_COMMENT : LineComment -> skip ;
HWS : Hws -> skip ;
VWS : Vws -> skip ;

mode CommandMode;
AGL : 'agl' -> pushMode(ArgMode) ;
ALIAS : 'alias' -> pushMode(ArgMode) ;
ANALYZE : 'analyze' -> pushMode(ArgMode) ;
BANG : '!' ('!' | Digit+ | Id) -> pushMode(ArgMode) ;
CD : 'cd' -> pushMode(ArgMode) ;
COMBINE : 'combine' -> pushMode(ArgMode) ;
CONVERT : 'convert' -> pushMode(ArgMode) ;
DOT : '.' -> pushMode(ArgMode) ;
DELABEL : 'delabel' -> pushMode(ArgMode) ;
DELETE : 'delete' -> pushMode(ArgMode) ;
FOLD : 'fold' -> pushMode(ArgMode) ;
FOLDLIT : 'foldlit' -> pushMode(ArgMode) ;
GROUP : 'group' -> pushMode(ArgMode) ;
HAS : 'has' -> pushMode(ArgMode) ;
HELP : 'help' -> pushMode(ArgMode) ;
HISTORY : 'history' -> pushMode(ArgMode) ;
JSON : 'json' -> pushMode(ArgMode) ;
KLEENE : 'kleene' -> pushMode(ArgMode) ;
LS : 'ls' -> pushMode(ArgMode) ;
MVSR : 'mvsr' -> pushMode(ArgMode) ;
PARSE : 'parse' -> pushMode(ArgMode) ;
POP : 'pop' -> pushMode(ArgMode) ;
PRINT : 'print' -> pushMode(ArgMode) ;
PWD : 'pwd' -> pushMode(ArgMode) ;
QUIT : 'quit' -> pushMode(ArgMode) ;
READ : 'read' -> pushMode(ArgMode) ;
RENAME : 'rename' -> pushMode(ArgMode) ;
REORDER : 'reorder' -> pushMode(ArgMode) ;
ROTATE : 'rotate' -> pushMode(ArgMode) ;
RR : 'rr' -> pushMode(ArgMode) ;
RUN : 'run' -> pushMode(ArgMode) ;
RUP : 'rup' -> pushMode(ArgMode) ;
SET : 'set' -> pushMode(ArgMode) ;
SPLIT : 'split' -> pushMode(ArgMode) ;
ST : 'st' -> pushMode(ArgMode) ;
STACK : 'stack' -> pushMode(ArgMode) ;
SVG : 'svg' -> pushMode(ArgMode) ;
TEXT : 'text' -> pushMode(ArgMode) ;
ULLITERAL : 'ulliteral' -> pushMode(ArgMode) ;
UNALIAS : 'unalias' -> pushMode(ArgMode) ;
UNFOLD : 'unfold' -> pushMode(ArgMode) ;
UNGROUP : 'ungroup' -> pushMode(ArgMode) ;
UNULLITERAL : 'unulliteral' -> pushMode(ArgMode) ;
VERSION : 'version' -> pushMode(ArgMode) ;
WORKSPACE : 'workspace' -> pushMode(ArgMode) ;
WRITE : 'write' -> pushMode(ArgMode) ;
XGREP : 'xgrep' -> pushMode(ArgMode) ;
XML : 'xml' -> pushMode(ArgMode) ;
HWSCommandMode : Hws -> skip ;
VWSCommandMode : Vws -> skip ;
BLOCK_COMMENT_CommandMode : BlockComment -> skip ;
LINE_COMMENT_CommandMode : LineComment -> skip ;
ID : Id -> pushMode(ArgMode) ;
INT : Digit+ -> pushMode(ArgMode) ;

mode ArgMode;
ALPHA : 'alpha' ;
ANTLR2 : 'antlr2' ;
ANTLR3 : 'antlr3' ;
ANTLR4 : 'antlr4' ;
BFS : 'bfs' ;
BISON : 'bison' ;
BUILD : 'build' ;
DFS : 'dfs' ;
DR : 'dr' ;
EBNF : 'ebnf' ;
EXIT : 'exit' ;
GENERATE : 'generate' ;
GRAPH : 'graph' ;
IR : 'ir' ;
LC : 'lc' ;
MODES : 'modes' ;
StringLiteral : ('\'' Lca Lca* '\'') | ('"' Lcb Lcb* '"') ;
UC : 'uc' ;
EQUAL : '=' ;
SEMI : ';' ;
PIPE : '|' -> popMode ;
NonWsArgMode : ~(' ' | '\n' | '\r' | ';' | '=' | '!' | '|') (~(' ' | '\n' | '\r' | ';' | '=' | '|'))* ;
HWSArgMode : Hws -> skip ;
VWSArgMode : Vws -> skip ;
BLOCK_COMMENT_ArgMode : BlockComment -> skip ;
LINE_COMMENT_ArgMode : LineComment -> skip ;

