# trextract

## Summary

Extract all target-specific codes from Antlr4 grammar and write
out macros and target-independent Antlr4 grammar with macros.

## Description

Given parsed .g4 files, output a refactored grammar files that
have all target-specific codes replaced with macro calls. The
trexpand command performs the opposite transformation. The purpose
of the code is to help convert target-agnostic and target-specific
grammars into completely target independent grammars.

## Usage

trparse *.g4 | trextract | trsponge -c

## Details

## Example

Consider the following grammar that is split.

_Input to command_

_Command_

trparse ExpressionLexer.g4 ExpressionParser.g4 | trextract | trsponge -c

The outputed files are:

## Current version

0.23.27 Added trff.

## License

The MIT License

Copyright (c) 2025 Ken Domino

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
