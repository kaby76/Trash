# trsponge

## Summary

Extract parsing results output of Trash command into files

## Description

Read the parse tree data from stdin and write the
results to file(s).

## Usage

    trsponge <options>

## Example

    trparse Arithmetic.g4 | trsplit | trsponge

## Current version

0.18.2 Fix trperf, trfirst. Adding Xalan code. Fix #180. Fix crash in trgen https://github.com/antlr/grammars-v4/issues/2818. Fix #134. Add -e option to trrename. Update Antlr4BuildTasks version. Fix #197, #198. Fix trparse exit code. Add --quiet option to trparse to just get exit code. Change trgen templates to remove -file option, make file name parsing the default.
