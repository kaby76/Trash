# trungroup

Perform an ungroup transformation of the 'element' node(s) specified by the string.

# Usage

    trungroup <string>

# Examples

    trparse A.g4 | trungroup " //parserRuleSpec[RULE_REF/text() = 'a']//ruleAltList"

# Current version

0.10.0 -- Updated trsplit, trtree, add trrup, trrr.
