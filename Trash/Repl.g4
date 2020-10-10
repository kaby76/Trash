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
  | delabel
  | delete
  | empty
  | find
  | fold
  | foldlit
  | group
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
  | run
  | rup
  | set
  | split
  | stack
  | unalias
  | unfold
  | ungroup
  | ulliteral
  | workspace
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
convert : CONVERT HWS* type? ;
delabel : DELABEL HWS* ;
delete : DELETE HWS* arg ;
dot : DOT ;
empty : ;
find : FIND HWS* arg ;
fold : FOLD HWS* arg ;
foldlit : FOLDLIT HWS* arg ;
group : GROUP HWS* arg ;
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
run : RUN HWS* arg (HWS* arg (HWS* arg)? )? ;
rup : RUP HWS* StringLiteral? ;
set : SET HWS* id_keyword HWS* '=' HWS* (StringLiteral | INT) ;
split : SPLIT ;
stack : STACK ;
ulliteral : ULLITERAL HWS* StringLiteral? ;
unalias : UNALIAS HWS* id ;
unfold : UNFOLD HWS* arg ;
ungroup : UNGROUP HWS* arg ;
workspace : WORKSPACE ;
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
  | DELABEL
  | DELETE
  | DR
  | EXIT
  | FIND
  | FOLD
  | FOLDLIT
  | GRAPH
  | GROUP
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
  | RUN
  | RUP
  | SET
  | SPLIT
  | STACK
  | ULLITERAL
  | UNALIAS
  | UNFOLD
  | UNGROUP
  | WORKSPACE
  | WRITE
  ;
stuff : STUFF | id_keyword ;
type : ANTLR4 | ANTLR3 | ANTLR2 | BISON | EBNF ;

ALPHA : 'alpha' ;
ALIAS : 'alias' ;
ANALYZE : 'analyze';
ANTLR2 : 'antlr2' ;
ANTLR3 : 'antlr3' ;
ANTLR4 : 'antlr4' ;
BANG : '!' ;
BFS : 'bfs' ;
BISON : 'bison' ;
BUILD : 'build' ;
CD : 'cd' ;
COMBINE : 'combine' ;
CONVERT : 'convert' ;
DELABEL : 'delabel' ;
DELETE : 'delete' ;
DFS : 'dfs' ;
DR : 'dr' ;
DOT : '.' ;
EBNF : 'ebnf' ;
EQUAL : '=' ;
EXIT : 'exit' ;
FIND : 'find' ;
FOLD : 'fold' ;
FOLDLIT : 'foldlit' ;
GRAPH : 'graph' ;
GROUP : 'group' ;
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
RUN : 'run' ;
RUP : 'rup' ;
SEMI : ';' ;
SET : 'set' ;
SPLIT : 'split' ;
STACK : 'stack' ;
StringLiteral : ('\'' Lca Lca* '\'') | ('"' Lcb Lcb* '"') ;
ULLITERAL : 'ulliteral' ;
UNALIAS : 'unalias' ;
UNFOLD : 'unfold' ;
UNGROUP : 'ungroup' ;
WORKSPACE : 'workspace' ;
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
