# tranalyze

## Summary

Analyze a grammar

## Description

Reads an Antlr4 grammar in the form of parse tree data from stdin,
searches for problems in the grammar, and outputs the results to stdout.

## Usage

    tranalyze

## Details

`tranalyze` performs a multi-pass search of a grammar in the
form of a parse result (from `trparse`), looking for problems in the
grammar.

* Classify each node type and output a count for each type.
* Check for unused symbols.
* Check for Unicode literals of the type '\unnnn' with
numbers that are over 32-bits.
* Check for common groupings in alts.
* Check for useless parentheses.
* Identify if a symbol derives the empty string, a non-empty string, or both.
* Check for unhalting nonterminals symbols in a single rule or group of rules.

## Example

Consider the following combined grammar.

_Input to command_

	grammar Test;

	start : 'a' empty infinite0 infinite1 infinite2 ;
	unused : 'b';
	infinite0 : (infinite0 'c')* ;  // Not legal in Antlr4 MLR, but okay
	infinite1 : (infinite1 'c')+ ;  // Not legal in Antlr4 MLR, infinite
	infinite2 : ('c' infinite2)+ ;  // Not flagged by Antlr4, infinite
	empty : ;

_Command_

    trparse Test.g4 | tranalyze

_Output_

	6 occurrences of Antlr - nonterminal def
	7 occurrences of Antlr - nonterminal ref
	1 occurrences of Antlr - keyword
	5 occurrences of Antlr - literal
	Rule start is NonEmpty
	Rule unused is NonEmpty
	Rule infinite0 is NonEmpty
	Rule infinite1 is NonEmpty
	Rule infinite2 is NonEmpty
	Rule empty is Empty
	Rule infinite1 is malformed. It does not derive a string with referencing itself.
	Rule infinite2 is malformed. It does not derive a string with referencing itself.


## Current version

0.23.24 Fix problem with file names for input streams using trparse.

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
