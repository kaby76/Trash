parser grammar RootParser;

options { tokenVocab = WordLexer; }
import StatementParser;

root : start ;
