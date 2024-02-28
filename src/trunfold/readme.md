# trunfold

## Summary

Perform an unfold transform on a grammar

## Description

The unfold command applies the unfold transform to a collection of terminal nodes
in the parse tree, which is identified with the supplied xpath expression. Prior
to using this command, you must have the file parsed. An unfold operation substitutes
the right-hand side of a parser or lexer rule into a reference of the rule name that
occurs at the specified node.

## Usage

    trunfold <string>

## Examples

Before:

	grammar Expresion;
	s : e ;
	e : e '*' e       # Mult
	    | INT           # primary
	    ;
	INT : [0-9]+ ;
	WS : [ \t\n]+ -> skip ;

Command:

    trparse Expression.g4 | trunfold "//parserRuleSpec[RULE_REF/text() = 's']//labeledAlt//RULE_REF[text() = 'e']" | trsponge -c

After:

	grammar Expression;
	s : ( e '*' e | INT ) ;
	e : e '*' e           # Mult
		| INT               # primary
		;
	INT : [0-9]+ ;
	WS : [ \t\n]+ -> skip ;


## Notes

If you are running MSYS2 on Windows, you may notice that XPaths are not being
processed by this command correctly. To avoid the Bash shell from altering
XPaths, type _export MSYS2_ARG_CONV_EXCL="*"_, then execute your command.

## Current version

0.22.1 Synchronized Antlr4 grammar with grammars-v4.

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
