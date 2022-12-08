grammar CocoR;

coco :
  .*?
  'COMPILER'
  Ident
  .*?
  'IGNORECASE'?
  ( 'CHARACTERS' setDecl* )?
  ( 'TOKENS'  tokenDecl* )?
  ( 'PRAGMAS' tokenDecl* )?
  ( 'COMMENTS'
    'FROM' tokenExpr
    'TO' tokenExpr
    'NESTED'?
  )*
  ( 'IGNORE' set
  )*

  // SYNC

  'PRODUCTIONS'
  ( Ident
    attrDecl?
    semText?
    //WEAK
    '='
    expression
    //WEAK
    '.'
  )*

  'END' Ident
  '.'
;

setDecl
:
  Ident
  '=' set
  '.'
;

set
:
  simSet
  ( '+' simSet
  | '-' simSet
  )*
;

simSet
:
( Ident
| String
| char
  ( '..' char
  )?
| 'ANY'
)
;

char
:
  Char
;

tokenDecl
:
  sym
  // SYNC
  ( '=' tokenExpr '.'
  |
  )
  semText?
;

attrDecl
:
  '<'                           // attributes denoted by < ... >
  ( ('^' | 'out')
    typeName
    Ident
    ( '>'
    | ','
      .? '>'
    )
  |
    ( . .*?)? '>'
  )
|
  '<.'
  ( ('^' | 'out')
    typeName
    Ident
    ( '.>'
    | ','
      .*? '.>'
    )
  | (. .*? )?  '.>'
  )
  ;

typeName
: Ident ('.' Ident | '[' ']' | '<' typeName (',' typeName)* '>')*
;

expression
:
  term
  ( //WEAK
  '|'
    term
  )*
;

term
:
( resolver?
  factor
  factor*
|
)
;

factor
:
( 'WEAK'?
  sym
  attribs?
| '(' expression ')'
| '[' expression ']'
| '{' expression '}'
| semText
| 'ANY'
| 'SYNC'
)
;

resolver
:
  'IF' '('
  condition
;

condition : ( '(' condition | . )*? ')' ;

tokenExpr
:
  tokenTerm
  ( //WEAK
  '|'
    tokenTerm
  )*
;

tokenTerm
:
  tokenFactor
  tokenFactor*
  ( 'CONTEXT'
    '(' tokenExpr
    ')'
  )?
;

tokenFactor
:
( sym
| '(' tokenExpr ')'
| '[' tokenExpr ']'
| '{' tokenExpr '}'
)
;

sym
:
( Ident
| (String
  | Char
  )
)
;

attribs
:
  '<'                           // attributes denoted by < ... >
  ( ('^' | 'out')
    ( .
    | bracketed
    )*?
    ( '>'
    | ','
      .*?
      '>'
    )
  |
    ( . .*?
    )? '>'
  )
|
	'<.'                          // attributes denoted by <. ... .>
  ( ('^' | 'out')
    ( .
    | bracketed
    )*?
    ( '.>'
    | ','
      .*?
      '.>'
    )
  |
    ( . .*?
    )? '.>'
  )
;

bracketed
: '(' (bracketed | .)*? ')' | '[' (bracketed | .)? ']' ;

semText
:
  '(.'
  .*?
  '.)'
;



fragment Letter: [A-Za-z_] ;
fragment Digit: [0-9] ;
fragment Cr: '\r' ;
fragment Lf: '\n' ;
fragment Tab: '\t' ;

// In Coco/R, there is no explicit use of space anywhere in the
// grammar file!
fragment Space : ' ' ;
fragment StringCh: ~('"' | '\\' | '\r' | '\n') ;
fragment CharCh: ~('\'' | '\\' | '\r' | '\n') ;
fragment Printable: '\u0020' .. '\u007e';
fragment Hex : [0123456789abcdef] ;

Ident : Letter ( Letter | Digit )* ;
Number : Digit Digit* ;
String : '"' ( StringCh | '\\' Printable )* '"' ;
Char  : '\'' ( CharCh | '\\' Printable Hex* ) '\'' ;

DdtSym : '$' ( Digit | Letter )* -> channel(HIDDEN) ;

OptionSym : '$' Letter Letter* '='
              ( Digit | Letter
              | '-' | '.' | ':'
              )* -> channel(HIDDEN) ;

COMMENT : '/*' .*? '*/' -> channel(HIDDEN) ;
LINE_COMMENT : '//' ~'\n'* -> channel(HIDDEN) ;
// Incomplete! IGNORE_ : (Cr | Lf | Tab) -> channel(HIDDEN) ;
IGNORE_ : (Cr | Lf | Tab | Space) -> channel(HIDDEN) ;


// Coco/R is context-sensitive lexing, but Antlr is not.
// For now, just have a bunch of typical tokens.

OTHER : . ;
