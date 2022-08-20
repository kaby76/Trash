#!/bin/bash

echo "Setting MSYS2_ARG_CONV_EXCL so that Trash XPaths do not get mutulated."
export MSYS2_ARG_CONV_EXCL="*"

trparse Expression.g4 | trreplace '//parserRuleSpec/RULE_REF[text()="a"]' 'atom' | trsponge -c -o Gold
