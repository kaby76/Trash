# trrup

Remove useless parentheses from a grammar.

# Usage

    trrup
    trrup <xpath>

# Details

`trrup` finds all altLists as specified by the xpath expression in the parsed file,
or the entire file if the xpath expression is not given. Transform the
grammar by removing useless parentheses.

# Example

Consider the following grammar to remove useless parentheses.

_Input to command_

Lexer grammar in ExpressionLexer.g4:

    lexer grammar ExpressionLexer;
    VARIABLE : ( VALID_ID_START VALID_ID_CHAR* ) ;

_Command_

    trparse ExpressionLexer.g4 | trrup | trprint

_Result_

    lexer grammar ExpressionLexer;
    VARIABLE : VALID_ID_START VALID_ID_CHAR* ;

# Current version

0.9.0 -- Updated trsplit, add trrup.
