# trxml2

## Summary

Print an enumeration of all paths in a parse tree to leaves

## Description

Read an xml file and enumerate all paths to elements in xpath syntax.

## Usage

    dotnet trash xml2 [--file-name] [file ...]

## Examples

    cat desc.xml | dotnet trash xml2

    dotnet trash xml2 --file-name desc.xml other.xml

`--file-name` prefixes each output line with the input argument and a colon,
for example `desc.xml:/desc/targets=CSharp`. For standard input, the prefix is
`<stdin>:`. Without this option, output is unchanged.

## Current version

Release 4.9.0.

## License

The MIT License

Copyright (c) 2026 Ken Domino

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
