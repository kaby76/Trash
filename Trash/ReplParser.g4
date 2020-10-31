parser grammar ReplParser;

options
{
	tokenVocab = ReplLexer;
	superClass = CompatShim.Parser;
}

cmd_all
  : cmd ( PIPE cmd )* EOF
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
  | st
  | stack
  | text
  | ulliteral
  | unalias
  | unfold
  | ungroup
  | unulliteral
  | workspace
  | write
  | anything
  ;
alias : ALIAS (NonWsArgMode EQUAL (StringLiteral | NonWsArgMode))? ;
analyze : ANALYZE ;
anything : id  .*  ;
bang : BANG .* ;
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
help : HELP NonWsArgMode?;
history : HISTORY ;
kleene : KLEENE StringLiteral? ;
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
set : SET NonWsArgMode '=' (StringLiteral | INT) ;
split : SPLIT ;
st : ST ;
stack : STACK ;
text : TEXT ;
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
arg : StringLiteral | NonWsArgMode ;
type : ANTLR4 | ANTLR3 | ANTLR2 | BISON | EBNF ;
