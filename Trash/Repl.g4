grammar Repl;

options
{
	superClass = CompatShim.Parser;
}

cmd_all
  : cmd EOF
  ;
cmd :
  alias
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
  | help
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
  | ulliteral
  | unalias
  | unfold
  | ungroup
  | unulliteral
  | workspace
  | write
  | anything
  ;
alias : ALIAS (ID EQUAL (StringLiteral | id_keyword))? ;
analyze : ANALYZE ;
anything : id  .*  ;
bang : BANG (BANG | int | id_keyword) ;
cd : CD arg? ;
combine : COMBINE ;
convert : CONVERT type? ;
delabel : DELABEL ;
delete : DELETE arg ;
dot : DOT ;
empty : ;
find : FIND arg ;
fold : FOLD arg ;
foldlit : FOLDLIT arg ;
group : GROUP arg ;
has : HAS (DR | IR) GRAPH? arg? ;
help : HELP id_keyword?;
history : HISTORY ;
kleene : KLEENE arg? ;
ls : LS arg*  ;
mvsr : MVSR StringLiteral ;
parse : PARSE type? ;
pop : POP ;
print : PRINT ;
pwd : PWD ;
quit : (QUIT | EXIT) ;
read : READ arg ;
rename : RENAME StringLiteral StringLiteral ;
reorder : REORDER (alpha | bfs | dfs | modes) ;
rotate : ROTATE ;
rr : RR StringLiteral ;
run : RUN arg (arg (arg)? )? ;
rup : RUP StringLiteral? ;
set : SET id_keyword '=' (StringLiteral | INT) ;
split : SPLIT ;
stack : STACK ;
ulliteral : ULLITERAL StringLiteral? ;
unalias : UNALIAS id ;
unfold : UNFOLD arg ;
ungroup : UNGROUP arg ;
unulliteral : UNULLITERAL uclc StringLiteral? ;
workspace : WORKSPACE ;
write : WRITE ;
alpha : ALPHA ;
bfs : BFS StringLiteral ;
dfs : DFS StringLiteral ;
uclc : UC | LC ;
modes : MODES ;
int : INT ;
id : ID ;
arg : (StringLiteral | id_keyword | NonWs | DOT) ;
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
  | HELP
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
  | UC
  | LC
  ;
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
HELP : 'help' ;
HISTORY : 'history' ;
INT : [0-9]+ ;
IR : 'ir' ;
LC : 'lc' ;
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
UC : 'uc' ;
ULLITERAL : 'ulliteral' ;
UNALIAS : 'unalias' ;
UNFOLD : 'unfold' ;
UNGROUP : 'ungroup' ;
UNULLITERAL : 'unulliteral' ;
WORKSPACE : 'workspace' ;
WRITE : 'write' ;
ID: Id ;
BLOCK_COMMENT : BlockComment -> skip ;
LINE_COMMENT : LineComment -> skip ;
HWS : Hws -> skip ;
VWS : Vws -> skip ;
NonWs : ( ~('\u0020' | '\u000B' | '\u000A' | '\u000D') )+ ;
STUFF : . ;

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
