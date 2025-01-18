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

If '-c' option specified, read commands from the file. Otherwise,
the command line args contain the commands.

For all commands, an XPath expression defines where an operation occurs.
Commands are executed in order as they appear. The advantage of this
command is that the parse tree does not have be re-read or written after
each operation as with trinsert, trdelete, or trreplace.


### Commands

grep <xpath> match-required?
insert (before|after)? <xpath> match-required? <string>
delete <xpath> match-required?
delete-reattach <xpath> match-required?
replace <xpath> match-required <string>
move (before|after)? <xpath> match-required? <xpath>

## Examples

## Notes

If you are running MSYS2 on Windows, you may notice that XPaths are not being
processed by this command correctly. To avoid the Bash shell from altering
XPaths, type _export MSYS2_ARG_CONV_EXCL="*"_, then execute your command.

## Current version

0.23.13 Fixes for #518, #524, #525, #527. Fix Go target trgen.

## License

The MIT License

Copyright (c) 2024 Ken Domino

Permission is hereby granted, free of charge, 
to any person obtaining a copy of this software and 
associated documentation files (the "Software"), to 
deal in the Software without restriction, including 
without limitation the rights to use, copy, modify, 
merge, publish, distribute, sublicense, and/or sell 
copies of the Software, and to permit persons to whom 
the Software is furnished to do so, 
subject to the following conditions:

The above copyright notice and this permission notice 
shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR 
ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
