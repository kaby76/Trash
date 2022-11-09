#!/bin/bash

# BASH error handling:
#   exit on command failure
set -e
#   keep track of the last executed command
trap 'LAST_COMMAND=$CURRENT_COMMAND; CURRENT_COMMAND=$BASH_COMMAND' DEBUG
#   on error: print the failed command
trap 'ERROR_CODE=$?; FAILED_COMMAND=$LAST_COMMAND; tput setaf 1; echo "ERROR: command \"$FAILED_COMMAND\" failed with exit code $ERROR_CODE"; put sgr0;' ERR INT TERM

# "Setting MSYS2_ARG_CONV_EXCL so that Trash XPaths do not get mutulated."
export MSYS2_ARG_CONV_EXCL="*"
where=`dirname -- "$0"`
cd "$where"

#for i in Antlr4cs Cpp CSharp Dart Go Java JavaScript PHP Python3
for i in Antlr4cs CSharp Dart Go Java
do
	pushd $i
	cp ../simple.g4 .
	rm -rf Generated/
	trgen -s hello -t $i
	cd Generated; make
	make run RUNARGS='-input "hello world" -tree' > ../output
	dos2unix ../output
	cd ..
	diff output Gold/
	rm -rf Generated/
	popd
done