/*
 [The "BSD licence"]
 Copyright (c) 2004 Terence Parr and Loring Craymer
 All rights reserved.

 Redistribution and use in source and binary forms, with or without
 modification, are permitted provided that the following conditions
 are met:
 1. Redistributions of source code must retain the above copyright
    notice, this list of conditions and the following disclaimer.
 2. Redistributions in binary form must reproduce the above copyright
    notice, this list of conditions and the following disclaimer in the
    documentation and/or other materials provided with the distribution.
 3. The name of the author may not be used to endorse or promote products
    derived from this software without specific prior written permission.

 THIS SOFTWARE IS PROVIDED BY THE AUTHOR ``AS IS'' AND ANY EXPRESS OR
 IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
 OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
 IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT,
 INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
 NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
 DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
 THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
 THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

/** Python 2.3.3 Grammar
 *
 *  Terence Parr and Loring Craymer
 *  February 2004
 *
 *  This grammar was derived automatically from the Python 2.3.3
 *  parser grammar to get a syntactically correct ANTLR grammar
 *  for Python.  Then Terence hand tweaked it to be semantically
 *  correct; i.e., removed lookahead issues etc...  It is LL(1)
 *  except for the (sometimes optional) trailing commas and semi-colons.
 *  It needs two symbols of lookahead in this case.
 *
 *  Starting with Loring's preliminary lexer for Python, I modified it
 *  to do my version of the whole nasty INDENT/DEDENT issue just so I
 *  could understand the problem better.  This grammar requires
 *  PythonTokenStream.java to work.  Also I used some rules from the
 *  semi-formal grammar on the web for Python (automatically
 *  translated to ANTLR format by an ANTLR grammar, naturally <grin>).
 *  The lexical rules for python are particularly nasty and it took me
 *  a long time to get it "right"; i.e., think about it in the proper
 *  way.  Resist changing the lexer unless you've used ANTLR a lot. ;)
 *
 *  I (Terence) tested this by running it on the jython-2.1/Lib
 *  directory of 40k lines of Python.
 *
 *  REQUIRES ANTLR 2.7.3rc3 at least (to resolve a FOLLOW bug).
 */
grammar Python;

// Start symbols for the grammar:
//	single_input is a single interactive statement;
//	file_input is a module or sequence of commands read from an input file;
//	eval_input is the input for the eval() and input() functions.
// NB: compound_stmt in single_input is followed by extra NEWLINE!

single_input
    : NEWLINE
	| simple_stmt
	| compound_stmt NEWLINE
	;

file_input
    :   (NEWLINE | stmt)* EOF
	;

eval_input
    :   (NEWLINE)* testlist (NEWLINE)* EOF
	;

funcdef
    :   'def' NAME parameters COLON suite
	;

parameters
    :   LPAREN (varargslist)? RPAREN
	;

varargslist
    :   defparameter (COMMA defparameter)*
        (COMMA
            ( STAR NAME (COMMA DOUBLESTAR NAME)?
            | DOUBLESTAR NAME
            )?
        )?
    |   STAR NAME (COMMA DOUBLESTAR NAME)?
    |   DOUBLESTAR NAME
    ;

defparameter
    :   fpdef (ASSIGN test)?
    ;

fpdef
    :   NAME
	|   LPAREN fplist RPAREN
	;

fplist
    :   fpdef (COMMA fpdef)* (COMMA)?
	;


stmt: simple_stmt
	| compound_stmt
	;

simple_stmt
    :   small_stmt (SEMI small_stmt)* (SEMI)? NEWLINE
	;

small_stmt: expr_stmt
	| print_stmt
	| del_stmt
	| pass_stmt
	| flow_stmt
	| import_stmt
	| global_stmt
	| exec_stmt
	| assert_stmt
	;

expr_stmt
	:	testlist
		(	augassign testlist
		|	(ASSIGN testlist)+
		)?
	;

augassign
    : PLUSEQUAL
	| MINUSEQUAL
	| STAREQUAL
	| SLASHEQUAL
	| PERCENTEQUAL
	| AMPEREQUAL
	| VBAREQUAL
	| CIRCUMFLEXEQUAL
	| LEFTSHIFTEQUAL
	| RIGHTSHIFTEQUAL
	| DOUBLESTAREQUAL
	| DOUBLESLASHEQUAL
	;

print_stmt:
        'print'
        (   testlist
        |   RIGHTSHIFT testlist
        )?
	;

del_stmt: 'del' exprlist
	;

pass_stmt: 'pass'
	;

flow_stmt: break_stmt
	| continue_stmt
	| return_stmt
	| raise_stmt
	| yield_stmt
	;

break_stmt: 'break'
	;

continue_stmt: 'continue'
	;

return_stmt: 'return' (testlist)?
	;

yield_stmt: 'yield' testlist
	;

raise_stmt: 'raise' (test (COMMA test (COMMA test)?)?)?
	;

import_stmt
    :   'import' dotted_as_name (COMMA dotted_as_name)*
	|   'from' dotted_name 'import'
        (STAR | import_as_name (COMMA import_as_name)*)
	;

import_as_name
    :   NAME (NAME NAME)?
	;

dotted_as_name: dotted_name (NAME NAME)?
	;

dotted_name: NAME (DOT NAME)*
	;

global_stmt: 'global' NAME (COMMA NAME)*
	;

exec_stmt: 'exec' expr ('in' test (COMMA test)?)?
	;

assert_stmt: 'assert' test (COMMA test)?
	;


compound_stmt: if_stmt
	| while_stmt
	| for_stmt
	| try_stmt
	| funcdef
	| classdef
	;

if_stmt: 'if' test COLON suite ('elif' test COLON suite)* ('else' COLON suite)?
	;

while_stmt: 'while' test COLON suite ('else' COLON suite)?
	;

for_stmt: 'for' exprlist 'in' testlist COLON suite ('else' COLON suite)?
	;

try_stmt
    :   'try' COLON suite
        (   (except_clause COLON suite)+ ('else' COLON suite)?
        |   'finally' COLON suite
        )
	;

except_clause: 'except' (test (COMMA test)?)?
	;

suite: simple_stmt
	| NEWLINE INDENT (stmt)+ DEDENT
	;


test: and_test ('or' and_test)*
	| lambdef
	;

and_test
	: not_test ('and' not_test)*
	;

not_test
	: 'not' not_test
	| comparison
	;

comparison: expr (comp_op expr)*
	;

comp_op: LESS
	|GREATER
	|EQUAL
	|GREATEREQUAL
	|LESSEQUAL
	|ALT_NOTEQUAL
	|NOTEQUAL
	|'in'
	|'not' 'in'
	|'is'
	|'is' 'not'
	;

expr: xor_expr (VBAR xor_expr)*
	;

xor_expr: and_expr (CIRCUMFLEX and_expr)*
	;

and_expr: shift_expr (AMPER shift_expr)*
	;

shift_expr: arith_expr ((LEFTSHIFT|RIGHTSHIFT) arith_expr)*
	;

arith_expr: term ((PLUS|MINUS) term)*
	;

term: factor ((STAR | SLASH | PERCENT | DOUBLESLASH ) factor)*
	;

factor
	: (PLUS|MINUS|TILDE) factor
	| power
	;

power
	:   atom (trailer)* (DOUBLESTAR factor)?
	;

atom: LPAREN (testlist)? RPAREN
	| LBRACK (listmaker)? RBRACK
	| LCURLY (dictmaker)? RCURLY
	| BACKQUOTE testlist BACKQUOTE
	| NAME
	| INT
    | LONGINT
    | FLOAT
    | COMPLEX
	| (STRING)+
	;

listmaker: test ( list_for | (COMMA test)* ) (COMMA)?
	;

lambdef: 'lambda' (varargslist)? COLON test
	;

trailer: LPAREN (arglist)? RPAREN
	| LBRACK subscriptlist RBRACK
	| DOT NAME
	;

subscriptlist
    :   subscript (COMMA subscript)* (COMMA)?
	;

subscript
	: DOT DOT DOT
    | test (COLON (test)? (sliceop)?)?
    | COLON (test)? (sliceop)?
    ;

sliceop: COLON (test)?
	;

exprlist
    :   expr (COMMA expr)* (COMMA)?
	;

testlist
    :   test (COMMA test)*
        (COMMA)?
    ;

dictmaker
    :   test COLON test
        (COMMA test COLON test)* (COMMA)?
    ;

classdef: 'class' NAME (LPAREN testlist RPAREN)? COLON suite
	;

arglist: argument (COMMA argument)*
        ( COMMA
          ( STAR test (COMMA DOUBLESTAR test)?
          | DOUBLESTAR test
          )?
        )?
    |   STAR test (COMMA DOUBLESTAR test)?
    |   DOUBLESTAR test
    ;

argument : test (ASSIGN test)?
         ;

list_iter: list_for
	| list_if
	;

list_for: 'for' exprlist 'in' testlist (list_iter)?
	;

list_if: 'if' test (list_iter)?
	;

LPAREN	: '(' ;

RPAREN	: ')' ;

LBRACK	: '[' ;

RBRACK	: ']' ;

COLON 	: ':' ;

COMMA	: ',' ;

SEMI	: ';' ;

PLUS	: '+' ;

MINUS	: '-' ;

STAR	: '*' ;

SLASH	: '/' ;

VBAR	: '|' ;

AMPER	: '&' ;

LESS	: '<' ;

GREATER	: '>' ;

ASSIGN	: '=' ;

PERCENT	: '%' ;

BACKQUOTE	: '`' ;

LCURLY	: '{' ;

RCURLY	: '}' ;

CIRCUMFLEX	: '^' ;

TILDE	: '~' ;

EQUAL	: '==' ;

NOTEQUAL	: '!=' ;

ALT_NOTEQUAL: '<>' ;

LESSEQUAL	: '<=' ;

LEFTSHIFT	: '<<' ;

GREATEREQUAL	: '>=' ;

RIGHTSHIFT	: '>>' ;

PLUSEQUAL	: '+=' ;

MINUSEQUAL	: '-=' ;

DOUBLESTAR	: '**' ;

STAREQUAL	: '*=' ;

DOUBLESLASH	: '//' ;

SLASHEQUAL	: '/=' ;

VBAREQUAL	: '|=' ;

PERCENTEQUAL	: '%=' ;

AMPEREQUAL	: '&=' ;

CIRCUMFLEXEQUAL	: '^=' ;

LEFTSHIFTEQUAL	: '<<=' ;

RIGHTSHIFTEQUAL	: '>>=' ;

DOUBLESTAREQUAL	: '**=' ;

DOUBLESLASHEQUAL	: '//=' ;

NUMBER
	:   // Hex
        '0' ('x' | 'X') ( '0' .. '9' | 'a' .. 'f' | 'A' .. 'F' )+
		('l' | 'L')?

    |   // Octal
        '0' Int
		(   FloatTrailer ('j' | 'J')?
        |   ('l' | 'L')
        )?

    |   '0'
		(   FloatTrailer ('j' | 'J')?
        |   ('l' | 'L')
        )?

    |   // Int or float
        (	NonZeroDigit (Int)?
			(	('l' | 'L')
			|	('j' | 'J')
			|	FloatTrailer ('j' | 'J')?
			|
			)
		)
	|	'.' Int (Exponent)? ('j' | 'J')?
    |   '.' // DOT (non number; e.g., field access)
	;
Int : ( '0' .. '9' )+ ;
NonZeroDigit : '1' .. '9' ;
FloatTrailer
	:	'.'
	|	'.' Int (Exponent)?
	|   Exponent
	;
Exponent
	:	('e' | 'E') ( '+' | '-' )? Int
	;
Name
    :	( 'a' .. 'z' | 'A' .. 'Z' | '_')
        ( 'a' .. 'z' | 'A' .. 'Z' | '_' | '0' .. '9' )*
    ;

STRING_OR_NAME
    :   String
    |Name
        (   {prefix.equals("R")||prefix.equals("U")||prefix.equals("UR")}?
            String
        |
        )
	;
String
    :   '\'\'\'' (ESC|NEWLINE|.)* '\'\'\''
	|   '"' '"' '"' (ESC|NEWLINE|.)* '"' '"' '"'
    |   '\'' (ESC|~('\\'|'\n'|'\''))* '\''
	|   '"' (ESC|~('\\'|'\n'|'"'))* '"'
	;
ESC
	:	'\\' .
	;

/** Consume a newline and any whitespace at start of next line */
CONTINUED_LINE
	:	'\\' ('\r')? '\n' (' '|'\t')*
	;

/** Grab everything before a real symbol.  Then if newline, kill it
 *  as this is a blank line.  If whitespace followed by comment, kill it
 *  as it's a comment on a line by itself.
 *
 *  Ignore leading whitespace when nested in [..], (..), {..}.
 */
LEADING_WS
    :   {getColumn()==1}?
        // match spaces or tabs, tracking indentation count
        ( 	' '
        |	'\t'
        |   '\014' // formfeed is ok
        )+

        // kill trailing newline or comment
        (   {implicitLineJoiningLevel==0}? ('\r')? '\n'

        |   // if comment, then only thing on a line; kill so we
            // ignore totally also wack any following newlines as
            // they cannot be terminating a statement
            '#' (~'\n')* ('\n')+
        )?
    ;

/** Comments not on line by themselves are turned into newlines because
    sometimes they are newlines like

    b = a # end of line comment

    or

    a = [1, # weird
         2]

    This rule is invoked directly by nextToken when the comment is in
    first column or when comment is on end of nonwhitespace line.

    The problem is that then we have lots of newlines heading to
    the parser.  To fix that, column==1 implies we should kill whole line.

    Consume any newlines following this comment as they are not statement
    terminators.  Don't let NEWLINE token handle them.
 */

COMMENT
    :   '#' (~'\n')*
        ( {startCol==1}? ('\n')+ )?
    ;

/** Treat a sequence of blank lines as a single blank line.  If
 *  nested within a (..), {..}, or [..], then ignore newlines.
 *  If the first newline starts in column one, they are to be ignored.
 */
NEWLINE
    :   (('\r')? '\n')+
    ;

WS  :   (' '|'\t')+
    ;
