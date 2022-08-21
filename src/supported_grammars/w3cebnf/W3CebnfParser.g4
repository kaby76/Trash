// https://www.w3.org/TR/REC-xml/#sec-notation
// https://www.bottlecaps.de/rr/ui

parser grammar W3CebnfParser;

options { tokenVocab = W3CebnfLexer; }

grammar_ : production* EOF ;
production : SYMBOL CCEQ ( choice | CONSTRAINT ) ;
choice : sequence_or_difference ( ALT sequence_or_difference)* ;
sequence_or_difference : ( item ( M item | item* ))? ;
item : primary ( Q | S | P )* ;
primary : SYMBOL | STRING | HEX | SET | OP choice CP ;
