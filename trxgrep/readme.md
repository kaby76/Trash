# trxgrep

Find all sub-trees in a parse tree using the given XPath expression.

# Usage

    trxgrep <string>

# Examples

    trparse A.g4 | trxgrep " //parserRuleSpec[RULE_REF/text() = 'normalAnnotation']"

# Current version

0.8.4 -- Updated tranalyze with detection of infinite recursion within rule. Updated basic graph implementations.
