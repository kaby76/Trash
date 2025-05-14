# trgenvsc

## Summary

Generate a VSCode extension and LSP server for an Antlr4 grammar.

## Description

Generates a Visual Studio Code extension from an Antlr4 grammar. A CSharp
target must be available for the grammar as the LSP server is implemented
in C#.

After generating the extension/server, proceed to building the extension
by changing to the "Generated-vsc/" directory, then running "bash install.sh".
The install.sh script performs the build of the C# server, and the TypeScript
extension. The script will also create a settings.rc JSON file that describes
the syntactic highlighting for the grammar. The script calls an auxilary Python
program to examine the grammar for single-character string literals, and using
the Unicode category to group these in a common class with a particular color.
You can modify the settings.rc file to perform semantic highlighting by adjusting
the XPath patterns to match certain parse trees.

## Usage

    trgenvsc <options>* 

## Examples

    trgenvsc

## Current version

0.23.18 General clean up and bug fixes.

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
