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

0.18.1 Fix trperf, trfirst. Adding Xalan code. Fix #180. Fix crash in trgen https://github.com/antlr/grammars-v4/issues/2818. Fix #134. Add -e option to trrename. Update Antlr4BuildTasks version. Fix #197, #198. Fix trparse exit code. Add --quiet option to trparse to just get exit code. Change trgen templates to remove -file option, make file name parsing the default.
