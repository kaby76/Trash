# Generated from trgen <version>

SAVEIFS=$IFS
IFS=$(echo -en "\n\b")

rm -f parse.txt

# Output basic information on machine.
unameOut="$(uname -s)"
case "${unameOut}" in
    Linux*)     machine=Linux;;
    Darwin*)    machine=Mac;;
    CYGWIN*)    machine=Cygwin;;
    MINGW*)     machine=MinGw;;
    *)          machine="UNKNOWN:${unameOut}"
esac

echo OS:
if [[ "$machine" == "Linux" ]]
then
    lsb_release -a >> parse.txt
fi
if [[ "$machine" == "CYGWIN" || "$machine" == "MinGw" ]]
then
    systeminfo | grep -E '^OS' >> parse.txt
fi

echo CPU:
if [[ "$machine" == "Linux" ]]
then
    lscpu | grep -e 'Model name' >> parse.txt
fi
if [[ "$machine" == "CYGWIN" || "$machine" == "MinGw" ]]
then
    pwsh -c 'Get-WmiObject -Class Win32_Processor | Select-Object -Property Name' >> parse.txt
fi

echo Memory:
if [[ "$machine" == "Linux" ]]
then
    free -h >> parse.txt
fi
if [[ "$machine" == "CYGWIN" || "$machine" == "MinGw" ]]
then
    pwsh -c 'Get-WmiObject -Class Win32_PhysicalMemory | Format-Table Capacity' >> parse.txt
fi

# Get a list of test files from the test directory. Do not include any
# .errors or .tree files. Pay close attention to remove only file names
# that end with the suffix .errors or .tree.
files2=`dotnet trglob '../<example_files_unix>' | grep -v '.errors$' | grep -v '.tree$'`
files=()
for f in $files2
do
    if [ -d "$f" ]; then continue; fi
    dotnet triconv -f utf-8 $f > /dev/null 2>&1
    if [ "$?" = "0" ]
    then
        files+=( $f )
    fi
done

# People often specify a test file directory, but sometimes no
# tests are provided. Git won't check in an empty directory.
# Test if there are no test files.
if [ ${#files[@]} -eq 0 ]
then
    echo "No test cases provided."
    exit 0
fi

n=10

# Parse all input files.
# Individual parsing.
for f in ${files[*]}
do
    # Loop from 1 to n and execute the body of the loop each time
    for ((i=1; i\<=n; i++))
    do
        dotnet trwdog python3 Test.py -prefix individual $f >> parse.txt 2>&1
        xxx="$?"
        if [ "$xxx" -ne 0 ]
        then
            status="$xxx"
        fi
    done
done

# Group parsing.
# Loop from 1 to n and execute the body of the loop each time
for ((i=1; i\<=n; i++))
do
    echo "${files[*]}" | dotnet trwdog python3 Test.py -x -prefix group >> parse.txt 2>&1
    xxx="$?"
    if [ "$xxx" -ne 0 ]
    then
        status="$xxx"
    fi
done

exit 0
