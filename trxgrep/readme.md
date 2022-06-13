# trxgrep

Find all sub-trees in a parse tree using the given XPath expression.

# Usage

    trxgrep <string>

# Examples

    trparse A.g4 | trxgrep "//parserRuleSpec[RULE_REF/text() = 'normalAnnotation']"

# Notes

If you are running MSYS2 on Windows, you may notice that XPaths are not being
processed by this command correctly. To avoid the Bash shell from altering
XPaths, type _export MSYS2_ARG_CONV_EXCL="*"_, then execute your command.

# Current version

0.16.4 -- Fixes to Java, Dart, Python, PHP templates in trgen; trxgrep results sent now to stdout; changes to trinsert, -a option; changes to Parsing Result Set data; fixes to trparse, trunfold, trinsert, trreplace, trdelete, add trsort.
