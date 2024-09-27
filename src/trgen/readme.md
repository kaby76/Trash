# trgen

## Summary

Generate an Antlr4 parser for a given target language

## Description

Generate a parser application using the Antlr tool and application templates.
The generated parser is placed in the directory <current-directory>/Generated-<target>/.
If there is a `desc.xml` file in the current directory, `trgen` will read
it for information on testing the grammar. If there is no `desc.xml` file,
the start rule must be provided. If the current directory is empty, `trgen` will
create a parser for the Arithmetic.g4 grammar.

## Usage

    trgen <options>* 

## Examples

    trgen

## Specification of desc.xml

The desc.xml is an XML file that specifies how to generate a driver and test
a grammar. The desc.xml is similar to the Antlr Maven Tester pom.xml file,
but with a few extension. trgen analyzes the grammar to determine how to test
the grammar; this analysis simplifies the information needed in the desc.xml
that would be required in the pom.xml.

A typical desc.xml simply specifies all targets to test.

```
<?xml version="1.0" encoding="UTF-8" ?>
<desc xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="../_scripts/desc.xsd">
   <targets>Antlr4cs;Antlr4ng;Cpp;CSharp;Dart;Go;Java;JavaScript;PHP;Python3;TypeScript</targets>
</desc>
```
trgen will read all .g4's contained in the directory with the desc.xml file.
It computes a dependency graph of the grammars used via `tokenVocab` options,
and `import` statements. The tool will then perform a topological sort, determine
the first "top-level" parser or combined grammar, and use that grammar for testing.
The start rule for the driver for testing will be determined by looking for the
first EOF-terminated rule. With these two analyses, the desc.xml eliminates the
need to specify information that is required in the pom.xml, and helps to greatly
simplify and eliminate bugs created when adding new grammars.


## Current version

0.23.7 Fixed trcover when started outside of Generated directory.

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
