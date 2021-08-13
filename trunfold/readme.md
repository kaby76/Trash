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

# Current version

0.8.9 -- Updated all app, but especially for tranalyze, trconvert, trformat, trgen, trrename.

