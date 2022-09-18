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

0.18.0 Fix 134 for all tools. Fix 180, string-length() and math operations in XPath engine. Fix for crash in https://github.com/antlr/grammars-v4/issues/2818 where _grammar-test was removed, but pom.xml not completely cleaned up of the reference to the directory. Fix Globbing package because of conflict with Antlr4BuildTasks. Update Antlr4BuildTasks version. Rename TreeEdits.Delete() so there is no confuson that it modifies entire parse tree, token and char streams. Add -R option for rename map as file in trrename. Update trrename to use xpath and TreeEdits. Add methods to TreeEdits. Rewrote trrename to use xpath/treeedits packages.
