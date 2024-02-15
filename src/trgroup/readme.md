# trgroup

## Summary

Perform a group transform on a grammar

## Description

Perform a recursive left- and right- factorization of alternatives for rules.

## Usage

    trgroup <string>

## Details

The command reads all parse tree data. Then, for each parse tree,
the XPath expression argument specified will be evaluated.

The nodes specified in the XPath arg must be for one or more
ruleAltList, lexerAltList, or altList. These node types contain
a sequence of children alternating with an "|"-operator
(`ruleAltList : labeledAlt ('|' labeledAlt)*`,
`lexerAltList : lexerAlt ('|' lexerAlt)*, and
`altList : alternative ('|' alternative)*`).

A "unification" of all the non-'|' children in the node is performed,
which results in a single sequence of elements with groupings. It is
possible for there to be multiple groups in the set of alternatives.

## Examples

_Input to command (file "temp.g4")_

    grammar temp;
    a : 'X' 'B' 'Z' | 'X' 'C' 'Z' | 'X' 'D' 'Z' ;

_Command_

    trparse temp.g4 | trgroup "//parserRuleSpec[RULE_REF/text()='a']//ruleAltList" | trsponge -c true
    
    # Or, a file-wide group refactoring, over all parser and lexer rules:
    
    trparse temp.g4 | trgroup | trsponge -c true

_Output_

    grammar temp;
    a : 'X' ( 'B' | 'C' | 'D' ) 'Z' ;

# Notes

If you are running MSYS2 on Windows, you may notice that XPaths are not being
processed by this command correctly. To avoid the Bash shell from altering
XPaths, type _export MSYS2_ARG_CONV_EXCL="*"_, then execute your command.

## Current version

0.22.0 Add trdot. Update to Antlr 4.13.1 and Dotnet 8.0.

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
