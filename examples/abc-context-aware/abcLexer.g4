/*
Lexer grammar for ABC music notation.
References:
 - https://en.wikipedia.org/wiki/ABC_notation

*/
lexer grammar abcLexer;

// ---------------------------------------------------------------------
// Header fields and body tokens share a lexer. The parser selects their
// meaning from its current rule rather than a lexer mode.
// ---------------------------------------------------------------------

COMMENT
   : '%' ~ [\r\n]* -> skip
   ;

WS
   : [ \t\r\n]+ -> skip
   ;

// X: (reference number) starts a new tune.
XFIELD
   : 'X' ':' ~ [\r\n]*
   ;

// K: (key) ends the header.
KFIELD
   : 'K' ':' ~ [\r\n]*
   ;

FIELD
   : [A-Za-z] ':' ~ [\r\n]*
   ;

// ---------------------------------------------------------------------
// Tune body: the music itself. The parser can expect these tokens after KFIELD.
// ---------------------------------------------------------------------


// An inline field, e.g. [K:Dm] or [r:allegro], changes a setting
// without leaving the tune body.
INLINE_FIELD
   : '[' [A-Za-z] ':' ~ [\]\r\n]* ']'
   ;

// A whole-line field inside the body, e.g. a lyrics line (w:...),
// a voice line (V:...), or a key/meter/tempo change.
ACCIDENTAL
   : '^^'
   | '__'
   | '^'
   | '_'
   | '='
   ;

PITCH
   : [A-Ga-g]
   ;

OCTAVE
   : ('\'' | ',')+
   ;

NUMBER
   : [0-9]+
   ;

SLASH
   : '/'
   ;

REST
   : [xzXZ]
   ;

BROKEN
   : '>'
   | '<'
   ;

TIE
   : '-'
   ;

GRACE_OPEN
   : '{'
   ;

GRACE_CLOSE
   : '}'
   ;

TUPLET
   : '(' [0-9] (':' [0-9] (':' [0-9])?)?
   ;

SLUR_OPEN
   : '('
   ;

SLUR_CLOSE
   : ')'
   ;

CHORD_SYMBOL
   : '"' ~ ["\r\n]* '"'
   ;

DECORATION
   : '!' ~ [!\r\n]* '!'
   ;

STACCATO
   : '.'
   ;

AMPERSAND
   : '&'
   ;

BARLINE
   : '::'
   | ':|:'
   | ':|'
   | '|:'
   | '|]'
   | '[|'
   | '||'
   | '|'
   ;

LBRACKET
   : '['
   ;

RBRACKET
   : ']'
   ;
