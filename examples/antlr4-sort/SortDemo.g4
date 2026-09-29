parser grammar SortDemo;

options { tokenVocab=SortDemoLexer; }

zeta : Z ;

start : zeta alpha beta EOF ;

beta : B ;

// Keep this comment with alpha.
alpha : A ;
