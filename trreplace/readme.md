# trreplace

Reads a parse tree from stdin, replaces nodes with text using
the specified XPath expression, and writes the modified tree
to stdout. The input and output are Parse Tree Data.

# Usage

    trreplace <xpath-string> <text-string>

# Example

    trparse Java.g4 | trreplace "//parserRuleSpec[RULE_REF/text() = 'normalAnnotation']" "nnn" | trtree | vim -

# Notes

If you are running MSYS2 on Windows, you may notice that XPaths are not being
processed by this command correctly. To avoid the Bash shell from altering
XPaths, type _export MSYS2_ARG_CONV_EXCL="*"_, then execute your command.

# Current version

0.16.4 -- Fixes to Java, Dart, Python, PHP templates in trgen; trxgrep results sent now to stdout; changes to trinsert, -a option; changes to Parsing Result Set data; fixes to trparse, trunfold, trinsert, trreplace, trdelete, add trsort.
