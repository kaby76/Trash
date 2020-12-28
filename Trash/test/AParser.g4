parser grammar AParser;

options { tokenVocab=ALexer; }
s : e ;
e : e '*' e | INT ;
