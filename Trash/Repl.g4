grammar Repl;

cmds
  : (cmd HWS* (SEMI | VWS+))+
  ;
cmd :
  HWS*
  ( alias
  | analyze
  | bang
  | cd
  | convert
  | combine
  | dot
  | delete
  | empty
  | find
  | fold
  | foldlit
  | has
  | history
  | kleene
  | ls
  | mvsr
  | parse
  | pop
  | print
  | pwd
  | quit
  | read
  | rename
  | reorder
  | rotate
  | rr
  | rup
  | set
  | split
  | stack
  | unalias
  | unfold
  | unify
  | ulliteral
  | write
  | anything
  )
  HWS*
  ;
alias : ALIAS HWS* (ID HWS* EQUAL HWS* (StringLiteral | id_keyword))? ;
analyze : ANALYZE ;
anything : id HWS* stuff* ;
bang : BANG HWS* (BANG | int | id_keyword) ;
cd : CD HWS* arg? ;
combine : COMBINE ;
convert : CONVERT HWS* type ;
delete : DELETE HWS* arg ;
dot : DOT ;
empty : ;
find : FIND HWS* arg ;
fold : FOLD HWS* arg ;
foldlit : FOLDLIT HWS* arg ;
has : HAS HWS* (DR | IR) HWS* GRAPH? HWS* arg? ;
history : HISTORY ;
kleene : KLEENE HWS* arg? ;
ls : LS HWS* arg?  ;
mvsr : MVSR HWS* StringLiteral ;
parse : PARSE HWS* type? ;
pop : POP ;
print : PRINT ;
pwd : PWD ;
quit : (QUIT | EXIT) ;
read : READ HWS* arg ;
rename : RENAME HWS* StringLiteral HWS* StringLiteral ;
reorder : REORDER HWS* (alpha | bfs | dfs | modes) ;
rotate : ROTATE ;
rr : RR HWS* StringLiteral ;
rup : RUP HWS* StringLiteral? ;
set : SET HWS* id_keyword HWS* '=' HWS* (StringLiteral | INT) ;
split : SPLIT ;
stack : STACK ;
unalias : UNALIAS HWS* id ;
unfold : UNFOLD HWS* arg ;
unify : UNIFY HWS* arg ;
ulliteral : ULLITERAL HWS* StringLiteral? ;
write : WRITE ;
alpha : ALPHA ;
bfs : BFS HWS* StringLiteral ;
dfs : DFS HWS* StringLiteral ;
modes : MODES ;
int : INT ;
id : ID ;
arg : (StringLiteral | stuff) ;
id_keyword : id
  | ALIAS
  | ANALYZE
  | ANTLR3
  | ANTLR2
  | BISON
  | CD
  | COMBINE
  | CONVERT
  | DELETE
  | DR
  | EXIT
  | FIND
  | FOLD
  | FOLDLIT
  | GRAPH
  | HAS
  | HISTORY
  | IR
  | KLEENE
  | LS
  | MVSR
  | PARSE
  | POP
  | PRINT
  | PWD
  | QUIT
  | READ
  | RENAME
  | REORDER
  | ROTATE
  | RR
  | RUP
  | SET
  | SPLIT
  | STACK
  | ULLITERAL
  | UNALIAS
  | UNFOLD
  | UNIFY
  | WRITE
  ;
stuff : STUFF | id_keyword ;
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
CD : 'cd' ;
COMBINE : 'combine' ;
CONVERT : 'convert' ;
DELETE : 'delete' ;
DFS : 'dfs' ;
DR : 'dr' ;
DOT : '.' ;
EQUAL : '=' ;
EXIT : 'exit' ;
FIND : 'find' ;
FOLD : 'fold' ;
FOLDLIT : 'foldlit' ;
GRAPH : 'graph' ;
HAS : 'has' ;
HISTORY : 'history' ;
INT : [0-9]+ ;
IR : 'ir' ;
LS : 'ls' ;
KLEENE : 'kleene' ;
MODES : 'modes' ;
MVSR : 'mvsr' ;
PARSE : 'parse' ;
POP : 'pop' ;
PRINT : 'print' ;
PWD : 'pwd' ;
QUIT : 'quit' ;
READ : 'read' ;
RENAME : 'rename' ;
REORDER : 'reorder' ;
ROTATE : 'rotate' ;
RR : 'rr' ;
RUP : 'rup' ;
SEMI : ';' ;
SET : 'set' ;
SPLIT : 'split' ;
STACK : 'stack' ;
StringLiteral : ('\'' Lca Lca* '\'') | ('"' Lcb Lcb* '"') ;
ULLITERAL : 'ulliteral' ;
UNALIAS : 'unalias' ;
UNFOLD : 'unfold' ;
UNIFY : 'unify' ;
WRITE : 'write' ;
ID: Id ;
BLOCK_COMMENT : BlockComment -> skip ;
LINE_COMMENT : LineComment -> skip ;
HWS : Hws ;
VWS : Vws ;
STUFF : (~([!] | [;] | '\u0020'| '\u000B' | '\u000A' | '\u000D') ~([;] | '\u0020' | '\u000A' | '\u000D' | '\u000B')*) ;

fragment Lca : Esc | ~ ('\'' | '\\') ;
fragment Lcb : Esc | ~ ('"' | '\\') ;
fragment Esc : '\\' ('n' | 'r' | 't' | 'b' | 'f' | '"' | '\'' | '\\' | '>' | 'u' XDIGIT XDIGIT XDIGIT XDIGIT | .) ;
fragment XDIGIT : '0' .. '9' | 'a' .. 'f' | 'A' .. 'F' ;
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
