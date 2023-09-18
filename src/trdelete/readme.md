# trdelete

## Summary

Delete nodes in a parse tree and reposition children to parents

## Description

Reads a parse tree from stdin, and given an XPath expression
for the nodes in the parse tree, delete the nodes and reattach
the children of the deleted nodes to the parent. Finally,
write the modified tree to stdout. The input and output are
Parse Tree Data.

## Usage

    trdelete <string>

## Notes

If you are running MSYS2 on Windows, you may notice that XPaths are not being
processed by this command correctly. To avoid the Bash shell from altering
XPaths, type _export MSYS2_ARG_CONV_EXCL="*"_, then execute your command.

## Current version

0.21.6 Fix trquery.
