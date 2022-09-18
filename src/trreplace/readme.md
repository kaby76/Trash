# trreplace

## Summary

Replace nodes in a parse tree with text

## Description

Reads a parse tree from stdin, replaces nodes with text using
the specified XPath expression, and writes the modified tree
to stdout. The input and output are Parse Tree Data.

## Usage

    trreplace <xpath-string> <text-string>

## Example

    trparse Java.g4 | trreplace "//parserRuleSpec[RULE_REF/text() = 'normalAnnotation']" "nnn" | trtree | vim -

## Notes

If you are running MSYS2 on Windows, you may notice that XPaths are not being
processed by this command correctly. To avoid the Bash shell from altering
XPaths, type _export MSYS2_ARG_CONV_EXCL="*"_, then execute your command.

## Current version

0.18.0 Fix 134 for all tools. Fix 180, string-length() and math operations in XPath engine. Fix for crash in https://github.com/antlr/grammars-v4/issues/2818 where _grammar-test was removed, but pom.xml not completely cleaned up of the reference to the directory. Fix Globbing package because of conflict with Antlr4BuildTasks. Update Antlr4BuildTasks version. Rename TreeEdits.Delete() so there is no confuson that it modifies entire parse tree, token and char streams. Add -R option for rename map as file in trrename. Update trrename to use xpath and TreeEdits. Add methods to TreeEdits. Rewrote trrename to use xpath/treeedits packages.
