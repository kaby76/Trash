# set -x
# set -e

myrealpath ()
{
    f=$@;
    if [[ -d "$f" ]]; then
        base="";
        dir="$f";
    else
        base="/$(basename "$f")";
        dir=$(dirname "$f");
    fi;
    dir=$(cd "$dir" && /bin/pwd);
    echo "$dir$base"
}

cwd=$(myrealpath $0)
cwd=$(dirname $cwd)

run_target_java() {
    files=()
    rm -rf Generated-Java*
    dotnet trash gen -t Java > /dev/null 2>&1
    if [[ $? -ne 0 ]]
    then
        return 1
    fi
    local gendir
    gendir=`find . -maxdepth 1 -type d -name 'Generated-Java' -or -name 'Generated-Java-*' | head -1`
    if [[ ! -d $gendir ]]; then
        return 1
    fi
    cd "$gendir"
    bash build.sh > /dev/null 2>&1
    if [[ "$?" != "0" ]]; then
        return 1
    fi
    # Test generated parser on examples.
    local SAVEIFS=$IFS
    IFS=$(echo -en "\n\b")
    local cmd
    cmd=`grep '^files2' test.sh`
    eval "$cmd"
    for f in $files2
    do
        if [[ -d "$f" ]]; then continue; fi
        files+=( $f )
    done
    IFS=$SAVEIFS
    if [[ ${#files[@]} -eq 0 ]]; then
        return 1
    fi
#    echo "files = ${files[@]}" 1>&2
    rm -rf ../pt-java
    printf '%s\n' "${files[@]}" | bash run.sh -x -o ../pt-java -tee -tree 2> >(egrep '^(TT|PT):')
}

run_target_allstar() {
    dotnet trash parse -t ANTLRv4 *.g4 2> /dev/null | dotnet trash interp -o interp
    cd interp
    rm -rf ../pt-allstar
    printf '%s\n' "${files[@]}" | dotnet trash parse -L . --allstar -x 2> foo.txt | dotnet trash tree -a --bundle | dotnet trash sponge -o ../pt-allstar -c
    egrep '^(TT|PT):' foo.txt
}

#rm -rf `find $cwd -name 'Generated-*'`
#rm -rf `find $cwd -name 'pt-*'`

cwd=`pwd`

if [[ "$#" -gt 0 ]]
then
    while [[ $# -gt 0 ]]; do
        d="$1"
	shift
        if [[ ! -f "$d"/pom.xml ]]
        then
            continue
        fi
        if [[ ! -f "$d"/desc.xml ]]
        then
            continue
        fi
        haveany=false
        action=false
        for g in `echo "$d"/*.g4`
        do
            haveany=true
            if [[ `dotnet trash parse -t ANTLRv4 $g 2> /dev/null | dotnet trash xpath 'count(//actionBlock) > 0'` == "true" ]]
            then
                action="true"
            fi
        done
        if [[ "$haveany" == "false" ]]
        then
            continue
        fi
        if [[ "$action" == "true" ]]
        then
            continue
        fi
        g=`cygpath -u $d | sed "s%$cwd/%%"`

	// test $g
    done
else
    for desc in `find . -name desc.xml | grep -v Generated | grep -v examples`
    do
	possible_grammar=`dirname $desc`
	cd $cwd/$possible_grammar
        if [[ ! -f pom.xml ]]
        then
            continue
        fi
        haveany=false
        action=false
        for g in `echo *.g4`
        do
            haveany=true
            if [[ `dotnet trash parse -t ANTLRv4 $g 2> /dev/null | dotnet trash xpath 'count(//actionBlock) > 0'` == "true" ]]
            then
                action="true"
            fi
        done
        if [[ "$haveany" == "false" ]]
        then
            continue
        fi
        if [[ "$action" == "true" ]]
        then
            continue
        fi
        d=`dirname $desc`
        g=`cygpath -u $d | sed "s%$cwd/%%"`

	# Testing $g
        echo $g
    done
fi

