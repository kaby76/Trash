# trkleene

Replace a rule with an EBNF form if it contains direct left or direct right recursion.

# Usage

    trkleene <string>?

# Examples

    trparse A.g4 | trkleene
    trparse A.g4 | trkleene "//parserRuleSpec/RULE_REF[text()='packageOrTypeName']"

# Notes

If you are running MSYS2 on Windows, you may notice that XPaths are not being
processed by this command correctly. To avoid the Bash shell from altering
XPaths, type _export MSYS2_ARG_CONV_EXCL="*"_, then execute your command.

# Current version

0.10.0 -- Updated trsplit, trtree, add trrup, trrr.
