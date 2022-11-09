# trtree

## Summary

Print a parse tree in a human-readable format

## Description

Reads a tree from stdin and prints the tree as an indented node list.

## Usage

    trtree

## Examples

    trparse A.g4 | trtree

## Current version

0.18.1 Fix trperf, trfirst. Adding Xalan code. Fix #180. Fix crash in trgen https://github.com/antlr/grammars-v4/issues/2818. Fix #134. Add -e option to trrename. Update Antlr4BuildTasks version. Fix #197, #198. Fix trparse exit code. Add --quiet option to trparse to just get exit code. Change trgen templates to remove -file option, make file name parsing the default.
