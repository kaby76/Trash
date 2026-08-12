insert nodes //labeledAlt/(POUND | identifer)/@WS before ./ancestor::labeledAlt;
delete //labeledAlt/(POUND | identifier);

move //labeledLexerElement/(identifier | ASSIGN | PLUS_ASSIGN)/@WS ./ancestor::labeledLexerElement;
delete //labeledLexerElement/(identifier | ASSIGN | PLUS_ASSIGN);

move //labeledElement/identifier/@WS ./ancestor::labeledElement;
delete //labeledElement/(identifier | ASSIGN | PLUS_ASSIGN);

