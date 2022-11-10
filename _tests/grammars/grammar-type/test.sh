#!/bin/bash

# "Setting MSYS2_ARG_CONV_EXCL so that Trash XPaths do not get mutulated."
export MSYS2_ARG_CONV_EXCL="*"
where=`dirname -- "$0"`
cd "$where"
rm -rf Generated
mkdir Generated
for i in `ls examples/* | sort -u`
do
	bash get-types.sh $i >> Generated/output
done
dos2unix Gold/output > /dev/null 2>&1
dos2unix Generated/output > /dev/null 2>&1
diff -r Gold Generated
if [ "$?" != "0" ]
then
	echo Test failed.
	exit 1
else
	echo Test succeeded.
fi
