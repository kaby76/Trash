#
set -e
set -x
rm -rf `find . -name 'Generated-*' -type d`
for desc in `find . -name desc.xml`
do
	dirname=`dirname $desc`
	pushd $dirname
	for target in Antlr4ng Cpp CSharp Dart Go Java JavaScript Python3 TypeScript
	do
		dotnet trgen -- -t $target
		pushd Generated-$target*
		make
		make test
		pwsh test.ps1
		popd
	done
	popd
done
