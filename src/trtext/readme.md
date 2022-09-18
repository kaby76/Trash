# trtext

## Summary

Print a parse tree with a specific interval

## Description

Reads a tree from stdin and prints the source text. If 'line-number' is
specified, the line number range for the tree is printed.

## Usage

    trtext line-number?

## Examples

    trxgrep //lexerRuleSpec | trtext

## Current version

0.18.0 Fix 134 for all tools. Fix 180, string-length() and math operations in XPath engine. Fix for crash in https://github.com/antlr/grammars-v4/issues/2818 where _grammar-test was removed, but pom.xml not completely cleaned up of the reference to the directory. Fix Globbing package because of conflict with Antlr4BuildTasks. Update Antlr4BuildTasks version. Rename TreeEdits.Delete() so there is no confuson that it modifies entire parse tree, token and char streams. Add -R option for rename map as file in trrename. Update trrename to use xpath and TreeEdits. Add methods to TreeEdits. Rewrote trrename to use xpath/treeedits packages.
