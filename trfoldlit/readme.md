# Trfoldlit

Reads a parse tree from stdin, replaces a sequence of symbols on
the RHS of a rule with the rule LHS symbol, and writes the modified tree
to stdout. The input and output are Parse Tree Data.

# Usage

    trfold <string>

# Examples

    trparse Foo.g4 | trfoldlit " //lexerRuleSpec/TOKEN_REF"

# Current version

0.10.0 -- Updated trsplit, trtree, add trrup, trrr.
