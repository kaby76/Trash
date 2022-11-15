# trrup

## Summary

Remove useless parentheses in a grammar

## Description

Remove useless parentheses from a grammar.

## Usage

    trrup

## Details

`trrup` removes useless parentheses in a grammar.

## Example

_Input to command_

grammar:

    grammar Expression;
    v : ( ( VALID_ID_START  ( VALID_ID_CHAR*) ) ) ;

_Command_

    trparse Expression.g4 | trrup | trprint

_Result_

    grammar Expression;
    v : VALID_ID_START VALID_ID_CHAR* ;

## Notes

If you are running MSYS2 on Windows, you may notice that XPaths are not being
processed by this command correctly. To avoid the Bash shell from altering
XPaths, type _export MSYS2_ARG_CONV_EXCL="*"_, then execute your command.

## Current version

0.18.2 Fix trperf, trfirst. Adding Xalan code. Fix #180. Fix crash in trgen https://github.com/antlr/grammars-v4/issues/2818. Fix #134. Add -e option to trrename. Update Antlr4BuildTasks version. Fix #197, #198. Fix trparse exit code. Add --quiet option to trparse to just get exit code. Change trgen templates to remove -file option, make file name parsing the default.
