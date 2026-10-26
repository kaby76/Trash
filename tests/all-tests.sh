#!/bin/bash

#set +e
set -x
export TERM=xterm-mono
export MSYS2_ARG_CONV_EXCL="*"
where=`dirname -- "$0"`
tests=`find "$where" -name test.sh | grep -v Generated\* | sort`
tests="$tests $where/../examples/test.sh"
echo Tests in Trash: $tests
failed=()
for i in $tests
do
	dir=`dirname "$i"`
	fn=`basename "$i"`
	pushd "$dir"
	log=$(mktemp)
	bash "$fn" 2>&1 | tee "$log"
	result=${PIPESTATUS[0]}
	if [ "$result" != 0 ]
	then
		echo "Failed $i"
		nested=0
		while IFS= read -r detail
		do
			failed+=( "$detail" )
			nested=1
		done < <(sed -n 's/^TEST_FAILURE: //p' "$log")
		if [ "$nested" = 0 ]
		then
			failed+=( "$i" )
		fi
	fi
	rm -f -- "$log"
	popd
done
if (( ${#failed[@]} != 0 ))
then
	printf 'Tests failed: %s\n' "${failed[*]}"
	exit 1
else
	echo Tests succeeded.
	exit 0
fi
