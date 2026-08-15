// https://www.w3.org/TR/REC-xml/#sec-notation
// https://www.bottlecaps.de/rr/ui

parser grammar W3CebnfParser;

options { tokenVocab = W3CebnfLexer; }

grammar_ : production* EOF ;
production : SYMBOL  '::=' choice ;
choice : sequence_or_difference (  '|' sequence_or_difference)* ;
sequence_or_difference : ( item (  '-' item | item* ))? ;
item : primary (  '?' |  '*' |  '+' )* ;
primary : SYMBOL | STRING | HEX | SET | CONSTRAINT |  '(' choice  ')' ;
