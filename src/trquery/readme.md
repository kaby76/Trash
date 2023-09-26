# trquery

## Summary

Execute a simple query of tree modifications.

## Description

Reads a parse tree from stdin, executes a list
of queries (insert, delete, or replace), 
and write the modified tree
to stdout. The input and output are Parse Tree Data.

## Usage

trquery insert xpath-expr string (; additional commands...)*
trquery delete xpath-expr (; additional commands...)*
trquery replace xpath-expr string (; additional commands...)*

## Details

For all commands, an XPath expression defines where an operation occurs.
Commands are executed in order as they appear. The advantage of this
command is that the parse tree does not have be re-read or written after
each operation as with trinsert, trdelete, or trreplace.

## Example

## Notes

If you are running MSYS2 on Windows, you may notice that XPaths are not being
processed by this command correctly. To avoid the Bash shell from altering
XPaths, type _export MSYS2_ARG_CONV_EXCL="*"_, then execute your command.

## Current version

0.21.7 Fix trgen globstar testing, Python3 target.
