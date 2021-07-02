# Trconvert

Reads a grammar from stdin and converts the grammar to/from Antlr version 4
syntax. The original grammar must be for a supported type (Antlr2, Antlr3,
Bison, W3C EBNF, Lark). The input and output are Parse Tree Data.

# Usage

    trconvert [-t <type>]

# Details

This command converts a grammar from one type to another. Most
conversions will handle only simple syntax differences. More complicated
scenarios are supported depending on the source and target grammar types.
For example, Bison is converted to Antlr4, but the reverse is not
implemented yet.

`trconvert` takes an option target type. If it is not used, the default
is to convert the input of whatever type to Antlr4 syntax. The output
of `trconvert` is a parse tree containing the converted grammar.

# Examples

    trparse A.g2 | trconvert | trprint > A.g4

# Current version

0.8.4 -- Updated tranalyze with detection of infinite recursion within rule. Updated basic graph implementations.
