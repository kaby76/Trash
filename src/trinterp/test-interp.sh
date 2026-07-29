#

# Get full path of this script.
full_path_script=$(realpath $0)
full_path_script_dir=`dirname $full_path_script`

grammars=()
while IFS= read -r desc_file; do
    g=$(dirname "$desc_file")
    g="${g#./}"
    grammars+=("$g")
done < <(find . -name desc.xml -not -path '*/.git/*' | grep -v Generated | sort -u)

for grammar in "${grammars[@]}"; do
    pushd $grammar

    rm -rf a i n

    # Generate ATN .dot files
    for g in *.g4
    do
        antlr4 -v 4.13.2 -encoding utf-8 -atn -Dlanguage=CSharp -o a $g > /dev/null 2>&1
        if [ $? -ne 0 ]
        then
            antlr4 -v 4.13.2 -encoding utf-8 -Dlanguage=CSharp -o a $g > /dev/null 2>&1
        fi
    done
#    for g in *.g4
#    do
#        antlr-ng --atn true -Dlanguage=None -o n $g > /dev/null 2>&1
#    done
    dotnet trash parse *.g4 2> /dev/null | dotnet trash interp --atn -o i

#    echo diff between Antlr4 and Antlr-ng
#    python $full_path_script_dir/compare-atn.py a n

    echo diff between Antlr4 and Trinterp
    python $full_path_script_dir/compare-atn.py a i

#    echo diff between Antlr-ng and Trinterp
#    python $full_path_script_dir/compare-atn.py n i

    rm -rf a i n
    popd
done
