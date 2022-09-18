# trinsert

## Summary

Insert string into points in a parse tree

## Description

Reads a parse tree from stdin, inserts text before or after
nodes in the tree using
the specified XPath expression, and writes the modified tree
to stdout. The input and output are Parse Tree Data.

## Usage

    trinsert <-a>? <xpath-string> <text-string>

## Details

The command reads all parse tree data. Then, for each parse tree,
the XPath expression argument specified will be evaluated.

The nodes specified in the XPath arg are for one or more
nodes of any type in a parse tree of any type.

For each node, the program inserts a string node in the parent's
list of children nodes prior to the node. Off-channel tokens occur
before the inserted text. If you specify the `-a` option, the text
is inserted after the node.

After performing the insert, if it is a grammar, the text is reparsed
and an entire new parse tree outputed.

## Example

    trparse Java.g4 | trinsert "//parserRuleSpec[RULE_REF/text() = 'normalAnnotation']" " /* This is a comment */" | trtree | vim -

## Notes

If you are running MSYS2 on Windows, you may notice that XPaths are not being
processed by this command correctly. To avoid the Bash shell from altering
XPaths, type _export MSYS2_ARG_CONV_EXCL="*"_, then execute your command.

## Current version

0.18.0 Fix 134 for all tools. Fix 180, string-length() and math operations in XPath engine. Fix for crash in https://github.com/antlr/grammars-v4/issues/2818 where _grammar-test was removed, but pom.xml not completely cleaned up of the reference to the directory. Fix Globbing package because of conflict with Antlr4BuildTasks. Update Antlr4BuildTasks version. Rename TreeEdits.Delete() so there is no confuson that it modifies entire parse tree, token and char streams. Add -R option for rename map as file in trrename. Update trrename to use xpath and TreeEdits. Add methods to TreeEdits. Rewrote trrename to use xpath/treeedits packages.
