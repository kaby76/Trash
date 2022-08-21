
// https://www.w3.org/TR/REC-xml/#sec-notation

parser grammar W3CebnfParser;

options { tokenVocab = W3CebnfLexer; contextSuperClass=AttributedParseTreeNode; }

prods : prod+ EOF ;
prod : lhs PPEQ rhs ;
lhs : symbol ;
rhs : alts ;
symbol : SYMBOL ;
alts : alt ( VP alt )* ;
alt : element* ;
element : block ( M block )*  ;
block : atom suffix? ;
atom : symbol | SET | STRING | OP alts CP ;
suffix : Q Q? | S Q? | P Q? ;
