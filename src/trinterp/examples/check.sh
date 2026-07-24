#
# set -x

# Get full path of this script.
full_path_script=$(realpath $0)
full_path_script_dir=`dirname $full_path_script`
rm -rf a n i
mkdir a n i
antlr4 -v 4.13.2 -atn -Dlanguage=CSharp -o a *.g4
antlr-ng --atn true -o n -Dlanguage=None *.g4
dotnet trash parse *.g4 | dotnet $full_path_script_dir/../bin/Release/net10.0/trinterp.dll --atn -o i
