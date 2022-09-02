#!/bin/bash

set +e
set -x
export TERM=xterm-mono
export MSYS2_ARG_CONV_EXCL="*"
where=`dirname -- "$0"`
tests=`find "$where" -name test.sh | grep -v Generated`
echo $tests
for i in $tests
do
	bash $i
done
