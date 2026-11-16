parser grammar FoldParser;

options { tokenVocab = FoldLexer; }

start : SELECT HEX RANGE NOT_LETTER EOF;
