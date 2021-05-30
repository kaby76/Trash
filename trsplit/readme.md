# Trsplit

The split command attempts to split a grammar. The grammar
must be a combined lexer/parser grammar for the transformation to proceed. The
transformation creates a lexer grammar and a parser grammar,
outputed as parse tree data.

# Usage

    trsplit <grammar>

# Example

    trsplit Arithmetic.g4 | trtee

# Current version

0.8.1 -- Improved help and documentation.
