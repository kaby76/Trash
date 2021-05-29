# Trconvert

Reads a grammar from stdin and converts the grammar to Antlr version 4
syntax. The original grammar must be for a supported type (Antlr2, Antlr3,
Bison, or W3C EBNF). The input and output are Parse Tree Data.

# Usage

    trconvert

# Examples

    trparse -f A.g2 -t antlr2 | trconvert | trprint > A.g4

# Current version

0.8.1 -- Improved help and documentation.
