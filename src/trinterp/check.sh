#
set -x

# Get full path of this script.
full_path_script=$(realpath $0)
full_path_script_dir=`dirname $full_path_script`
rm -rf a n i
mkdir a n i

for g in *.g4
do
	antlr4 -v 4.13.2 -encoding utf-8 -atn -Dlanguage=CSharp -o a $g > /dev/null 2>&1
	if [ $? -ne 0 ]
	then
		antlr4 -v 4.13.2 -encoding utf-8 -Dlanguage=CSharp -o a $g > /dev/null 2>&1
	fi
done
for g in *.g4
do
	antlr-ng --atn true -Dlanguage=None -o n $g > /dev/null 2>&1
done
dotnet trash parse *.g4 2> /dev/null | dotnet trash interp --atn -o i
