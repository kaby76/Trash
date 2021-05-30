# Tranalyze

Reads an Antlr4 grammar from stdin and identifies problems in the grammar.
The input is Parse Tree Data, and output text.

# Usage

    tranalyze

# Example

    trparse -f A.g4 | tranalyze

# Current version

0.8.1 -- Improved help and documentation.


Combine two grammars on top of stack into one grammar.
One grammar must be a lexer grammar, the other a parser grammar,
order is irrelevant.

Example:
    (top of stack contains a lexer file and a parser file, both parsed.)
    combine
