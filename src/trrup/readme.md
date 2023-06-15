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

0.20.27 Major change to trgen templates and processing of STG, ST, and plain files. NB: not all Trash tools supported yet.
