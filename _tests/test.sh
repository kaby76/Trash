#!/bin/bash

for i in `find . -name test.sh | grep -v '[.]/test.sh'`
do
	directory=${i%/*}
	echo $directory
	pushd $directory
	bash test.sh
	popd
done
