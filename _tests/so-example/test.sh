#
set -e
set -x
trap 'LAST_COMMAND=$CURRENT_COMMAND; CURRENT_COMMAND=$BASH_COMMAND' DEBUG
trap 'ERROR_CODE=$?; FAILED_COMMAND=$LAST_COMMAND; echo "ERROR: command \"$FAILED_COMMAND\" failed with exit code $ERROR_CODE";' ERR INT TERM
export MSYS2_ARG_CONV_EXCL="*"
where=`dirname -- "$0"`
cd "$where"
where=`pwd`
cd "$where"
echo "$where"
rm -rf `find . -name 'Generated-*' -type d`
dotnet trgen -t CSharp
pushd Generated-CSharp
make
