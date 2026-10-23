#!/bin/bash

export MSYS2_ARG_CONV_EXCL="*"
where=`dirname -- "$0"`
cd "$where"
where=`pwd`
echo "$where"
failures=()

record_failure() {
	local label=$1
	local status=$2
	printf 'TEST_FAILURE: trgen-targets/%s (exit %s)\n' "$label" "$status"
	failures+=("$label")
}

for target in Antlr4ng CSharp Cpp Dart Java JavaScript Python3 TypeScript Go
do
	if dotnet trash gen -t "$target"
	then
		:
	else
		record_failure "$target:gen" "$?"
		continue
	fi
	if [ ! -d "Generated-$target" ]
	then
		record_failure "$target:gen-no-output" 1
		continue
	fi
	pushd "Generated-$target" || exit 1
	if make
	then
		if bash run.sh -input '1+2'
		then
			:
		else
			record_failure "$target:run" "$?"
		fi
	else
		record_failure "$target:make" "$?"
	fi
	popd
done
if (( ${#failures[@]} != 0 ))
then
	printf 'Tests failed: trgen-targets/%s\n' "${failures[*]}"
	exit 1
fi
echo Test succeeded.
exit 0
