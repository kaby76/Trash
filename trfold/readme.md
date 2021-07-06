# Trfold

Reads a parse tree from stdin, replaces a sequence of symbols on
the RHS of a rule with the rule LHS symbol, and writes the modified tree
to stdout. The input and output are Parse Tree Data.

# Usage

    trfold <string>

# Example

    trparse A.g4 | trfold " //parserRuleSpec[RULE_REF/text() = 'normalAnnotation']"

# Current version

0.8.5 -- Updated trdot, trthompson, trparse (Lark grammar), trconvert, tranalyze, trcombine, trsplit.
