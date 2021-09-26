# Trunfold

The unfold command applies the unfold transform to a collection of terminal nodes
in the parse tree, which is identified with the supplied xpath expression. Prior
to using this command, you must have the file parsed. An unfold operation substitutes
the right-hand side of a parser or lexer rule into a reference of the rule name that
occurs at the specified node. The resulting code is parsed and placed on the top of
stack.

# Usage

    trunfold <string>

# Examples

    trparse A.g4 | trunfold "//parserRuleSpec/RULE_REF[text() = 'markerAnnotation']"

# Notes

If you are running MSYS2 on Windows, you may notice that XPaths are not being
processed by this command correctly. To avoid the Bash shell from altering
XPaths, type _export MSYS2_ARG_CONV_EXCL="*"_, then execute your command.

# Current version

0.11.2 -- Updated trgroup.

