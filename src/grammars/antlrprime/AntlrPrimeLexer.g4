lexer grammar AntlrPrimeLexer;

options { superClass = LexerAdaptor; }

import LexBasic;

tokens { TOKEN_REF , RULE_REF , LEXER_CHAR_SET }
channels { OFF_CHANNEL , COMMENT }

DOC_COMMENT
   : DocComment -> channel (COMMENT)
   ;

BLOCK_COMMENT
   : BlockComment -> channel (COMMENT)
   ;

LINE_COMMENT
   : LineComment -> channel (COMMENT)
   ;

INT
   : DecimalNumeral
   ;

STRING_LITERAL
   : SQuoteLiteral
   ;

UNTERMINATED_STRING_LITERAL
   : USQuoteLiteral
   ;

BEGIN_ARGUMENT
   : LBrack
   { this.handleBeginArgument(); }
   ;

BEGIN_ACTION
   : LBrace -> pushMode (TargetLanguageAction)
   ;

OPTIONS      : 'options'  WSNLCHARS* '{'  ;
TOKENS       : 'tokens'   WSNLCHARS* '{'  ;
CHANNELS     : 'channels' WSNLCHARS* '{'  ;

fragment WSNLCHARS : ' ' | '\t' | '\f' | '\n' | '\r' ;

IMPORT
   : 'import'
   ;

FRAGMENT
   : 'fragment'
   ;

LEXER
   : 'lexer'
   ;

PARSER
   : 'parser'
   ;

GRAMMAR
   : 'grammar'
   ;

PROTECTED
   : 'protected'
   ;

PUBLIC
   : 'public'
   ;

PRIVATE
   : 'private'
   ;

RETURNS
   : 'returns'
   ;

LOCALS
   : 'locals'
   ;

THROWS
   : 'throws'
   ;

CATCH
   : 'catch'
   ;

FINALLY
   : 'finally'
   ;

MODE
   : 'mode'
   ;

COLON
   : Colon
   ;

COLONCOLON
   : DColon
   ;

COMMA
   : Comma
   ;

SEMI
   : Semi
   ;

LPAREN
   : LParen
   ;

RPAREN
   : RParen
   ;

LBRACE
   : LBrace
   ;

RBRACE
   : RBrace
   ;

RARROW
   : RArrow
   ;

LT
   : Lt
   ;

GT
   : Gt
   ;

ASSIGN
   : Equal
   ;

QUESTION
   : Question
   ;

STAR
   : Star
   ;

PLUS_ASSIGN
   : PlusAssign
   ;

PLUS
   : Plus
   ;

BANG
   : Bang
   ;

ROOT
   : Root
   ;

OR
   : Pipe
   ;

DOLLAR
   : Dollar
   ;

RANGE
   : Range
   ;

DOT
   : Dot
   ;

AT
   : At
   ;

POUND
   : Pound
   ;

NOT
   : Tilde
   ;

ID
   : Id
   ;

WS
   : Ws+ -> channel (OFF_CHANNEL)
   ;

ERRCHAR
   : . -> channel (HIDDEN)
   ;

mode Argument;
// E.g., [int x, List<String> a[]]

NESTED_ARGUMENT
   : LBrack -> type (ARGUMENT_CONTENT) , pushMode (Argument)
   ;

ARGUMENT_ESCAPE
   : EscAny -> type (ARGUMENT_CONTENT)
   ;

ARGUMENT_STRING_LITERAL
   : DQuoteLiteral -> type (ARGUMENT_CONTENT)
   ;

ARGUMENT_CHAR_LITERAL
   : SQuoteLiteral -> type (ARGUMENT_CONTENT)
   ;

END_ARGUMENT
   : RBrack
   { this.handleEndArgument(); }
   ;

UNTERMINATED_ARGUMENT
   : EOF -> popMode
   ;

ARGUMENT_CONTENT
   : .
   ;

mode TargetLanguageAction;

NESTED_ACTION
   : LBrace -> type (ACTION_CONTENT) , pushMode (TargetLanguageAction)
   ;

ACTION_ESCAPE
   : EscAny -> type (ACTION_CONTENT)
   ;

ACTION_STRING_LITERAL
   : DQuoteLiteral -> type (ACTION_CONTENT)
   ;

ACTION_CHAR_LITERAL
   : SQuoteLiteral -> type (ACTION_CONTENT)
   ;

ACTION_DOC_COMMENT
   : DocComment -> type (ACTION_CONTENT)
   ;

ACTION_BLOCK_COMMENT
   : BlockComment -> type (ACTION_CONTENT)
   ;

ACTION_LINE_COMMENT
   : LineComment -> type (ACTION_CONTENT)
   ;

END_ACTION
   : RBrace
   { this.handleEndAction(); }
   ;

UNTERMINATED_ACTION
   : EOF -> popMode
   ;

ACTION_CONTENT
   : .
   ;

mode LexerCharSet;

LEXER_CHAR_SET_BODY
   : (~ [\]\\] | EscAny)+ -> more
   ;

LEXER_CHAR_SET
   : RBrack -> popMode
   ;

UNTERMINATED_CHAR_SET
   : EOF -> popMode
   ;

fragment Id
   : NameStartChar NameChar*
   ;
   
