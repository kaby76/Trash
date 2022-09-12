#!/usr/bin/bash
# "Setting MSYS2_ARG_CONV_EXCL so that Trash XPaths do not get mutulated."
export MSYS2_ARG_CONV_EXCL="*"
where=`dirname -- "$0"`
cd "$where"
rm -rf Generated
mkdir Generated
for i in `find . -name pom.xml | grep -v _grammar-test | grep -v generated | grep -v target`
do
    base=`dirname $i`
    echo $i
    grep -qs . $base/*.g4
    if [ "$?" = "0" ]
    then
	for j in "$base"/*.g4
	do
		bash "$where/check.sh" "$j"
	done
    fi
done