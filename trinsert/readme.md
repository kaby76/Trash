# trinsert

Reads a parse tree from stdin, inserts text before
nodes in the tree using
the specified XPath expression, and writes the modified tree
to stdout. The input and output are Parse Tree Data.

# Usage

    trinsert <xpath-string> <text-string>

# Example

    trparse Java.g4 | trinsert "//parserRuleSpec[RULE_REF/text() = 'normalAnnotation']" " /* This is a comment */" | trtree | vim -

# Notes

If you are running MSYS2 on Windows, you may notice that XPaths are not being
processed by this command correctly. To avoid the Bash shell from altering
XPaths, type _export MSYS2_ARG_CONV_EXCL="*"_, then execute your command.

# Current version

0.10.0 -- Updated trsplit, trtree, add trrup, trrr.
