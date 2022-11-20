#!/bin/bash
#!/bin/bash

# BASH error handling:
#   exit on command failure
set -e
#   keep track of the last executed command
trap 'LAST_COMMAND=$CURRENT_COMMAND; CURRENT_COMMAND=$BASH_COMMAND' DEBUG
#   on error: print the failed command
trap 'ERROR_CODE=$?; FAILED_COMMAND=$LAST_COMMAND; tput setaf 1; echo "ERROR: command \"$FAILED_COMMAND\" failed with exit code $ERROR_CODE"; put sgr0;' ERR INT TERM
# Setting MSYS2_ARG_CONV_EXCL so that Trash XPaths do not get mutulated.
export MSYS2_ARG_CONV_EXCL="*"

# Start in the directory containing test script.
where=`dirname -- "$0"`
cd "$where"
where=`pwd`
cd "$where"
echo "$where"

# Test.
rm -rf Generated
mkdir Generated
trparse -e -v Expression.g4 > Generated/trparse.tree 2>&1
trparse Expression.g4 | trdelete -v '//xxxx' > Generated/trdelete.tree
# remove tail from trdelete.tree.
wc=`cat Generated/trparse.tree | wc -l`
echo $wc
cat Generated/trdelete.tree | head -"$wc" > Generated/new.tree

for j in eof no-eof
do
	cd $j
	rm -rf Generated-CSharp
	trgen -s s
	cd Generated-CSharp
	make
	echo "1 + 2 + 3" | trparse -e -v > trparse.tree 2>&1
	cd ..
	cd ..
done

# Diff result.
for i in Generated/*
do
	dos2unix $i
done
for i in Gold/*
do
	dos2unix $i
done
diff Generated Gold
if [ "$?" != "0" ]
then
	echo Test failed.
	exit 1
else
	echo Test succeeded.
fi
exit 0
