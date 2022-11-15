# trrename

## Summary

Rename symbols in a grammar

## Description

Rename symbols in a grammar.

## Usage

    trrename -r <string>

## Details

`trrename` renames rule symbols in a grammar.

The `-r` option is required. It
is a list of semi-colon delimited pairs of symbol names, which are separated
by a comma, e.g., `id,identifier;name,name_`. If you are using Bash,
make sure to enclose the argument as it contains semi-colons.

## Examples

    trparse Foobar.g4 | trrename -r "a,b;c,d" | trprint > new-grammar.g4

## Current version

0.18.2 Fix trperf, trfirst. Adding Xalan code. Fix #180. Fix crash in trgen https://github.com/antlr/grammars-v4/issues/2818. Fix #134. Add -e option to trrename. Update Antlr4BuildTasks version. Fix #197, #198. Fix trparse exit code. Add --quiet option to trparse to just get exit code. Change trgen templates to remove -file option, make file name parsing the default.
