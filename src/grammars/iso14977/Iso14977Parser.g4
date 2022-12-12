parser grammar Iso14977Parser;

// https://www.iso.org/standard/26153.html
// https://dwheeler.com/essays/dont-use-iso-14977-ebnf.html
// https://www.grammarware.net/text/2012/bnf-was-here.pdf

options
{
    tokenVocab = Iso14977Lexer;
    contextSuperClass=AttributedParseTreeNode;
}

/*
The syntax of Extended BNF can be defined using
itself. There are four parts in this example,
the first part names the characters, the second
part defines the removal of unnecessary nonprinting
characters, the third part defines the
removal of textual comments, and the final part
defines the structure of Extended BNF itself.

Each syntax rule in this example starts with a
comment that identifies the corresponding clause
in the standard.

The meaning of special-sequences is not defined
in the standard. In this example (see the
reference to 7.6) they represent control
functions defined by ISO/IEC 6429:1992.
Another special-sequence defines a
syntactic-exception (see the reference to 4.7).
*/

/*
The first part of the lexical syntax defines the
characters in the 7-bit character set (ISO/IEC
646:1991) that represent each terminal-character
and gap-separator in Extended BNF.
*/

/* see 7.2 */ letter :
'a' | 'b' | 'c' | 'd' | 'e' | 'f' | 'g' | 'h'
| 'i' | 'j' | 'k' | 'l' | 'm' | 'n' | 'o' | 'P'
| 'q' | 'r' | 's' | 't' | 'u' | 'v' | 'w' | 'x'
| 'y' | 'z'
| 'A' | 'B' | 'C' | 'D' | 'E' | 'F' | 'G' | 'H'
| 'I' | 'J' | 'K' | 'L' | 'M' | 'N' | 'O' | 'P'
| 'Q' | 'R' | 'S' | 'T' | 'U' | 'V' | 'W' | 'X'
| 'Y' | 'Z'
;

/* see 7.2 */ decimal_digit
: '0' | '1' | '2' | '3' | '4' | '5' | '6' | '7'
| '8' | '9'
;

/*
The representation of the following
terminal-characters is defined in clauses 7.3,
7.4 and tables 1, 2.
*/

concatenate_symbol : ',' ;
defining_symbol : '=' ;
definition_separator_symbol : '|' | '/' | '!' ;
end_comment_symbol : '*)' ;
end_group_symbol : ')' ;
end_option_symbol : ']' | '/)' ;
end_repeat_symbol : '}' | ':)' ;
except_symbol : '-' ;
first_quote_symbol : '\'' | '�' ;
repetition_symbol : '*' ;
second_quote_symbol : '"' ;
special_sequence_symbol : '?' ;
start_comment_symbol : '(*' ;
start_group_symbol : '(' ;
start_option_symbol : '[' | '(/' ;
start_repeat_symbol : '{' | '(:' ;
terminator_symbol : ';' | '.' ;
/* see 7.5 */ other_character
: ' ' | ':' | '+' | '_' | '%' | '@'
| '&' | '#' | '$' | '<' | '>' | '\\'
| '^' | '`' | '~' ;

/* see 7.6 */ space_character : ' ' ;

horizontal_tabulation_character
: '\t' ;

new_line
: ('\n' | '\r')+ ;

vertical_tabulation_character
: '\r' ;

form_feed
: '\n' ;

/*
The second part of the syntax defines the
removal of unnecessary non-printing characters
from a syntax.
*/

/* see 6.2 */ terminal_character
: letter
| decimal_digit
| concatenate_symbol
| defining_symbol
| definition_separator_symbol
| end_comment_symbol
| end_group_symbol
| end_option_symbol
| end_repeat_symbol
| except_symbol
| first_quote_symbol
| repetition_symbol
| second_quote_symbol
| special_sequence_symbol
| start_comment_symbol
| start_group_symbol
| start_option_symbol
| start_repeat_symbol
| terminator_symbol
| other_character
;

/* see 6.3 */ gap_free_symbol
: // terminal_character - (first_quote_symbol | second_quote_symbol)
{ !(
    InputStream.LA(1) == SQ
    || InputStream.LA(1) == FSQ
    || InputStream.LA(1) == DQ
    ) }?
    terminal_character
| terminal_string
;

/* see 4.16 */ terminal_string
: first_quote_symbol first_terminal_character+ first_quote_symbol
| second_quote_symbol second_terminal_character+ second_quote_symbol
;

/* see 4.17 */ first_terminal_character
: // terminal character - first_quote_symbol;
{ !(
    InputStream.LA(1) == SQ
    || InputStream.LA(1) == FSQ
    ) }?
    terminal_character
;

/* see 4.18 */ second_terminal_character
: // terminal_character - second_quote_symbol
{ !(
    InputStream.LA(1) == DQ
    ) }?
    terminal_character
;

/* see 6.4 */ gap_separator
: space_character
| horizontal_tabulation_character
| new_line
| vertical_tabulation_character
| form_feed
;

/* see 6.5 */ syntax1
: gap_separator* (gap_free_symbol gap_separator*)+ EOF
;

/*
The third part of the syntax defines the
removal of bracketed-textual-comments from
gap-free-symbols that form a syntax.
*/

/* see 6.6 */ commentless_symbol
:   // terminal_character
    //   - (letter
    //    | decimal_digit
    //    | first_quote_symbol
    //    | second_quote_symbol
    //    | start_comment_symbol
    //    | end_comment_symbol
    //    | special_sequence_symbol
    //    | other_character)
    { !(
    (InputStream.LA(1) >= Al && InputStream.LA(1) <= Zl)
    || (InputStream.LA(1) >= Au && InputStream.LA(1) <= Zu)

    || (InputStream.LA(1) >= N0 && InputStream.LA(1) <= N9)

    || InputStream.LA(1) == SQ
    || InputStream.LA(1) == FSQ

    || InputStream.LA(1) == DQ

    || InputStream.LA(1) == STARCP

    || InputStream.LA(1) == OPSTAR

    || InputStream.LA(1) == QM
    || InputStream.LA(1) == COLON
    || InputStream.LA(1) == PLUS
    || InputStream.LA(1) == UNDERSCORE
    || InputStream.LA(1) == PERCENT
    || InputStream.LA(1) == AT
    || InputStream.LA(1) == AMP
    || InputStream.LA(1) == POUND
    || InputStream.LA(1) == DOLLAR
    || InputStream.LA(1) == POUND
    || InputStream.LA(1) == LT
    || InputStream.LA(1) == GT
    || InputStream.LA(1) == BSLASH
    || InputStream.LA(1) == XOR
    || InputStream.LA(1) == BQUOTE
    || InputStream.LA(1) == TILDE
    ) }?
    terminal_character
| meta_identifier
| integer
| terminal_string
| special_sequence
;

/* see 4.9 */ integer
: decimal_digit decimal_digit*
;

/* see 4.14 */ meta_identifier
: letter meta_identifier_character*
;

/* see 4.15 */ meta_identifier_character
: letter
| decimal_digit
| space_character // KED
;

/* see 4.19 */ special_sequence
: special_sequence_symbol
special_sequence_character*
special_sequence_symbol
;

/* see 4.20 */ special_sequence_character
: // terminal_character - special_sequence_symbol
{ !(
    InputStream.LA(1) == QM
    ) }?
    terminal_character
;

/* see 6.7 */ comment_symbol
: bracketed_textual_comment
| other_character
| commentless_symbol
;

/* see 6.8 */ bracketed_textual_comment
: start_comment_symbol comment_symbol* end_comment_symbol
;

/* see 6.9 */ syntax2
: bracketed_textual_comment*
commentless_symbol
bracketed_textual_comment*
(commentless_symbol bracketed_textual_comment*)*
EOF
;

/*
The final part of the syntax defines the
abstract syntax of Extended BNF, i.e. the
structure in terms of the commentless symbols.
*/

/* see 4.2 */ syntax3
: syntax_rule syntax_rule* EOF
;

/* see 4.3 */ syntax_rule
: meta_identifier defining_symbol
definitions_list terminator_symbol
;

/* see 4.4 */ definitions_list
: single_definition
  (definition_separator_symbol
    single_definition)*
;

/* see 4.5 */ single_definition
: syntactic_term
  (concatenate_symbol syntactic_term)*
;

/* see 4.6 */ syntactic_term
: syntactic_factor
  (except_symbol syntactic_exception)?
;

/* see 4.7 */ syntactic_exception
: syntactic_factor
//? a syntactic-factor that could be replaced
//by a syntactic-factor containing no
//meta-identifiers
// ?
;

/* see 4.8 */ syntactic_factor
: (integer repetition_symbol)?
  syntactic_primary
;

/* see 4.10 */ syntactic_primary
: optional_sequence
| repeated_sequence
| grouped_sequence
| meta_identifier
| terminal_string
| special_sequence
| empty_sequence
;

/* see 4.11 */ optional_sequence
: start_option_symbol definitions_list
  end_option_symbol
;

/* see 4.12 */ repeated_sequence
: start_repeat_symbol definitions_list
  end_repeat_symbol
;

/* see 4.13 */ grouped_sequence
: start_group_symbol definitions_list
  end_group_symbol
;

/* see 4.21 */ empty_sequence
:
;
