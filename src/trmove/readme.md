# trmove

## Summary

Move nodes in a parse tree

## Description

Reads a parse tree from stdin, moves
nodes in the tree using
the specified XPath expression, and writes the modified tree
to stdout. The input and output are Parse Tree Data.

## Usage

    trmove <-a>? <xpath-string-1> <xpath-string-2>

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

0.19.0-alpha6 Complete rewrite of parse tree representation.
