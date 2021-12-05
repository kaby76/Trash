# trstrip

Read the parse tree data from stdin and strip the grammar
of all comments, labels, and action blocks.

# Usage

    trstrip

# Examples

    trparse A.g4 | trstrip

# Current version

0.12.0 -- Bug fixes for: parsing result sets reading; update to Antlr 4.9.3; standardize -f, -v options across tools; fix trkleene, trrup, trrename, trparse, trfold, trsponge; remove trmvsr, add trmove.
