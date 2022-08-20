#!/bin/bash

export MSYS2_ARG_CONV_EXCL="*"
where=`dirname -- "$0"`
tests=`find "$where" -name test.sh | grep -v Generated`
echo $tests
for i in $tests
do
	bash $i
	if [ "$?" != "0" ]
	then
		exit 1
	fi
done
