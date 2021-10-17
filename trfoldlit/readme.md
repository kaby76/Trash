# Trfoldlit

Reads a parse tree from stdin, replaces a sequence of symbols on
the RHS of a rule with the rule LHS symbol, and writes the modified tree
to stdout. The input and output are Parse Tree Data.

# Usage

    trfold <string>

# Examples

    trparse Foo.g4 | trfoldlit " //lexerRuleSpec/TOKEN_REF"

# Notes

If you are running MSYS2 on Windows, you may notice that XPaths are not being
processed by this command correctly. To avoid the Bash shell from altering
XPaths, type _export MSYS2_ARG_CONV_EXCL="*"_, then execute your command.

# Current version

0.11.5 -- Bug fixes in trkleene, trrup.
