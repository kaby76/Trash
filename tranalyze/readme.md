# tranalyze

Reads an Antlr4 grammar in the form of parse tree data from stdin,
searches for problems in the grammar, and outputs the results to stdout.

# Usage

    tranalyze

# Details

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

# Example

Consider the following combined grammar.

_Input to command_

	grammar Test;

	start : 'a';
	unused : 'b';
	infinite : (infinite 'c')+ ;
	empty : ;

_Command_

    trparse Test.g4 | tranalyze

_Output_

	4 occurrences of a Antlr - nonterminal def
	1 occurrences of a Antlr - nonterminal ref
	1 occurrences of a Antlr - keyword
	3 occurrences of a Antlr - literal
	Rule start is NonEmpty
	Rule unused is NonEmpty
	Rule infinite is NonEmpty
	Rule empty is Empty
	Rule infinite is malformed. It does not derive a string with referencing itself.

# Current version

0.8.4 -- Updated tranalyze with detection of infinite recursion within rule. Updated basic graph implementations.
