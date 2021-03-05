#

alias make='mingw32-make.exe'
for i in CSharp Java JavaScript Dart Python3
do
	echo $i
	rm -rf Generated
	dotnet-antlr -t $i
	pushd Generated
	ls
	mingw32-make.exe
	mingw32-make.exe run RUNARGS="-input 1+2 -tree"
	mingw32-make.exe clean
	ls
	popd
done
