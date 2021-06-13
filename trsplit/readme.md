# Trsplit

The split command attempts to split a grammar. The grammar
must be a combined lexer/parser grammar for the transformation to proceed. The
transformation creates a lexer grammar and a parser grammar,
outputed as parse tree data.

# Usage

    trsplit

# Example

    trparse Arithmetic.g4 | trsplit | trsponge

# Current version

0.8.3 -- Updated trfoldlit, trgen and templates, trmvsr, trsponge, trunfold, trxml2
