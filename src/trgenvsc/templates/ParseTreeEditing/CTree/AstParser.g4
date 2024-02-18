grammar AstParser;

options { tokenVocab = AstLexer; }

ast : node EOF ;
node : OPEN_PAREN ID (val | node | attr)* CLOSE_PAREN ;
attr : ID EQUALS StringLiteral ;
val : OPEN_CURLY ID CLOSE_CURLY ;

 