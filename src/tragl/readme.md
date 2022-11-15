# Tragl

## Summary

Display a parse tree using Microsoft Automatic Graph Layout

## Description

Read a parse tree from stdin and open a Windows Form that displays the tree.
This tool is part of Trash, Transformations for Antlr Shell.

## Usage

    tragl

## Example

    trparse -i "1+2" | tragl

## Current version

0.18.2 Fix trperf, trfirst. Adding Xalan code. Fix #180. Fix crash in trgen https://github.com/antlr/grammars-v4/issues/2818. Fix #134. Add -e option to trrename. Update Antlr4BuildTasks version. Fix #197, #198. Fix trparse exit code. Add --quiet option to trparse to just get exit code. Change trgen templates to remove -file option, make file name parsing the default.
