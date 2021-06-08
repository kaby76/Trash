# Trcombine

Combine two grammars into one grammar.
One grammar must be a lexer grammar, the other a parser grammar,
order is irrelevant. The output is parse tree data.

# Usage

    trcombine <grammar1> <grammar2>

# Example

    trcombine ArithmeticLexer.g4 ArithmeticParser.g4 | trprint > Arithmetic.g4

# Current version

0.8.2 -- Improved help and documentation.
