#!/bin/sh
set -e
for i in [a-z]*
do
	if [ ! -d $i ]
	then
		continue
	fi
	pushd $i > /dev/null 2>&1
	cd Generated-*
	pwd
	make
	popd > /dev/null 2>&1
done
