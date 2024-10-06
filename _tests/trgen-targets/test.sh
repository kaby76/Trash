#
trap 'LAST_COMMAND=$CURRENT_COMMAND; CURRENT_COMMAND=$BASH_COMMAND' DEBUG
trap 'ERROR_CODE=$?; FAILED_COMMAND=$LAST_COMMAND; tput setaf 1; echo "ERROR: command \"$FAILED_COMMAND\" failed with exit code $ERROR_CODE"; put sgr0;' ERR INT TERM
export MSYS2_ARG_CONV_EXCL="*"
where=`dirname -- "$0"`
cd "$where"
where=`pwd`
cd "$where"
echo "$where"

for target in Antlr4ng CSharp Cpp Dart Java JavaScript Python3 TypeScript Go
do
	dotnet trgen -t $target
	pushd Generated-$target
	make
	bash run.sh -input '1+2'
	if [ "$?" != "0" ]
	then
		echo Test failed.
		exit 1
	fi
	popd
done
echo Test succeeded.
exit 0
