parser grammar ParserOnly;
options { tokenVocab=ModeDemo; }
start : Identifier EOF;
