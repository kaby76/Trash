// Derived from https://www.python.org/dev/peps/pep-0617/
// Tokens assumed to be derived from https://raw.githubusercontent.com/python/cpython/3.10/Grammar/Tokens
// Ken Domino, 2 Sep 2021

parser grammar pegen_v3_10Parser;

options { tokenVocab=pegen_v3_10Lexer; }

start: grammar_ EOF ;

grammar_ : metas? rules ;

metas : meta metas? ;

meta
    : '@' NAME
    | '@' a=NAME b=NAME
    | '@' NAME STRING
    ;

rules : rule_ rules? ;

rule_
//    : rulename memoflag? ':' alts NEWLINE INDENT more_alts DEDENT
//    | rulename memoflag? ':' NEWLINE INDENT more_alts DEDENT
//    | rulename memoflag? ':' alts NEWLINE
    : rulename memoflag? ':' alts // more_alts?
    ;

rulename
    : NAME '[' type=NAME '*'? ']'
    | NAME
    ;

// In the future this may return something more complicated
memoflag : '(' 'memo' ')' ;

alts : '|'? alt ('|' alt)* ;

//more_alts
//    : '|' alts NEWLINE more_alts
//    | '|' alts NEWLINE
//    ;

alt
    : items '$' action
    | items '$'
    | items action
    | items
    ;

items : named_item items? ;

named_item
    : NAME '[' type=NAME '*' ']' '=' item      // ~ item
    | NAME '[' type=NAME ']' '=' item          // ~ item
    | NAME '=' item                            // ~ item
    | item
    | it=forced_atom
    | it2=lookahead
    ;

forced_atom
    : '&''&' atom               // ~ atom
    ;

lookahead
    : '&' atom                  // ~ atom
    | '!' atom                  // ~ atom
    | '~'
    ;

item
    : '[' alts                  // ~ alts
      ']'
    |  atom '?'
    |  atom '*'
    |  atom '+'
    |  sep=atom '.' node=atom '+'
    |  atom
    ;

atom
    : '(' alts                  // ~ alts
      ')'
    | NAME
    | STRING
    ;

// Mini-grammar for the actions

action: '{' target_atoms                    // ~ target_atoms
        '}' ;

target_atoms : target_atom target_atoms? ;

target_atom
    : '{' target_atoms                       // ~ target_atoms
      '}'
    | NAME
    | NUMBER
    | STRING
    | '?'
    | ':'
    | ~'}'
//    | //~
//      '}' OP
    ;

