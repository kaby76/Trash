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

0.16.4 -- Fixes to Java, Dart, Python, PHP templates in trgen; trxgrep results sent now to stdout; changes to trinsert, -a option; changes to Parsing Result Set data; fixes to trparse, trunfold, trinsert, trreplace, trdelete, add trsort.
