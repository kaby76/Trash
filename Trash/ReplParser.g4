parser grammar ReplParser;

options
{
	tokenVocab = ReplLexer;
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
find : FIND StringLiteral ;
fold : FOLD StringLiteral ;
foldlit : FOLDLIT StringLiteral ;
group : GROUP StringLiteral ;
has : HAS (DR | IR) GRAPH? arg? ;
help : HELP id_keyword?;
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
