# trreplace

Reads a parse tree from stdin, replaces nodes with text using
the specified XPath expression, and writes the modified tree
to stdout. The input and output are Parse Tree Data.

# Usage

    trreplace <xpath-string> <text-string>

# Example

    trparse Java.g4 | trreplace " //parserRuleSpec[RULE_REF/text() = 'normalAnnotation']" "nnn" | trtree | vim -

# Current version

0.10.0 -- Updated trsplit, trtree, add trrup, trrr.
