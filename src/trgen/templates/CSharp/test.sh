# Template generated code from trgen <version>
err=0
SAVEIFS=$IFS
IFS=$(echo -en "\n\b")
files=`find ../<example_files_unix> -type f | grep -v '.errors$' | grep -v '.tree$'`
echo "$files" | trwdog ./bin/Debug/net6.0/<exec_name> -x -shunt -tree
status=$?
rm -rf `find ../<example_files_unix> -type f -size 0 -name '*.errors' -o -name '*.tree'`
unameOut="$(uname -s)"
case "${unameOut}" in
    Linux*)     machine=Linux;;
    Darwin*)    machine=Mac;;
    CYGWIN*)    machine=Cygwin;;
    MINGW*)     machine=MinGw;;
    *)          machine="UNKNOWN:${unameOut}"
esac
if [[ "$machine" == "MinGw" || "$machine" == "Msys" || "$machine" == "Cygwin" || "#machine" == "Linux" ]]
then
  dos2unix `find ../<example_files_unix> -type f -name '*.errors' -o -name '*.tree'`
fi
cd ../<example_files_unix>
git diff --exit-code --name-only . > temp-output.txt 2>&1
diffs=$?
if [ "$diffs" = "129" ]
then
  err=$status
elif [ "$diffs" = "1" ]
then
  cat temp-output.txt
  echo Output difference--failed test.
  err=1
else
  err=$status
fi
rm -f temp-output.txt
exit $err
