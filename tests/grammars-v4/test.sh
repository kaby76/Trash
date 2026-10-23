#
set -x
rm -rf `find . -name 'Generated-*' -type d`
failures=()

record_failure() {
	local label=$1
	local status=$2
	printf 'TEST_FAILURE: grammars-v4/%s (exit %s)\n' "$label" "$status"
	failures+=("$label")
}

for desc in `find . -name desc.xml`
do
	dirname=`dirname $desc`
	pushd "$dirname" || exit 1
	for target in Antlr4ng Cpp CSharp Dart Go Java JavaScript TypeScript
	do
		if dotnet trash gen -t "$target"
		then
			:
		else
			record_failure "${dirname#./}/$target:gen" "$?"
			continue
		fi
		# If a target is not available, exit code should zero, but Generated directory won't exist.
		if [ ! -d "Generated-$target" ]
		then
			continue
		fi
		pushd "Generated-$target" || exit 1
		if make
		then
			if make test
			then
				if pwsh test.ps1
				then
					:
				else
					record_failure "${dirname#./}/$target:pwsh-test" "$?"
				fi
			else
				record_failure "${dirname#./}/$target:make-test" "$?"
			fi
		else
			record_failure "${dirname#./}/$target:make" "$?"
		fi
		popd
	done
	popd
done

if (( ${#failures[@]} != 0 ))
then
	printf 'Tests failed: grammars-v4/%s\n' "${failures[*]}"
	exit 1
fi
echo 'Grammar tests succeeded.'
