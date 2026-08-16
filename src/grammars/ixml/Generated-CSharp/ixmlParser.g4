/*
 * Parser grammar for Invisible XML (iXML) 1.0
 * https://invisiblexml.org/1.0/
 *
 * Parser rule names mirror those of the iXML specification.
 * The mark/tmark characters ('@', '^', '-') are retained in the
 * parse tree as data rather than being suppressed.
 *
 * Notes on the translation:
 *   - ANTLR4 reserved words 'rule', 'from', 'class', 'string'
 *     are renamed with a trailing underscore.
 *   - 's' (optional whitespace) and 'rs' (required separation)
 *     are empty parser rules: WS and COMMENT go to HIDDEN channel,
 *     so all separation is handled transparently by the lexer.
 *   - 'comment', 'cchar', 'dchar', 'schar', 'whitespace_' are
 *     likewise stubs — the real work is done in the lexer.
 *   - Nested comments are supported via the COMMENT lexer rule.
 *   - '.' is omitted from NAME (see ixmlLexer.g4) and '#' uses
 *     HEX_MODE so hex letters never merge into surrounding NAMEs.
 */
parser grammar ixmlParser;

options { tokenVocab = ixmlLexer; }

// ── Parser rules ─────────────────────────────────────────────────────────────

ixml
    : s prolog? rule_ (rs rule_)* s EOF
    ;

prolog
    : version s
    ;

version
    : IXML_KW rs VERSION_KW rs string_ s DOT
    ;

rule_
    : (mark s)? name s ASSIGN s alts DOT
    ;

mark
    : AT
    | CARET
    | MINUS
    ;

alts
    : alt (ALT_SEP s alt)*
    ;

alt
    : (term_ (COMMA s term_)*)?
    ;

term_
    : factor
    | option
    | repeat0
    | repeat1
    ;

factor
    : terminal_
    | nonterminal
    | insertion
    | LPAREN s alts RPAREN s
    ;

repeat0
    : factor STAR s
    | factor DSTAR s sep
    ;

repeat1
    : factor PLUS s
    | factor DPLUS s sep
    ;

option
    : factor QMARK s
    ;

sep
    : factor
    ;

nonterminal
    : (mark s)? name s
    ;

name
    : NAME
    | IXML_KW
    | VERSION_KW
    | CODE
    ;

terminal_
    : literal
    | charset
    ;

literal
    : quoted
    | encoded
    ;

quoted
    : (tmark s)? string_ s
    ;

tmark
    : CARET
    | MINUS
    ;

string_
    : DQUOTE_STRING
    | SQUOTE_STRING
    ;

// dchar and schar are handled at the lexer level inside DQUOTE_STRING /
// SQUOTE_STRING; these stubs preserve the rule names from the iXML spec.
dchar
    : // ~['"'; #a; #d] | '""'  — bundled into DQUOTE_STRING
    ;

schar
    : // ~["'"; #a; #d] | "''"  — bundled into SQUOTE_STRING
    ;

encoded
    : (tmark s)? HASH hex s
    ;

hex
    : HEX_DIGITS
    ;

charset
    : inclusion
    | exclusion
    ;

inclusion
    : (tmark s)? set_
    ;

exclusion
    : (tmark s)? TILDE s set_
    ;

set_
    : LBRACKET s (member s (ALT_SEP s member s)*)? RBRACKET s
    ;

member
    : string_
    | HASH hex
    | range_
    | class_
    ;

range_
    : from_ s MINUS s to_
    ;

from_
    : character
    ;

to_
    : character
    ;

// The iXML spec constrains character to exactly one dchar/schar,
// but DQUOTE_STRING/SQUOTE_STRING may contain multiple characters;
// single-character validation is a semantic (not syntactic) concern.
character
    : DQUOTE_STRING
    | SQUOTE_STRING
    | HASH hex
    ;

class_
    : code
    ;

code
    : CODE
    ;

insertion
    : PLUS s (string_ | HASH hex) s
    ;

// These rules mirror iXML spec names. Their content is handled by
// the lexer (WS → HIDDEN, COMMENT → HIDDEN).
s
    : // (whitespace | comment)*
    ;

rs
    : // (whitespace | comment)+  — required separation, lexer-enforced
    ;

comment
    : // '{' (cchar | comment)* '}'  — see COMMENT lexer rule
    ;

cchar
    : // ~['{' '}']  — see COMMENT lexer rule
    ;

whitespace_
    : // [\p{Zs}\t\r\n]  — see WS lexer rule
    ;
