# trkleene

Replace a rule with an EBNF form if it contains direct left or direct right recursion.

# Usage

    trkleene <string>?

# Examples

    trparse A.g4 | trkleene
    trparse A.g4 | trkleene " //parserRuleSpec/RULE_REF[text()='packageOrTypeName']"

# Current version

0.9.0 -- Updated trsplit, add trrup.
