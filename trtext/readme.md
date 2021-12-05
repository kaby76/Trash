# trtext

Reads a tree from stdin and prints the source text. If 'line-number' is
specified, the line number range for the tree is printed.

# Usage

    trtext line-number?

# Examples

    trxgrep //lexerRuleSpec | trtext

# Current version

0.12.0 -- Bug fixes for: parsing result sets reading; update to Antlr 4.9.3; standardize -f, -v options across tools; fix trkleene, trrup, trrename, trparse, trfold, trsponge; remove trmvsr, add trmove.
