# trstrip

## Summary

Strip a grammar of all actions, labels, etc.

## Description

Read the parse tree data from stdin and strip the grammar
of all comments, labels, and action blocks.

## Usage

    trstrip

## Examples

    trparse A.g4 | trstrip

## Current version

0.18.0 Adding Xalan code. Fix #180. Fix crash in trgen https://github.com/antlr/grammars-v4/issues/2818. Fix #134. Add -e option to trrename. Update Antlr4BuildTasks version. Fix #197, #198. Fix trparse exit code. Add --quiet option to trparse to just get exit code. Change trgen templates to remove -file option, make file name parsing the default.
