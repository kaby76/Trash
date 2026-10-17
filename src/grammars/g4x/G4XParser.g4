/*
 * [The "BSD license"]
 *  Copyright (c) 2012-2014 Terence Parr
 *  Copyright (c) 2012-2014 Sam Harwell
 *  Copyright (c) 2015 Gerald Rosenberg
 *  All rights reserved.
 *
 *  Redistribution and use in source and binary forms, with or without
 *  modification, are permitted provided that the following conditions
 *  are met:
 *
 *  1. Redistributions of source code must retain the above copyright
 *     notice, this list of conditions and the following disclaimer.
 *  2. Redistributions in binary form must reproduce the above copyright
 *     notice, this list of conditions and the following disclaimer in the
 *     documentation and/or other materials provided with the distribution.
 *  3. The name of the author may not be used to endorse or promote products
 *     derived from this software without specific prior written permission.
 *
 *  THIS SOFTWARE IS PROVIDED BY THE AUTHOR ``AS IS'' AND ANY EXPRESS OR
 *  IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
 *  OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
 *  IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT,
 *  INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
 *  NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
 *  DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
 *  THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 *  (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
 *  THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

/*	A grammar for ANTLR v4 written in ANTLR v4.
 *
 *	Modified 2015.06.16 gbr
 *	-- update for compatibility with Antlr v4.5
 *	-- add mode for channels
 *	-- moved members to LexerAdaptor
 * 	-- move fragments to imports
 */

// $antlr-format alignTrailingComments on, columnLimit 130, minEmptyLines 1, maxEmptyLinesToKeep 1, reflowComments off
// $antlr-format useTab off, allowShortRulesOnASingleLine off, allowShortBlocksOnASingleLine on, alignSemicolons hanging
// $antlr-format alignColons hanging

parser grammar G4XParser;

options {
    tokenVocab = G4XLexer;
}

// The main entry point for parsing a v4 grammar.
grammarSpec
    : grammarDecl prequelConstruct* rules modeSpec* EOF
    ;

grammarDecl
    : grammarType ID SEMI
    ;

grammarType
    : LEXER GRAMMAR
    | PARSER GRAMMAR
    | GRAMMAR
    ;

// This is the list of all constructs that can be declared before
// the set of rules that compose the grammar, and is invoked 0..n
// times by the grammarPrequel rule.

prequelConstruct
    : optionsSpec
    | delegateGrammars
    | tokensSpec
    | channelsSpec
    | action_
    ;

// ------------
// Options - things that affect analysis and/or code generation

optionsSpec
    : OPTIONS (option SEMI)* RBRACE
    ;

option
    : ID ASSIGN optionValue
    ;

optionValue
    : ID (DOT ID)*
    | STRING_LITERAL
    | ACTION
    | INT
    ;

// ------------
// Delegates

delegateGrammars
    : IMPORT delegateGrammar (COMMA delegateGrammar)* SEMI
    ;

delegateGrammar
    : ID ASSIGN ID
    | ID
    ;

// ------------
// Tokens & Channels

tokensSpec
    : TOKENS idList? RBRACE
    ;

channelsSpec
    : CHANNELS idList? RBRACE
    ;

idList
    : ID (COMMA ID)* COMMA?
    ;

// Match stuff like @parser::members {int i;}

action_
    : AT (actionScopeName COLONCOLON)? ID ACTION
    ;

// Scope names could collide with keywords; allow them as ids for action scopes

actionScopeName
    : ID
    | LEXER
    | PARSER
    ;

argActionBlock
    : BEGIN_ARGUMENT ARGUMENT_CONTENT*? END_ARGUMENT
    ;

modeSpec
    : MODE ID SEMI ruleSpec*
    ;

rules
    : ruleSpec*
    ;

ruleSpec
    : ruleModifiers? ID argActionBlock? ruleReturns? throwsSpec? localsSpec? rulePrequel* COLON ruleAltList SEMI
        exceptionGroup
    ;

exceptionGroup
    : exceptionHandler* finallyClause?
    ;

exceptionHandler
    : CATCH argActionBlock ACTION
    ;

finallyClause
    : FINALLY ACTION
    ;

rulePrequel
    : optionsSpec
    | ruleAction
    ;

ruleReturns
    : RETURNS argActionBlock
    ;

// --------------
// Exception spec
throwsSpec
    : THROWS qualifiedIdentifier (COMMA qualifiedIdentifier)*
    ;

localsSpec
    : LOCALS argActionBlock
    ;

/** Match stuff like @init {int i;} */
ruleAction
    : AT ID ACTION
    ;

ruleModifiers
    : ruleModifier+
    ;

// Modifiers are accepted on every rule. Restrictions such as whether
// 'fragment' is meaningful for parser rules are checked separately.

ruleModifier
    : PUBLIC
    | PRIVATE
    | PROTECTED
    | FRAGMENT
    ;

ruleAltList
    : alternative (OR alternative)*
    ;

// E.g., channel(HIDDEN), skip, more, mode(INSIDE), push(INSIDE), pop

lexerCommands
    : RARROW lexerCommand (COMMA lexerCommand)*
    ;

lexerCommand
    : lexerCommandName LPAREN lexerCommandExpr RPAREN
    | lexerCommandName
    ;

lexerCommandName
    : ID
    | MODE
    ;

lexerCommandExpr
    : ID
    | INT
    ;

// --------------------
// Rule Alts

altList
    : alternative (OR alternative)*
    ;

alternative
    : elementOptions? element* exclusion? lexerCommands?
    ;

// G4X set difference. The parenthesized form keeps OR in the exclusion
// list distinct from the surrounding rule's alternatives.
exclusion
    : MINUS (exclusionOperand | LPAREN exclusionOperand (OR exclusionOperand)* RPAREN)
    ;

exclusionOperand
    : ID
    | STRING_LITERAL
    | characterRange
    | argActionBlock
    | LEXER_CHAR_SET
    ;

element
    : atom ebnfSuffix?
    | ebnf
    | ACTION QUESTION? predicateOptions?
    ;

predicateOptions
    : LT predicateOption (COMMA predicateOption)* GT
    ;

predicateOption
    : elementOption
    | ID ASSIGN (ACTION | INT | STRING_LITERAL)
    ;

// --------------------
// EBNF and blocks

ebnf
    : block ebnfSuffix?
    ;

ebnfSuffix
    : QUESTION QUESTION?
    | STAR QUESTION?
    | PLUS QUESTION?
    ;

atom
    : characterRange
    | symbolRef
    | notSet
    | argActionBlock
    | LEXER_CHAR_SET
    | wildcard
    ;

wildcard
    : DOT elementOptions?
    ;

// --------------------
// Inverted element set
notSet
    : NOT setElement
    | NOT blockSet
    ;

blockSet
    : LPAREN setElement (OR setElement)* RPAREN
    ;

setElement
    : ID elementOptions?
    | STRING_LITERAL elementOptions?
    | characterRange
    | argActionBlock
    | LEXER_CHAR_SET
    ;

// -------------
// Grammar Block
block
    : LPAREN (optionsSpec? ruleAction* COLON)? altList RPAREN
    ;

// ---------------
// Character Range
characterRange
    : STRING_LITERAL RANGE STRING_LITERAL
    ;

symbolRef
    : ID argActionBlock? elementOptions?
    | STRING_LITERAL elementOptions?
    ;

// Terminals may be adorned with certain options when
// reference in the grammar: TOK<,,,>
elementOptions
    : LT elementOption (COMMA elementOption)* GT
    ;

elementOption
    : qualifiedIdentifier
    | ID ASSIGN (qualifiedIdentifier | STRING_LITERAL | INT)
    ;

qualifiedIdentifier
    : ID (DOT ID)*
    ;
