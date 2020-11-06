#!/bin/sh

exe="/c/Users/kenne/Documents/AntlrVSIX/Trash/bin/Debug/netcoreapp3.1/Trash.exe -noprompt -echo"
cd /c/Users/kenne/Documents/grammars-v4/
files=`find . -name '*.g4'`
for i in $files
do
cat << HERE | $exe
read $i
parse
. | xgrep "//ebnf[blockSuffix/ebnfSuffix/QUESTION and block/altList[@ChildCount = 1]/alternative[@ChildCount = 1]/element/ebnf/blockSuffix/ebnfSuffix/PLUS]" | text line-number
HERE
done

