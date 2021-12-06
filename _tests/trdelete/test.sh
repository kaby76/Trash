#!/bin/bash

echo "Setting MSYS2_ARG_CONV_EXCL so that Trash XPaths do not get mutulated."
export MSYS2_ARG_CONV_EXCL="*"

trparse Expression.g4 | trdelete '//parserRuleSpec[RULE_REF/text()="a"]' | trsponge -c -o Gold
