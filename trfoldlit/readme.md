# Trfoldlit

Reads a parse tree from stdin, replaces a sequence of symbols on
the RHS of a rule with the rule LHS symbol, and writes the modified tree
to stdout. The input and output are Parse Tree Data.

# Usage

    trfold <string>

# Examples

    trparse --file foo.g4 | refoldlit ""//lexerRuleSpec/TOKEN_REF""

# Current version

0.8.1 -- Improved help and documentation.
