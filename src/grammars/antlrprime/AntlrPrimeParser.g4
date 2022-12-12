parser grammar AntlrPrimeParser;

options
{
	tokenVocab = AntlrPrimeLexer;
}

grammarSpec
   : grammarDecl prequelConstruct* rules modeSpec* EOF
   ;

grammarDecl
   : grammarType identifier SEMI
   ;

grammarType
   : (LEXER GRAMMAR | PARSER GRAMMAR | GRAMMAR)
   ;

prequelConstruct
   : optionsSpec
   | delegateGrammars
   | tokensSpec
   | channelsSpec
   | action_
   ;

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

delegateGrammars
   : IMPORT delegateGrammar (COMMA delegateGrammar)* SEMI
   ;

delegateGrammar
   : identifier ASSIGN identifier
   | identifier
   ;

tokensSpec
   : TOKENS idList? RBRACE
   ;

channelsSpec
   : CHANNELS idList? RBRACE
   ;

idList
   : identifier (COMMA identifier)* COMMA?
   ;

action_
   : AT (actionScopeName COLONCOLON)? identifier actionBlock
   ;

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
   : ruleModifiers? RULE_REF BANG? argActionBlock? ruleReturns? throwsSpec? localsSpec? rulePrequel* COLON ruleBlock SEMI exceptionGroup
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

labeledLexerElement
   : identifier (ASSIGN | PLUS_ASSIGN) (lexerAtom | lexerBlock)
   ;

lexerBlock
   : LPAREN lexerAltList RPAREN
   ;

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
   | notSet ( ROOT | BANG | )
   | DOT elementOptions?  ( ROOT | BANG | )
   ;

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

block
   : LPAREN (optionsSpec? ruleAction* COLON)? altList RPAREN (ROOT | BANG)?
   ;

ruleref
   : RULE_REF argActionBlock? elementOptions? (ROOT | BANG)?
   ;

characterRange
   : STRING_LITERAL RANGE STRING_LITERAL
   ;

terminal
   : (
	TOKEN_REF elementOptions?
	| STRING_LITERAL elementOptions?
      ) (ROOT | BANG)?
   ;

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
   
