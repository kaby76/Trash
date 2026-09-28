parser grammar OptionsParser;
options { tokenVocab=OptionsLexer; language=CSharp; }
start : WORD EOF;
