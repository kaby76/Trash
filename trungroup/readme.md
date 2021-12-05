# trungroup

Perform an ungroup transformation of the 'element' node(s) specified by the string.

# Usage

    trungroup <string>

# Examples

    trparse A.g4 | trungroup "//parserRuleSpec[RULE_REF/text() = 'a']//ruleAltList"

# Notes

If you are running MSYS2 on Windows, you may notice that XPaths are not being
processed by this command correctly. To avoid the Bash shell from altering
XPaths, type _export MSYS2_ARG_CONV_EXCL="*"_, then execute your command.

# Current version

0.12.0 -- Bug fixes for: parsing result sets reading; update to Antlr 4.9.3; standardize -f, -v options across tools; fix trkleene, trrup, trrename, trparse, trfold, trsponge; remove trmvsr, add trmove.
