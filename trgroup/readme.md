# Trgroup

Perform a recursive left- and right- factorization of alternatives for rules.
The nodes specified must be for ruleAltList, lexerAltList, or altList. A common
prefix and suffix is performed on the alternatives, and a new expression derived.
The process repeats for alternatives nested.

# Usage

    trgroup <string>

# Examples

    trparse A.g4 | trgroup "//parserRuleSpec[RULE_REF/text()='additiveExpression']//altList"

# Notes

If you are running MSYS2 on Windows, you may notice that XPaths are not being
processed by this command correctly. To avoid the Bash shell from altering
XPaths, type _export MSYS2_ARG_CONV_EXCL="*"_, then execute your command.

# Current version

0.11.0 -- Updated trkleen. Added trreplace.
