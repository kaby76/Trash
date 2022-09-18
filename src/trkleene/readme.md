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

0.18.0 Fix 134 for all tools. Fix 180, string-length() and math operations in XPath engine. Fix for crash in https://github.com/antlr/grammars-v4/issues/2818 where _grammar-test was removed, but pom.xml not completely cleaned up of the reference to the directory. Fix Globbing package because of conflict with Antlr4BuildTasks. Update Antlr4BuildTasks version. Rename TreeEdits.Delete() so there is no confuson that it modifies entire parse tree, token and char streams. Add -R option for rename map as file in trrename. Update trrename to use xpath and TreeEdits. Add methods to TreeEdits. Rewrote trrename to use xpath/treeedits packages.
