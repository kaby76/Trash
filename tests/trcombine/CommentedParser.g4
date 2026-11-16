/* Parser license header. */
parser grammar CommentedParser;

options { tokenVocab = CommentedLexer; }

start : WORD EOF ;
