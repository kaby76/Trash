/*
 [The "BSD licence"]
 Copyright (c) 2005-2007 Terence Parr
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

/** ANTLR v3 grammar written in ANTLR v3 with AST construction */
grammar ANTLRv3;

tokens {
	DOC_COMMENT,
	PARSER,	
    LEXER,
    RULE,
    BLOCK,
    OPTIONAL,
    CLOSURE,
    POSITIVE_CLOSURE,
    SYNPRED,
    RANGE,
    CHAR_RANGE,
    EPSILON,
    ALT,
    EOR,
    EOB,
    EOA, // end of alt
    ID,
    ARG,
    ARGLIST,
    RET,
    LEXER_GRAMMAR,
    PARSER_GRAMMAR,
    TREE_GRAMMAR,
    COMBINED_GRAMMAR,
    INITACTION,
    LABEL, // $x used in rewrite rules
    TEMPLATE,
    SCOPE,
    SEMPRED,
    GATED_SEMPRED, // {p}? =>
    SYN_SEMPRED, // (...) =>   it's a manually-specified synpred converted to sempred
    BACKTRACK_SEMPRED, // auto backtracking mode syn pred converted to sempred
    FRAGMENT,
    TREE_BEGIN,
    ROOT,
    BANG,
    RANGE,
    REWRITE
}

@members {
	int gtype;
}

grammarDef
    :   DOC_COMMENT?
    	(	'lexer'  {gtype=LEXER_GRAMMAR;}    // pure lexer
    	|   'parser' {gtype=PARSER_GRAMMAR;}   // pure parser
    	|   'tree'   {gtype=TREE_GRAMMAR;}     // a tree parser
    	|		     {gtype=COMBINED_GRAMMAR;} // merged parser/lexer
    	)
    	g='grammar' id ';' optionsSpec? tokensSpec? attrScope* action*
    	rule+
    	EOF
    ;

tokensSpec
	:	TOKENS tokenSpec+ '}'
	;

tokenSpec
	:	TOKEN_REF
		(	'=' (lit=STRING_LITERAL|lit=CHAR_LITERAL)
		|
		)
		';'
	;

attrScope
	:	'scope' id ACTION
	;

/** Match stuff like @parser::members {int i;} */
action
	:	'@' (actionScopeName '::')? id ACTION
	;

/** Sometimes the scope names will collide with keywords; allow them as
 *  ids for action scopes.
 */
actionScopeName
	:	id
	|	l='lexer'
    |   p='parser'
	;

optionsSpec
	:	OPTIONS (option ';')+ '}'
	;

option
    :   id '=' optionValue
 	;
 	
optionValue
    :   id
    |   STRING_LITERAL
    |   CHAR_LITERAL
    |   INT
    |	s='*'  // used for k=*
    ;

rule
	:	DOC_COMMENT?
		( modifier=('protected'|'public'|'private'|'fragment') )?
		id {$rule::name = $id.text;}
		'!'?
		( arg=ARG_ACTION )?
		( 'returns' rt=ARG_ACTION  )?
		throwsSpec? optionsSpec? ruleScopeSpec? ruleAction*
		':'	altList	';'
		exceptionGroup?
	;

/** Match stuff like @init {int i;} */
ruleAction
	:	'@' id ACTION
	;

throwsSpec
	:	'throws' id ( ',' id )*
	;

ruleScopeSpec
	:	'scope' ACTION
	|	'scope' id (',' id)* ';'
	|	'scope' ACTION
		'scope' id (',' id)* ';'
	;

block
    :   lp='('
		( (opts=optionsSpec)? ':' )?
		a1=alternative rewrite ( '|' a2=alternative rewrite )*
        rp=')'
    ;

altList
@init {
	// must create root manually as it's used by invoked rules in real antlr tool.
	// leave here to demonstrate use of {...} in rewrite rule
	// it's really BLOCK[firstToken,"BLOCK"]; set line/col to previous ( or : token.
    CommonTree blkRoot = (CommonTree)adaptor.create(BLOCK,input.LT(-1),"BLOCK");
}
    :   a1=alternative rewrite ( '|' a2=alternative rewrite )*
    ;

alternative
@init {
	Token firstToken = input.LT(1);
	Token prevToken = input.LT(-1); // either : or | I think
}
    :   element+
    |
    ;

exceptionGroup
	:	( exceptionHandler )+ ( finallyClause )?
	|	finallyClause
    ;

exceptionHandler
    :    'catch' ARG_ACTION ACTION
    ;

finallyClause
    :    'finally' ACTION
    ;

element
	:	elementNoOptionSpec
	;

elementNoOptionSpec
	:	id (labelOp='='|labelOp='+=') atom
		(	ebnfSuffix
		|
		)
	|	id (labelOp='='|labelOp='+=') block
		(	ebnfSuffix
		|
		)
	|	atom
		(	ebnfSuffix
		|
		)
	|	ebnf
	|   ACTION
	|   SEMPRED ( '=>' | )
	|   treeSpec
		(	ebnfSuffix
		|
		)
	;

atom:   range ( (op='^'|op='!') | )
    |   terminal
    |	notSet ( (op='^'|op='!') | )
    |   RULE_REF ( arg=ARG_ACTION )? ( (op='^'|op='!') )?
    ;

notSet
	:	'~'
		(	notTerminal
		|	block
		)
	;

treeSpec
	:	'^(' element ( element )+ ')'
	;

/** Matches ENBF blocks (and token sets via block rule) */
ebnf
@init {
    Token firstToken = input.LT(1);
}
@after {
	$ebnf.tree.getToken().setLine(firstToken.getLine());
	$ebnf.tree.getToken().setCharPositionInLine(firstToken.getCharPositionInLine());
}
	:	block
		(	op='?'
		|	op='*'
		|	op='+'
		|   '=>'
        |
		)
	;

range
	:	c1=CHAR_LITERAL RANGE c2=CHAR_LITERAL
	;

terminal
    :   (	CHAR_LITERAL
    		// Args are only valid for lexer rules
		|   TOKEN_REF
			( ARG_ACTION
			|
			)
		|   STRING_LITERAL
		|   '.'
		)	
		(	'^'
		|	'!'
		)?
	;

notTerminal
	:   CHAR_LITERAL
	|	TOKEN_REF
	|	STRING_LITERAL
	;
	
ebnfSuffix
@init {
	Token op = input.LT(1);
}
	:	'?'
  	|	'*'
   	|	'+'
	;
	


// R E W R I T E  S Y N T A X

rewrite
@init {
	Token firstToken = input.LT(1);
}
	:	(rew+='->' preds+=SEMPRED predicated+=rewrite_alternative)*
		rew2='->' last=rewrite_alternative
	|
	;

rewrite_alternative
	:	rewrite_template
	|	rewrite_tree_alternative
   	|
	;
	
rewrite_tree_block
    :   lp='(' rewrite_tree_alternative ')'
    ;

rewrite_tree_alternative
    :	rewrite_tree_element+
    ;

rewrite_tree_element
	:	rewrite_tree_atom
	|	rewrite_tree_atom ebnfSuffix
	|   rewrite_tree
		(	ebnfSuffix
		|
		)
	|   rewrite_tree_ebnf
	;

rewrite_tree_atom
    :   CHAR_LITERAL
	|   TOKEN_REF ARG_ACTION? // for imaginary nodes
    |   RULE_REF
	|   STRING_LITERAL
	|   d='$' id // reference to a label in a rewrite rule
	|	ACTION
	;

rewrite_tree_ebnf
@init {
    Token firstToken = input.LT(1);
}
@after {
	$rewrite_tree_ebnf.tree.getToken().setLine(firstToken.getLine());
	$rewrite_tree_ebnf.tree.getToken().setCharPositionInLine(firstToken.getCharPositionInLine());
}
	:	rewrite_tree_block ebnfSuffix
	;
	
rewrite_tree
	:	'^(' rewrite_tree_atom rewrite_tree_element* ')'
	;

/** Build a tree for a template rewrite:
      ^(TEMPLATE (ID|ACTION) ^(ARGLIST ^(ARG ID ACTION) ...) )
    where ARGLIST is always there even if no args exist.
    ID can be "template" keyword.  If first child is ACTION then it's
    an indirect template ref

    -> foo(a={...}, b={...})
    -> ({string-e})(a={...}, b={...})  // e evaluates to template name
    -> {%{$ID.text}} // create literal template from string (done in ActionTranslator)
	-> {st-expr} // st-expr evaluates to ST
 */
rewrite_template
	:   // -> template(a={...},...) "..."    inline template
		id lp='(' rewrite_template_args	')'
		( str=DOUBLE_QUOTE_STRING_LITERAL | str=DOUBLE_ANGLE_STRING_LITERAL )

	|	// -> foo(a={...}, ...)
		rewrite_template_ref

	|	// -> ({expr})(a={...}, ...)
		rewrite_indirect_template_head

	|	// -> {...}
		ACTION
	;

/** -> foo(a={...}, ...) */
rewrite_template_ref
	:	id lp='(' rewrite_template_args	')'
	;

/** -> ({expr})(a={...}, ...) */
rewrite_indirect_template_head
	:	lp='(' ACTION ')' '(' rewrite_template_args ')'
	;

rewrite_template_args
	:	rewrite_template_arg (',' rewrite_template_arg)*
	|
	;

rewrite_template_arg
	:   id '=' ACTION
	;

id	:	TOKEN_REF
	|	RULE_REF
	;

// L E X I C A L   R U L E S

SL_COMMENT
 	:	'//'
 	 	(	' $ANTLR ' SRC // src directive
 		|	~('\r'|'\n')*
		)
		'\r'? '\n'
		{$channel=HIDDEN;}
	;

ML_COMMENT
	:	'/*' {if (input.LA(1)=='*') $type=DOC_COMMENT; else $channel=HIDDEN;} .* '*/'
	;

CHAR_LITERAL
	:	'\'' LITERAL_CHAR '\''
	;

STRING_LITERAL
	:	'\'' LITERAL_CHAR LITERAL_CHAR* '\''
	;

fragment
LITERAL_CHAR
	:	ESC
	|	~('\''|'\\')
	;

DOUBLE_QUOTE_STRING_LITERAL
	:	'"' (ESC | ~('\\'|'"'))* '"'
	;

DOUBLE_ANGLE_STRING_LITERAL
	:	'<<' .* '>>'
	;

fragment
ESC	:	'\\'
		(	'n'
		|	'r'
		|	't'
		|	'b'
		|	'f'
		|	'"'
		|	'\''
		|	'\\'
		|	'>'
		|	'u' XDIGIT XDIGIT XDIGIT XDIGIT
		|	. // unknown, leave as it is
		)
	;

fragment
XDIGIT :
		'0' .. '9'
	|	'a' .. 'f'
	|	'A' .. 'F'
	;

INT	:	'0'..'9'+
	;

ARG_ACTION
	:	NESTED_ARG_ACTION
	;

fragment
NESTED_ARG_ACTION :
	'['
	(	NESTED_ARG_ACTION
	|	ACTION_STRING_LITERAL
	|	ACTION_CHAR_LITERAL
	|	.
	) * ?
	']'
	;

ACTION
	:	NESTED_ACTION ( '?' {$type = SEMPRED;} )?
	;

fragment
NESTED_ACTION :
	'{'
	(	NESTED_ACTION
	|	SL_COMMENT
	|	ML_COMMENT
	|	ACTION_STRING_LITERAL
	|	ACTION_CHAR_LITERAL
	|	.
	) * ?
	'}'
   ;

fragment
ACTION_CHAR_LITERAL
	:	'\'' (ACTION_ESC|~('\\'|'\'')) '\''
	;

fragment
ACTION_STRING_LITERAL
	:	'"' (ACTION_ESC|~('\\'|'"'))* '"'
	;

fragment
ACTION_ESC
	:	'\\\''
	|	'\\' '"' // ANTLR doesn't like: '\\"'
	|	'\\' ~('\''|'"')
	;

TOKEN_REF
	:	'A'..'Z' ('a'..'z'|'A'..'Z'|'_'|'0'..'9')*
	;

RULE_REF
	:	'a'..'z' ('a'..'z'|'A'..'Z'|'_'|'0'..'9')*
	;

/** Match the start of an options section.  Don't allow normal
 *  action processing on the {...} as it's not a action.
 */
OPTIONS
	:	'options' WS_LOOP '{'
	;
	
TOKENS
	:	'tokens' WS_LOOP '{'
	;

/** Reset the file and line information; useful when the grammar
 *  has been generated so that errors are shown relative to the
 *  original file like the old C preprocessor used to do.
 */
fragment
SRC	:	'src' ' 'ACTION_STRING_LITERAL ' 'INT
	;

WS	:	(	' '
		|	'\t'
		|	'\r'? '\n'
		)+
		{$channel=HIDDEN;}
	;

fragment
WS_LOOP
	:	(	WS
		|	SL_COMMENT
		|	ML_COMMENT
		)*
	;



SCOPE : 'scope' ;

FRAGMENT : 'fragment' ;

TREE_BEGIN : '^(' ;

ROOT : '^' ;

BANG : '!' ;

RANGE : '..' ;

REWRITE : '->' ;