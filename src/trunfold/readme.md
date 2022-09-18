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

0.18.0 Fix 134 for all tools. Fix 180, string-length() and math operations in XPath engine. Fix for crash in https://github.com/antlr/grammars-v4/issues/2818 where _grammar-test was removed, but pom.xml not completely cleaned up of the reference to the directory. Fix Globbing package because of conflict with Antlr4BuildTasks. Update Antlr4BuildTasks version. Rename TreeEdits.Delete() so there is no confuson that it modifies entire parse tree, token and char streams. Add -R option for rename map as file in trrename. Update trrename to use xpath and TreeEdits. Add methods to TreeEdits. Rewrote trrename to use xpath/treeedits packages.

