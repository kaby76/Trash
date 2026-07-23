#
# set -x

# Get full path of this script.
full_path_script=$(realpath $0)
full_path_script_dir=`dirname $full_path_script`
rm -rf a i
mkdir a i
antlr4 -v 4.13.2 -atn -Dlanguage=CSharp -o a *.g4
dotnet trash parse *.g4 | dotnet $full_path_script_dir/../bin/Release/net10.0/trinterp.dll --atn -o i

pushd a
for d in `ls *.dot`
do
	python /c/Users/Kenne/Documents/GitHub/Trash/sort-dot.py $d > xxx
	mv xxx $d
done
popd

pushd i
for d in `ls *.dot`
do
	python /c/Users/Kenne/Documents/GitHub/Trash/sort-dot.py $d > xxx
	mv xxx $d
done
popd

diff -r a i
