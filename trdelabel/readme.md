# Delabel

Remove all labels from an Antlr4 grammar.

    "expr : lhs=expr (PLUS | MINUS) rhs=expr # foobar1 ....." => "expr : expr (PLUS | MINUS) expr ....."

# Usage

    trdelabel

# Example

    trparse A.g4 | delabel | trprint

# Current version

0.13.0 -- add trgen2; fix trull; rewrite of trkleene base code; rewrite of trgen to include Trash parse of grammars for information extraction.
