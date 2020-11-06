#!/bin/sh

exe="/c/Users/kenne/Documents/AntlrVSIX/Trash/bin/Debug/netcoreapp3.1/Trash.exe -noprompt -echo"
cd /c/Users/kenne/Documents/grammars-v4/
files=`find . -name '*.g4'`
for i in $files
do
cat << HERE | $exe
read $i
parse
HERE
done

