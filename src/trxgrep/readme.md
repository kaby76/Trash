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

0.19.3 Complete rewrite of parse tree representation. NB: not all Trash tools supported yet.
