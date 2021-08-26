# Trgroup

Perform a recursive left- and right- factorization of alternatives for rules.
The nodes specified must be for ruleAltList, lexerAltList, or altList. A common
prefix and suffix is performed on the alternatives, and a new expression derived.
The process repeats for alternatives nested.

# Usage

    trgroup <string>

# Examples

    trparse A.g4 | trgroup " //parserRuleSpec[RULE_REF/text()='additiveExpression']//altList"

# Current version

0.9.0 -- Updated trsplit.
