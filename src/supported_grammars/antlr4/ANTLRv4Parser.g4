parser grammar ANTLRv4Parser;

options
{
	tokenVocab = ANTLRv4Lexer;
//    contextSuperClass=LanguageServer.AttributedParseTreeNode;
}

// The main entry point for parsing a v4 grammar.
grammarSpec
   : grammarDecl prequelConstruct* rules modeSpec* EOF
   ;

grammarDecl
   : grammarType identifier SEMI
   ;

grammarType
   : (LEXER GRAMMAR | PARSER GRAMMAR | GRAMMAR)
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
   : identifier ASSIGN optionValue
   ;

optionValue
   : identifier (DOT identifier)*
   | STRING_LITERAL
   | actionBlock
   | INT
   ;
   // ------------
   // Delegates

delegateGrammars
   : IMPORT delegateGrammar (COMMA delegateGrammar)* SEMI
   ;

delegateGrammar
   : identifier ASSIGN identifier
   | identifier
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
   : identifier (COMMA identifier)* COMMA?
   ;
   // Match stuff like @parser::members {int i;}

action_
   : AT (actionScopeName COLONCOLON)? identifier actionBlock
   ;
   // Scope names could collide with keywords; allow them as ids for action scopes

actionScopeName
   : identifier
   | LEXER
   | PARSER
   ;

actionBlock
   : BEGIN_ACTION ACTION_CONTENT* END_ACTION
   ;

argActionBlock
   : BEGIN_ARGUMENT ARGUMENT_CONTENT* END_ARGUMENT
   ;

modeSpec
   : MODE identifier SEMI lexerRuleSpec*
   ;

rules
   : ruleSpec*
   ;

ruleSpec
   : parserRuleSpec
   | lexerRuleSpec
   ;

parserRuleSpec
   : ruleModifiers? RULE_REF argActionBlock? ruleReturns? throwsSpec? localsSpec? rulePrequel* COLON ruleBlock SEMI exceptionGroup
   ;

exceptionGroup
   : exceptionHandler* finallyClause?
   ;

exceptionHandler
   : CATCH argActionBlock actionBlock
   ;

finallyClause
   : FINALLY actionBlock
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
   : THROWS identifier (COMMA identifier)*
   ;

localsSpec
   : LOCALS argActionBlock
   ;

/** Match stuff like @init {int i;} */
ruleAction
   : AT identifier actionBlock
   ;

ruleModifiers
   : ruleModifier+
   ;
   // An individual access modifier for a rule. The 'fragment' modifier
   // is an internal indication for lexer rules that they do not match
   // from the input but are like subroutines for other lexer rules to
   // reuse for certain lexical patterns. The other modifiers are passed
   // to the code generation templates and may be ignored by the template
   // if they are of no use in that language.

ruleModifier
   : PUBLIC
   | PRIVATE
   | PROTECTED
   | FRAGMENT
   ;

ruleBlock
   : ruleAltList
   ;

ruleAltList
   : labeledAlt (OR labeledAlt)*
   ;

labeledAlt
   : alternative (POUND identifier)?
   ;
   // --------------------
   // Lexer rules

lexerRuleSpec
   : FRAGMENT? TOKEN_REF COLON lexerRuleBlock SEMI
   ;

lexerRuleBlock
   : lexerAltList
   ;

lexerAltList
   : lexerAlt (OR lexerAlt)*
   ;

lexerAlt
   : lexerElements lexerCommands?
   |
   // explicitly allow empty alts
   ;

lexerElements
   : lexerElement+
   |
   ;

lexerElement
   : labeledLexerElement ebnfSuffix?
   | lexerAtom ebnfSuffix?
   | lexerBlock ebnfSuffix?
   | actionBlock QUESTION?
   ;
   // but preds can be anywhere

labeledLexerElement
   : identifier (ASSIGN | PLUS_ASSIGN) (lexerAtom | lexerBlock)
   ;

lexerBlock
   : LPAREN lexerAltList RPAREN
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
   : identifier
   | MODE
   ;

lexerCommandExpr
   : identifier
   | INT
   ;
   // --------------------
   // Rule Alts

altList
   : alternative (OR alternative)*
   ;

alternative
   : elementOptions? element+
   |
   // explicitly allow empty alts
   ;

element
   : labeledElement (ebnfSuffix |)
   | atom (ebnfSuffix |)
   | ebnf
   | actionBlock QUESTION?
   ;

labeledElement
   : identifier (ASSIGN | PLUS_ASSIGN) (atom | block)
   ;
   // --------------------
   // EBNF and blocks

ebnf
   : block blockSuffix?
   ;

blockSuffix
   : ebnfSuffix
   ;

ebnfSuffix
   : QUESTION QUESTION?
   | STAR QUESTION?
   | PLUS QUESTION?
   ;

lexerAtom
   : characterRange
   | terminal
   | notSet
   | LEXER_CHAR_SET
   | DOT elementOptions?
   ;

atom
   : terminal
   | ruleref
   | notSet
   | DOT elementOptions?
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
   : TOKEN_REF elementOptions?
   | STRING_LITERAL elementOptions?
   | characterRange
   | LEXER_CHAR_SET
   ;

// -------------
// Grammar Block
block
   : LPAREN (optionsSpec? ruleAction* COLON)? altList RPAREN
   ;

// ----------------
// Parser rule ref
ruleref
   : RULE_REF argActionBlock? elementOptions?
   ;

// ---------------
// Character Range
characterRange
   : STRING_LITERAL RANGE STRING_LITERAL
   ;

terminal
   : TOKEN_REF elementOptions?
   | STRING_LITERAL elementOptions?
   ;

// Terminals may be adorned with certain options when
// reference in the grammar: TOK<,,,>
elementOptions
   : LT elementOption (COMMA elementOption)* GT
   ;

elementOption
   : identifier
   | identifier ASSIGN (identifier | STRING_LITERAL)
   ;

identifier
   : RULE_REF
   | TOKEN_REF
   ;
   
