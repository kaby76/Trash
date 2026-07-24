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
    antlr4 -v 4.13.2 -atn -Dlanguage=CSharp -o a *.g4
    antlr-ng --atn true -Dlanguage=None -o n *.g4
    dotnet trash parse *.g4 | dotnet trash interp --atn -o i

    echo diff between Antlr4 and Antlr-ng
    python $full_path_script_dir/compare-atn.py a n

    echo diff between Antlr4 and Trinterp
    python $full_path_script_dir/compare-atn.py a i

    rm -rf a i n
    popd
done
