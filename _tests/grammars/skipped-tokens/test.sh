#!/usr/bin/bash
# "Setting MSYS2_ARG_CONV_EXCL so that Trash XPaths do not get mutulated."
export MSYS2_ARG_CONV_EXCL="*"
where=`dirname -- "$0"`
cd "$where"
where=`pwd`
rm -rf Generated
mkdir Generated
cd Generated
git init grammars-v4
cd grammars-v4
git remote add -f origin https://github.com/antlr/grammars-v4.git
git config core.sparseCheckout true
echo "abb/" >> .git/info/sparse-checkout
echo "abnf/" >> .git/info/sparse-checkout
echo "/asm" >> .git/info/sparse-checkout
git pull origin 0d8be4573d284a89444a46beeface54f48853c8a
for i in `find . -name pom.xml`
do
    base=`dirname $i`
    echo $i
    grep -qs . $base/*.g4
    if [ "$?" = "0" ]
    then
	for j in "$base"/*.g4
	do
		bash "$where/check.sh" "$j" >> "$where/Generated/output" 2>&1
	done
    fi
done
dos2unix "$where/Generated/output"
dos2unix "$where/Gold/output"
diff -r "$where/Gold/output" "$where/Generated/output"
if [ "$?" != "0" ]
then
	echo Test failed.
	echo "generated:"
	cat "$where/Generated/output"
	echo "gold:"
	cat "$where/Gold/output"
	exit 1
else
	echo Test succeeded.
fi
