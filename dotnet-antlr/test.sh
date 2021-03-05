#
shopt -s expand_aliases
ls -l ~/.bash_profile
if [ -f ~/.bash_profile ]
then
	echo sourcing
	. ~/.bash_profile
fi

for i in CSharp Java JavaScript Dart Python3 Go
do
	echo $i
	rm -rf Generated
	dotnet-antlr -t $i
	pushd Generated
	ls
	make
	make run RUNARGS="-input 1+2 -tree"
	make clean
	ls
	popd
done
