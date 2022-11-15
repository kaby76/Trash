# trxml2

## Summary

Print an enumeration of all paths in a parse tree to leaves

## Description

Read an xml file and enumerate all paths to elements in xpath syntax.

## Usage

    trxml2

## Examples

    cat pom.xml | trxml2

## Current version

0.18.2 Fix trperf, trfirst. Adding Xalan code. Fix #180. Fix crash in trgen https://github.com/antlr/grammars-v4/issues/2818. Fix #134. Add -e option to trrename. Update Antlr4BuildTasks version. Fix #197, #198. Fix trparse exit code. Add --quiet option to trparse to just get exit code. Change trgen templates to remove -file option, make file name parsing the default.
