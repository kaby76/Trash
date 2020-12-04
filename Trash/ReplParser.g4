parser grammar ReplParser;

options
{
	tokenVocab = ReplLexer;
	superClass = CompatShim.Parser;
}

cmd_all
  : cmd ( PIPE cmd )* (GT arg)? EOF
  ;
cmd :
  agl
  | alias
  | analyze
  | bang
  | cat
  | cd
  | convert
  | combine
  | dot
  | delabel
  | delete
  | echo
  | empty
  | fold
  | foldlit
  | generate
  | group
  | has
  | help
  | history
  | json
  | kleene
  | ls
  | mvsr
  | parse
  | period
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
  | svg
  | text
  | ctokens
  | ctree
  | ulliteral
  | unalias
  | unfold
  | ungroup
  | unulliteral
  | version
  | workspace
  | write
  | xgrep
  | xml
  | anything
  ;
agl : AGL ;
alias : ALIAS (NonWsArgMode EQUAL (StringLiteral | NonWsArgMode))? ;
analyze : ANALYZE ;
anything : id (~(PIPE | SEMI))*  ;
bang : BANG (~(PIPE | SEMI))* ;
cat : CAT arg+ ;
cd : CD arg? ;
combine : COMBINE ;
convert : CONVERT type? ;
delabel : DELABEL ;
delete : DELETE arg ;
dot : DOT ;
echo : ECHO arg* ;
empty : ;
fold : FOLD arg ;
foldlit : FOLDLIT arg ;
generate : GENERATE arg+ ;
group : GROUP arg ;
has : HAS (DR | IR) GRAPH? arg? ;
help : HELP NonWsArgMode?;
history : HISTORY ;
json : JSON ;
kleene : KLEENE StringLiteral? ;
ls : LS arg* ;
mvsr : MVSR StringLiteral ;
parse : PARSE type? ;
period : PERIOD ;
pop : POP ;
print : PRINT ;
pwd : PWD ;
quit : (QUIT | EXIT) ;
read : READ arg ;
rename : RENAME StringLiteral StringLiteral ;
reorder : REORDER (alpha | bfs | dfs | modes) ;
rotate : ROTATE ;
rr : RR StringLiteral ;
run : RUN arg? ;
rup : RUP StringLiteral? ;
set : SET NonWsArgMode '=' (StringLiteral | INT) ;
split : SPLIT ;
st : ST ;
stack : STACK ;
svg : SVG ;
text : TEXT arg? ;
ctokens : TOKENS ;
ctree : TREE ;
ulliteral : ULLITERAL StringLiteral? ;
unalias : UNALIAS id ;
unfold : UNFOLD arg ;
ungroup : UNGROUP arg ;
unulliteral : UNULLITERAL uclc StringLiteral? ;
version : VERSION ;
workspace : WORKSPACE ;
write : WRITE ;
xgrep : XGREP arg ;
xml : XML ;
alpha : ALPHA ;
bfs : BFS StringLiteral ;
dfs : DFS StringLiteral ;
uclc : UC | LC ;
modes : MODES ;
int : INT ;
id : ID ;
arg : StringLiteral | NonWsArgMode ;
type : ANTLR4 | ANTLR3 | ANTLR2 | BISON | EBNF ;
