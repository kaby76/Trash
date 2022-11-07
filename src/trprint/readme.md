# trprint

## Summary

Print a parse tree, including off-token characters

## Description

Read stdin and print out the text for the tree.

## Usage

    trprint

## Examples

    trparse A.g4 | trprint

## Current version

0.18.0 Adding Xalan code. Fix #180. Fix crash in trgen https://github.com/antlr/grammars-v4/issues/2818. Fix #134. Add -e option to trrename. Update Antlr4BuildTasks version. Fix #197, #198. Fix trparse exit code. Add --quiet option to trparse to just get exit code. Change trgen templates to remove -file option, make file name parsing the default.
