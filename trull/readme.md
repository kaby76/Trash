# Trull

The ulliteral command applies the upper- and lowercase string literal transform
to a collection of terminal nodes in the parse tree, which is identified with the supplied
xpath expression. If the xpath expression is not given, the transform is applied to the
whole file.

# Usage

    trull <xpath>?

# Examples

Command:

    trull "//lexerRuleSpec[TOKEN_REF/text() = 'A']//STRING_LITERAL"

# Notes

If you are running MSYS2 on Windows, you may notice that XPaths are not being
processed by this command correctly. To avoid the Bash shell from altering
XPaths, type _export MSYS2_ARG_CONV_EXCL="*"_, then execute your command.

# Current version

0.12.0 -- Bug fixes for: parsing result sets reading; update to Antlr 4.9.3; standardize -f, -v options across tools; fix trkleene, trrup, trrename, trparse, trfold, trsponge; remove trmvsr, add trmove.
