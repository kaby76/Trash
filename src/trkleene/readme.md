# trkleene

## Summary

Perform a Kleene transform of a grammar

## Description

Replace a rule with an EBNF form if it contains direct left or direct right recursion.

## Usage

    trkleene <string>?

## Details

`trkleene` refactors rules in a grammar with direct left or direct right
recursion. The program first reads from stdin the parse tree data of
grammar files(x). It then searches
the parse tree for the nodes identified by the XPath expression argument
or if none given, all parser and lexer rules in the grammar.
The XPath argument can select any node for the rule (e.g., the LHS symbol,
any RHS symbol, the colon in the rule, etc). The program will finally
replace the RHS of each rule selected with a "Kleene" version of the rule,
removing the recursion. The updated grammar(s) as parse tree data
is outputed to stdout.

## Examples

    trparse A.g4 | trkleene
    trparse A.g4 | trkleene "//parserRuleSpec/RULE_REF[text()='packageOrTypeName']"

## Notes

If you are running MSYS2 on Windows, you may notice that XPaths are not being
processed by this command correctly. To avoid the Bash shell from altering
XPaths, type _export MSYS2_ARG_CONV_EXCL="*"_, then execute your command.

## Current version

0.23.8 

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
