/*
 [The "BSD license"]
 Copyright (c) 2005-2011 Terence Parr
 All rights reserved.

 Grammar conversion to ANTLR v3:
 Copyright (c) 2011 Sam Harwell
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

/** Read in an ANTLR grammar and build an AST.  Try not to do
 *  any actions, just build the tree.
 *
 *  The phases are:
 *
 *      antlr.g (this file)
 *      assign.types.g
 *      define.g
 *      buildnfa.g
 *      antlr.print.g (optional)
 *      codegen.g
 *
 *  Terence Parr
 *  University of San Francisco
 *  2005
 */

grammar ANTLR;

options
{
    language=Java;
}

tokens
{
    //OPTIONS='options';
    //TOKENS='tokens';
    LEXER,
    PARSER,
    CATCH,
    FINALLY,
    GRAMMAR,
    PRIVATE,
    PROTECTED,
    PUBLIC,
    RETURNS,
    THROWS,
    TREE,

    RULE,
    PREC_RULE,
    RECURSIVE_RULE_REF, // flip recursive RULE_REF to RECURSIVE_RULE_REF in prec rules
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
    FORCED_ACTION, // {{...}} always exec even during syn preds
    LABEL, // $x used in rewrite rules
    TEMPLATE,
    SCOPE,
    IMPORT,
    GATED_SEMPRED, // {p}? =>
    SYN_SEMPRED, // (...) =>   it's a manually-specified synpred converted to sempred
    BACKTRACK_SEMPRED, // auto backtracking mode syn pred converted to sempred
    FRAGMENT,
    DOT,
    REWRITES
}

// Token string literals converted to explicit lexer rules.
// Reorder these rules accordingly.

LEXER: 'lexer';
PARSER: 'parser';
CATCH: 'catch';
FINALLY: 'finally';
GRAMMAR: 'grammar';
PRIVATE: 'private';
PROTECTED: 'protected';
PUBLIC: 'public';
RETURNS: 'returns';
THROWS: 'throws';
TREE: 'tree';
SCOPE: 'scope';
IMPORT: 'import';
FRAGMENT: 'fragment';
//




grammar_
    :   //hdr:headerSpec
        ( ACTION )?
        (DOC_COMMENT  )?grammarTypeid SEMI
        (   optionsSpec
        )?
        (delegateGrammars)?
        (tokensSpec)?attrScopes
        (actions)?rules
        EOF
    ;

grammarType
    :   (   'lexer''grammar'
        |   'parser''grammar'
        |   'tree''grammar'
        |'grammar'
        )
    ;

actions
    :   (action)+
    ;

/** Match stuff like @parser::members {int i;} */
action
    :   AMPERSAND (actionScopeName COLON COLON)? id ACTION
    ;

/** Sometimes the scope names will collide with keywords; allow them as
 *  ids for action scopes.
 */
actionScopeName
    :   id
    |'lexer'
    |'parser'
    ;

optionsSpec returns
    :   OPTIONS (option SEMI)+ RCURLY
    ;

option
    :   id ASSIGN optionValue
    ;

optionValue returns
    :id
    |STRING_LITERAL
    |CHAR_LITERAL
    |INT
    |STAR
//  |   cs:charSet       {value = #cs;} // return set AST in this case
    ;

delegateGrammars
    :   'import' delegateGrammar (COMMA delegateGrammar)* SEMI
    ;

delegateGrammar
    :id ASSIGNid
    |id
    ;

tokensSpec
    :   TOKENS
            tokenSpec*
        RCURLY
    ;

tokenSpec
    :   TOKEN_REF ( ASSIGN (STRING_LITERAL|CHAR_LITERAL) )? SEMI
    ;

attrScopes
    :   (attrScope)*
    ;

attrScope
    :   'scope' id ruleActions? ACTION
    ;

rules
    :   (   rule
        )+
    ;


rule
    :
    (   (DOC_COMMENT
        )?
        ('protected'  //{modifier=$p1.tree;}
        |'public'     //{modifier=$p2.tree;}
        |'private'    //{modifier=$p3.tree;}
        |'fragment'   //{modifier=$p4.tree;}
        )?id
        ( BANG )?
        (ARG_ACTION )?
        ( 'returns'ARG_ACTION  )?
        ( throwsSpec )?
        ( optionsSpec )?ruleScopeSpec
        (ruleActions)?
        COLON
        ruleAltList
        SEMI
        (exceptionGroup )?
    )
    ;

ruleActions
    :   (ruleAction)+
    ;

/** Match stuff like @init {int i;} */
ruleAction
    :   AMPERSAND id ACTION
    ;

throwsSpec
    :   'throws' id ( COMMA id )*
    ;

ruleScopeSpec
    :   ( 'scope' ruleActions? ACTION )?
        ( 'scope' idList SEMI )*
    ;

ruleAltList
    :
        (alternativerewrite
        )
        (   (   ORalternativerewrite
            )+
        |
        )
    ;
finally

/** Build #(BLOCK ( #(ALT ...) EOB )+ ) */
block
    :   (LPAREN
        )
        (
            // 2nd alt and optional branch ambig due to
            // linear approx LL(2) issue.  COLON ACTION
            // matched correctly in 2nd alt.
            (optionsSpec)?
            ( ruleActions )?
            COLON
        |   ACTION COLON
        )?alternativerewrite
        (   ORalternativerewrite
        )*RPAREN
    ;
finally

// ALT and EOA have indexes tracking start/stop of entire alt
alternative
    :   element+
    |
    ;

exceptionGroup
    :   exceptionHandler+ finallyClause?
    |   finallyClause
    ;

exceptionHandler
    :   'catch' ARG_ACTION ACTION
    ;

finallyClause
    :   'finally' ACTION
    ;

element
    :   elementNoOptionSpec
    ;

elementNoOptionSpec
    :   (   id (ASSIGN|PLUS_ASSIGN)
            (   atom (ebnfSuffix)?
            |   ebnf
            )
        |atom
            (ebnfSuffix
            )?
        |   ebnf
        |   FORCED_ACTION
        |   ACTION
        |SEMPRED ( IMPLIES )?
        |tree_
        )
    ;

atom
    :   range (ROOT|BANG)?
    |   (
            idWILDCARD (terminal|ruleref)
        |   terminal
        |   ruleref
        )
    |   notSet (ROOT|BANG)?
    ;

ruleref
    :   RULE_REF ARG_ACTION? (ROOT|BANG)?
    ;

notSet
    :   NOT
        (   notTerminal
        |   block
        )
    ;

treeRoot
    :   id (ASSIGN|PLUS_ASSIGN) (atom|block)
    |   atom
    |   block
    ;

tree_
    :   TREE_BEGIN
        treeRoot element+
        RPAREN
    ;

/** matches ENBF blocks (and sets via block rule) */
ebnf
    :   block
        (   QUESTION
        |   STAR
        |   PLUS
        |   IMPLIES
        |   ROOT
        |   BANG
        |
        )
    ;

range
    : CHAR_LITERAL RANGECHAR_LITERAL
    |   // range elsewhere is an error
        (TOKEN_REFRANGE TOKEN_REF
        |STRING_LITERALRANGE STRING_LITERAL
        |CHAR_LITERALRANGE CHAR_LITERAL
        ) // have to generate something for surrounding code, just return first token
    ;

terminal
    :CHAR_LITERAL ( elementOptions )? (ROOT|BANG)?

    |TOKEN_REF
        ( elementOptions )?
        ( ARG_ACTION )? // Args are only valid for lexer rules
        (ROOT|BANG)?

    |STRING_LITERAL ( elementOptions )? (ROOT|BANG)?

    |WILDCARD (ROOT|BANG)?
    ;

elementOptions
    :   OPEN_ELEMENT_OPTION defaultNodeOption CLOSE_ELEMENT_OPTION
    |   OPEN_ELEMENT_OPTION elementOption (SEMI elementOption)* CLOSE_ELEMENT_OPTION
    ;

defaultNodeOption
    :   elementOptionId
    ;

elementOption
    :   id ASSIGN
        (   elementOptionId
        |   (STRING_LITERAL|DOUBLE_QUOTE_STRING_LITERAL|DOUBLE_ANGLE_STRING_LITERAL)
        )
    ;

elementOptionId returns
    :id ('.'id)*
    ;

ebnfSuffix
    :
        (   QUESTION
        |   STAR
        |   PLUS
        )
    ;

notTerminal
    :   CHAR_LITERAL
    |   TOKEN_REF
    |   STRING_LITERAL
    ;

idList
    :   id (COMMA id)*
    ;

id
    :   TOKEN_REF
    |   RULE_REF
    ;

// R E W R I T E  S Y N T A X

rewrite
    :   rewrite_with_sempred*
        REWRITE rewrite_alternative
    |
    ;

rewrite_with_sempred
    :   REWRITE SEMPRED rewrite_alternative
    ;

rewrite_block
    :   LPAREN
        rewrite_alternative
        RPAREN
    ;

rewrite_alternative
options{k=1;}
    :  rewrite_template

    |  ( rewrite_element )+

    |
    | ETC
    ;

rewrite_element
    :   (rewrite_atom
        )
        (ebnfSuffix
        )?
    |   rewrite_ebnf
    |   (rewrite_tree
        )
        (ebnfSuffix
        )?
    ;

rewrite_atom
    :TOKEN_REF elementOptions? ARG_ACTION? // for imaginary nodes
    |   RULE_REF
    |CHAR_LITERAL elementOptions?
    |STRING_LITERAL elementOptions?
    |   DOLLAR label // reference to a label in a rewrite rule
    |   ACTION
    ;

label
    :   TOKEN_REF
    |   RULE_REF
    ;

rewrite_ebnf
    :rewrite_block
        (   QUESTION
        |   STAR
        |   PLUS
        )
    ;

rewrite_tree
    :   TREE_BEGIN
            rewrite_atom rewrite_element*
        RPAREN
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
options{k=1;}
    :  // inline
        (   rewrite_template_head
        )
        (DOUBLE_QUOTE_STRING_LITERAL |DOUBLE_ANGLE_STRING_LITERAL )

    |   // -> foo(a={...}, ...)
        rewrite_template_head

    |   // -> ({expr})(a={...}, ...)
        rewrite_indirect_template_head

    |   // -> {...}
        ACTION
    ;

/** -> foo(a={...}, ...) */
rewrite_template_head
    :   idLPAREN
        rewrite_template_args
        RPAREN
    ;

/** -> ({expr})(a={...}, ...) */
rewrite_indirect_template_head
    :LPAREN
        ACTION
        RPAREN
        LPAREN rewrite_template_args RPAREN
    ;

rewrite_template_args
    :   rewrite_template_arg (COMMA rewrite_template_arg)*
    |
    ;

rewrite_template_arg
    :   idASSIGN ACTION
    ;

//////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////
// L E X E R

// get rid of warnings:
fragment STRING_LITERAL : ;
fragment FORCED_ACTION : ;
fragment DOC_COMMENT : ;
fragment SEMPRED : ;

WS
    :   (   ' '
        |   '\t'
        |   ('\r')? '\n'
        )
    ;

COMMENT
    :   ( SL_COMMENT | ML_COMMENT )
    ;

fragment
SL_COMMENT
    :   '//'
        ( ' $ANTLR ' SRC (('\r')? '\n')? // src directive
        |   ~('\r'|'\n')* (('\r')? '\n')?
        )
    ;

fragment
ML_COMMENT
    :   '/*'
        .*
        '*/'
    ;

OPEN_ELEMENT_OPTION
    :   '<'
    ;

CLOSE_ELEMENT_OPTION
    :   '>'
    ;

AMPERSAND : '@';

COMMA : ',';

QUESTION :  '?' ;

TREE_BEGIN : '^(' ;

LPAREN: '(' ;

RPAREN: ')' ;

COLON : ':' ;

STAR:   '*' ;

PLUS:   '+' ;

ASSIGN : '=' ;

PLUS_ASSIGN : '+=' ;

IMPLIES : '=>' ;

REWRITE : '->' ;

SEMI:   ';' ;

ROOT : '^' ;

BANG : '!' ;

OR  :   '|' ;

WILDCARD : '.' ;

ETC : '...' ;

RANGE : '..' ;

NOT :   '~' ;

RCURLY: '}' ;

DOLLAR : '$' ;

STRAY_BRACKET
    :   ']'
    ;

CHAR_LITERAL
    :   '\''
        (   ESC
        |   ~('\\'|'\'')
        )*
        '\''
    ;

DOUBLE_QUOTE_STRING_LITERAL
    :   '"'
        ( '\\' '"'
        |   '\\'~'"'
        |~('\\'|'"')
        )*
        '"'
    ;

DOUBLE_ANGLE_STRING_LITERAL
    :   '<<' .* '>>'
    ;

fragment
ESC
    :   '\\'
        (   // due to the way ESC is used, we don't need to handle the following character in different ways
            /*'n'
        |   'r'
        |   't'
        |   'b'
        |   'f'
        |   '"'
        |   '\''
        |   '\\'
        |   '>'
        |   'u' XDIGIT XDIGIT XDIGIT XDIGIT
        |*/ . // unknown, leave as it is
        )
    ;

fragment
DIGIT
    :   '0'..'9'
    ;

fragment
XDIGIT
    :   '0' .. '9'
    |   'a' .. 'f'
    |   'A' .. 'F'
    ;

INT
    :   ('0'..'9')+
    ;

ARG_ACTION
    :   '['
        NESTED_ARG_ACTION
        ']'
    ;

fragment
NESTED_ARG_ACTION
    :   ( '\\' ']'
        |   '\\'~(']')
        |   ACTION_STRING_LITERAL
        |   ACTION_CHAR_LITERAL
        |~('\\'|'"'|'\''|']')
        )*
    ;

ACTION
    :   NESTED_ACTION
        ('?')?
    ;

fragment
NESTED_ACTION
    :   '{'
        (   NESTED_ACTION
        |   ACTION_CHAR_LITERAL
        | COMMENT
        |   ACTION_STRING_LITERAL
        |   ACTION_ESC
        |   ~('{'|'\''|'"'|'\\'|'}')
        )*
        '}'
    ;

fragment
ACTION_CHAR_LITERAL
    :   '\''
        (   ACTION_ESC
        |   ~('\\'|'\'')
        )*
        '\''
    ;

fragment
ACTION_STRING_LITERAL
    :   '"'
        (   ACTION_ESC
        |   ~('\\'|'"')
        )*
        '"'
    ;

fragment
ACTION_ESC
    :   '\\\''
    |   '\\\"'
    |   '\\' ~('\''|'"')
    ;

TOKEN_REF
    :   'A'..'Z'
        (   'a'..'z'|'A'..'Z'|'_'|'0'..'9'
        )*
    ;

TOKENS
    :   'tokens' WS_LOOP '{'
    ;

OPTIONS
    :   'options' WS_LOOP '{'
    ;

// we get a warning here when looking for options '{', but it works right
RULE_REF
    :   'a'..'z' ('a'..'z' | 'A'..'Z' | '_' | '0'..'9')*
    ;

fragment
WS_LOOP
    :   (   WS
        |   COMMENT
        )*
    ;

fragment
WS_OPT
    :   (WS)?
    ;

/** Reset the file and line information; useful when the grammar
 *  has been generated so that errors are shown relative to the
 *  original file like the old C preprocessor used to do.
 */
fragment
SRC
    :   'src' ' 'ACTION_STRING_LITERAL ' 'INT
    ;