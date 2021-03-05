#

alias make='mingw32-make.exe'
for i in CSharp Java JavaScript Dart Python3
do
	echo $i
	rm -rf Generated > /dev/null 2>&1
	dotnet-antlr -t $i > /dev/null 2>&1
	pushd Generated > /dev/null 2>&1
	ls > /dev/null 2>&1
	mingw32-make.exe > /dev/null 2>&1
	mingw32-make.exe run RUNARGS="-input 1+2 -tree"
	mingw32-make.exe clean > /dev/null 2>&1
	ls > /dev/null 2>&1
	popd > /dev/null 2>&1
done
