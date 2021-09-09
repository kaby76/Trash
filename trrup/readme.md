# trrup

Remove useless parentheses from a grammar.

# Usage

    trrup
    trrup <xpath>

# Details

`trrup` removes useless parentheses in a grammar, at a specific point
in the grammar as specified by the xpath expression, or the entire
file if the xpath expression is not given.

# Example

_Input to command_

grammar:

    grammar Expression;
    v : ( ( VALID_ID_START  ( VALID_ID_CHAR*) ) ) ;

_Command_

    trparse Expression.g4 | trrup | trprint

_Result_

    grammar Expression;
    v : VALID_ID_START VALID_ID_CHAR* ;

# Current version

0.10.0 -- Updated trsplit, trtree, add trrup, trrr.
