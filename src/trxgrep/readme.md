# trxgrep

## Summary

"Grep" for nodes in a parse tree using XPath

## Description

Find all sub-trees in a parse tree using the given XPath expression.

## Usage

    trxgrep <string>

## Examples

    trparse A.g4 | trxgrep "//parserRuleSpec[RULE_REF/text() = 'normalAnnotation']"

## Notes

If you are running MSYS2 on Windows, you may notice that XPaths are not being
processed by this command correctly. To avoid the Bash shell from altering
XPaths, type _export MSYS2_ARG_CONV_EXCL="*"_, then execute your command.

## Current version

0.18.0 Adding Xalan code. Fix #180. Fix crash in trgen https://github.com/antlr/grammars-v4/issues/2818. Fix #134. Add -e option to trrename. Update Antlr4BuildTasks version. Fix #197, #198. Fix trparse exit code. Add --quiet option to trparse to just get exit code. Change trgen templates to remove -file option, make file name parsing the default.
