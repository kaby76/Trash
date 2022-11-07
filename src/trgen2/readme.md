# trgen2

## Summary

Generate files from template and XML doc list.

## Description

Generate files from template and a parameterize list of names and values.
The generated parser is placed in the directory <current-directory>/Generated/.

## Usage

    trgen2 <options>* 

## Examples

    trgen

## Current version

0.18.0 Adding Xalan code. Fix #180. Fix crash in trgen https://github.com/antlr/grammars-v4/issues/2818. Fix #134. Add -e option to trrename. Update Antlr4BuildTasks version. Fix #197, #198. Fix trparse exit code. Add --quiet option to trparse to just get exit code. Change trgen templates to remove -file option, make file name parsing the default.
