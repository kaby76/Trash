grammar Abnf;

rulelist : (rule_ | (WSP* c_nl))+ EOF ; // Note errata.
rule_ : rulename defined_as elements c_nl ;
rulename : ID ;
defined_as : c_wsp* ('=' | '=/') c_wsp* ;
elements : alternation WSP* ; // Note errata.
c_wsp : WSP | c_nl WSP ;
c_nl : COMMENT | CRLF ;
COMMENT : ';' ~( '\n' | '\r' )* CRLF ;
alternation : concatenation ( c_wsp* '/' c_wsp* concatenation )* ;
concatenation : repetition (c_wsp+ repetition)* ;
repetition : repeat_? element ;
repeat_ : INT | ( INT? '*' INT? ) ;
INT : DIGIT+ ;
fragment DIGIT : '0' .. '9' ;
element : rulename | group | option | char_val | num_val | prose_val ;
group : '(' c_wsp* alternation c_wsp* ')' ;
option : '[' c_wsp* alternation c_wsp* ']' ;
char_val : STRING ;
STRING : '"' ( ~('"' | '\n' | '\r') )* '"' ;
num_val : NumberValue ;
NumberValue : '%' ( BinaryValue | DecimalValue | HexValue ) ;
fragment BinaryValue : 'b' BIT+ ( ( '.' BIT+ )+ | ( '-' BIT+ ) )? ;
fragment DecimalValue : 'd' DIGIT+ ( ( '.' DIGIT+ )+ | ( '-' DIGIT+ ) )? ;
fragment HexValue : 'x' HEX_DIGIT+ ( ( '.' HEX_DIGIT+ )+ | ( '-' HEX_DIGIT+ ) )? ;
prose_val : ProseValue ;
ProseValue : '<' ( ~ '>' )* '>' ;
ID : LETTER ( LETTER | DIGIT | '-' )* ;
//WS : ( ' ' | '\t' | '\r' | '\n' ) -> channel(HIDDEN) ;
fragment LETTER : 'a' .. 'z' | 'A' .. 'Z' ;
fragment BIT : '0' .. '1' ;
fragment HEX_DIGIT : ( '0' .. '9' | 'a' .. 'f' | 'A' .. 'F' ) ;

WSP : SP | HTAB ;
fragment SP : '\u0020' ;
fragment HTAB : '\u0009' ;
fragment LF : '\u000A' ;
fragment CR : '\u000D' ;
CRLF : (CR LF | CR | LF) ;
