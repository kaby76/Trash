grammar Pegjs;

grammar_ : initializer? rule+ EOF;
initializer : CodeBlock eos ;
eos : ';' | ;
rule : identifier StringLiteral? '=' expression eos ;
expression : choiceexpression ;
choiceexpression : actionexpression ('/' actionexpression)* ;
actionexpression : sequenceexpression CodeBlock? ;
sequenceexpression : labeledexpression labeledexpression* ;
labeledexpression : '@' labelidentifier? prefixedexpression
  | labelidentifier prefixedexpression
  | prefixedexpression
  ;
labelidentifier : identifier ':' ;
prefixedexpression : prefixedoperator suffixedexpression
  | suffixedexpression
  ;
prefixedoperator : '$'
  | '&'
  | '!'
  ;
suffixedexpression : primaryexpression suffixedoperator
  | primaryexpression
  ;
suffixedoperator : '?'
  | '*'
  | '+'
  ;
primaryexpression : literalMatcher
  | CharacterClassMatcher
  | AnyMatcher
  | rulereferenceexpression
  | semanticpredicateexpression
  | '(' expression ')'
  ;
rulereferenceexpression : identifier  /* {! stringliteral? '=' }? */ ;
semanticpredicateexpression : semanticpredicateoperator CodeBlock ;
semanticpredicateoperator : '&' | '!' ;

identifier : Identifier;
Identifier : Identifierstart Identifierpart* ;

WhiteSpace : [\t\n\r\f \u00a0\ufeff] -> channel(HIDDEN);

fragment LineTerminator : [\n\r\u2028\u2029] ;
fragment LineTerminatorSequence : '\n' | '\r' '\n' | '\r' | '\u2028' | '\u2029' ;
fragment SourceCharacter : ~[\n\r\u2028\u2029] ;

Comment : (MultiLineComment | SingleLineComment) -> channel(HIDDEN);
fragment MultiLineComment : '/*' .*? '*/' ;
fragment SingleLineComment : '//' SourceCharacter* ;
fragment Identifierstart : UnicodeLetter | '$' | '_' | '\\' UnicodeEscapeSequence ;
fragment Identifierpart : Identifierstart | UnicodeCombiningMark | UnicodeDigit | UnicodeConnectorPunctuation | '\u200c' | '\u200d' ;
fragment UnicodeLetter : [\p{Lu}] | [\p{Ll}] | [\p{Lt}] | [\p{Lm}] | [\p{Lo}] | [\p{Nl}] ;
fragment UnicodeCombiningMark : [\p{Mn}] | [\p{Mc}] ;
fragment UnicodeDigit : [\p{Nd}] ;
fragment UnicodeConnectorPunctuation : [\p{Pc}] ;

CharacterClassMatcher : '[' CharacterPart*? ']' 'i'? ;
fragment CharacterPart : (~(']' | '\\')) | '\\' . ;

fragment EscapeSequence : SingleEscapeSequence | '0' | HexEscapeSequence | UnicodeEscapeSequence ;
fragment SingleEscapeCharacter : '\'' | '"' | '\\' | 'b' | 'f' | 'n' | 'r' | 't' | 'v' | ']' | . ;
fragment EscapeCharacter : SingleEscapeCharacter | DecimalDigit | 'x' | 'u' ;
fragment HexEscapeSequence : 'x' HexDigit HexDigit ;
fragment UnicodeEscapeSequence : 'u' HexDigit HexDigit HexDigit HexDigit ;

DecimalDigit : [0-9] ;
HexDigit : [0-9a-fA-F] ;
AnyMatcher : '.' ;
CodeBlock : CB ;
fragment CB : '{' CBAux* '}' ;
fragment CBAux : (~('{' | '}')) | CB ;

literalMatcher : StringLiteral 'i'? ;
StringLiteral : '"' DoubleStringCharacters? '"' | '\'' SingleStringCharacters? '\'' ;
fragment DoubleStringCharacters : DoubleStringCharacter+ ;
fragment DoubleStringCharacter : ~["\\\r\n] | DoubleEscapeSequence ;
fragment DoubleEscapeSequence : '\\' [bvtnfr"'\\] | OctalEscape | UnicodeEscape ;
fragment SingleStringCharacters : SingleStringCharacter+ ;
fragment SingleStringCharacter : ~['\\\r\n] | SingleEscapeSequence ;
fragment SingleEscapeSequence : '\\' [bvtnfr"'\\] | OctalEscape | UnicodeEscape ;
fragment OctalDigit : [0-7] ;
fragment OctalEscape : '\\' OctalDigit | '\\' OctalDigit OctalDigit | '\\' ZeroToThree OctalDigit OctalDigit ;
fragment ZeroToThree : [0-3] ;
fragment UnicodeEscape : '\\' 'u'+  HexDigit HexDigit HexDigit HexDigit ;

