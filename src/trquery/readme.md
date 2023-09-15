# trquery

## Summary

Execute a simple query of tree modifications.

## Description

Reads a parse tree from stdin, and a query file via options,
then execute the query, which modifies the parse tree,
and write the modified tree
to stdout. The input and output are Parse Tree Data.

## Usage

trquery <file-name>

## Details

## Example

## Notes

If you are running MSYS2 on Windows, you may notice that XPaths are not being
processed by this command correctly. To avoid the Bash shell from altering
XPaths, type _export MSYS2_ARG_CONV_EXCL="*"_, then execute your command.

## Current version

0.21.4 Fix trcover.
