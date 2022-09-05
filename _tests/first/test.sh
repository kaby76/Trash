#!/bin/bash
# "Setting MSYS2_ARG_CONV_EXCL so that Trash XPaths do not get mutulated."
export MSYS2_ARG_CONV_EXCL="*"
where=`dirname -- "$0"`
cd "$where"
rm -rf Generated
mkdir Generated
for i in *.g4
do
	rm -rf grammar-temp
	mkdir grammar-temp
	cp $i grammar-temp
	cd grammar-temp
	trgen -s s
	cd Generated
	dotnet build
	cd ..
	trfirst >> ../Generated/output
	cd ..
	rm -rf grammar-temp
done
cd "$where"
diff -r Gold Generated
if [ "$?" != "0" ]
then
	echo Test failed.
	exit 1
else
	echo Test succeeded.
fi
