grammar Repl;

cmds : cmd+ ;
cmd :
  ( alias
  | analyze
  | bang
  | convert
  | combine
  | dot
  | delete
  | empty
  | find
  | fold
  | history
  | kleene
  | mvsr
  | parse
  | pop
  | print
  | quit
  | read
  | rename
  | reorder
  | rotate
  | rup
  | split
  | stack
  | unalias
  | unfold
  | unify
  | ulliteral
  | write
  | anything
  ) ';'
  ;
alias : ALIAS (id '=' (StringLiteral | id_keyword))? ;
analyze : ANALYZE ;
anything : id rest? ;
bang : BANG (BANG | int | id_keyword) ;
combine : COMBINE ;
convert : CONVERT type ;
delete : DELETE StringLiteral ;
dot : DOT ;
empty : ;
find : FIND StringLiteral ;
fold : FOLD StringLiteral ;
history : HISTORY ;
kleene : KLEENE StringLiteral ;
mvsr : MVSR StringLiteral ;
parse : PARSE type? ;
pop : POP ;
print : PRINT ;
quit : (QUIT | EXIT) ;
read : READ ffn ;
rename : RENAME StringLiteral StringLiteral ;
reorder : REORDER (alpha | bfs | dfs | modes) ;
rotate : ROTATE ;
rup : RUP StringLiteral ;
split : SPLIT ;
stack : STACK ;
unalias : UNALIAS id ;
unfold : UNFOLD StringLiteral ;
unify : UNIFY StringLiteral ;
ulliteral : ULLITERAL StringLiteral ;
write : WRITE ;
alpha : ALPHA ;
bfs : BFS StringLiteral ;
dfs : DFS StringLiteral ;
ffn : StringLiteral ;
modes : MODES ;
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
  | DELETE
  | EXIT
  | FIND
  | FOLD
  | HISTORY
  | KLEENE
  | MVSR
  | PARSE
  | POP
  | PRINT
  | QUIT
  | READ
  | RENAME
  | REORDER
  | ROTATE
  | RUP
  | SPLIT
  | STACK
  | ULLITERAL
  | UNALIAS
  | UNFOLD
  | UNIFY
  | WRITE
  ;
rest : .+ ;
type : ANTLR4 | ANTLR3 | ANTLR2 | BISON ;
ALPHA : 'alpha' ;
ALIAS : 'alias' ;
ANALYZE : 'analyze';
ANTLR2 : 'antlr2' ;
ANTLR3 : 'antlr3' ;
ANTLR4 : 'antlr4' ;
BANG : '!' ;
BFS : 'bfs' ;
BISON : 'bison' ;
COMBINE : 'combine' ;
CONVERT : 'convert' ;
DELETE : 'delete' ;
DFS : 'dfs' ;
DOT : '.' ;
EXIT : 'exit' ;
FIND : 'find' ;
FOLD : 'fold' ;
HISTORY : 'history' ;
INT : [0-9]+ ;
KLEENE : 'kleene' ;
MODES : 'modes' ;
MVSR : 'mvsr' ;
PARSE : 'parse' ;
POP : 'pop' ;
PRINT : 'print' ;
QUIT : 'quit' ;
READ : 'read' ;
RENAME : 'rename' ;
REORDER : 'reorder' ;
ROTATE : 'rotate' ;
RUP : 'rup' ;
SPLIT : 'split' ;
STACK : 'stack' ;
StringLiteral : ('\'' Lca Lca* '\'') | ('"' Lcb Lcb* '"') ;
ULLITERAL : 'ulliteral' ;
UNALIAS : 'unalias' ;
UNFOLD : 'unfold' ;
UNIFY : 'unify' ;
WRITE : 'write' ;

// Last on purpose.
ID: Id ;

BLOCK_COMMENT : BlockComment -> skip ;
LINE_COMMENT : LineComment -> skip ;
WS : ( Hws | Vws )+ -> skip ;

fragment Lca : Esc | ~ ('\'' | '\\') ;
fragment Lcb : Esc | ~ ('"' | '\\') ;
fragment Esc : '\\' ('n' | 'r' | 't' | 'b' | 'f' | '"' | '\'' | '\\' | '>' | 'u' XDIGIT XDIGIT XDIGIT XDIGIT | .) ;
fragment XDIGIT : '0' .. '9' | 'a' .. 'f' | 'A' .. 'F' ;
fragment BlockComment : '/*' ( ('/' ~'*') | ~'/' )* '*/' ;
fragment LineComment : '//' ~[\r\n]* ;
fragment LineCommentExt : '//' ~'\n'* ( '\n' Hws* '//' ~'\n'* )* ;
fragment Ws : Hws | Vws ;
fragment Hws : [ \t] ;
fragment Vws : [\r\n\f] ;
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
