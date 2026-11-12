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
echo cwd $cwd

run_target() {
    files=()
    f2=`trash xml2 desc.xml | grep inputs | head -1 | awk -F = '{print $2}'`
    echo f2 = "'""$f2""'"
    if [[ "$f2" == "" ]]
    then
        f2=examples/
    fi
    files2=`trash glob "$f2" | tr -d '\r' | grep -v '[.]errors$' | grep -v '[.]tree$' | grep -v '[.]trq$'`
    echo "files2 = $files2"
    # Test generated parser on examples.
    local SAVEIFS=$IFS
    IFS=$(echo -en "\n\b")
    for f in $files2
    do
        if [[ -d "$f" ]]; then continue; fi
        files+=( $f )
    done
    IFS=$SAVEIFS
    if [[ ${#files[@]} -eq 0 ]]; then
        return 1
    fi
    echo "files = ${files[@]}" 1>&2
    start_rule=$(dotnet trash xml2 desc.xml | awk -F= '/\/entry-point=/{print $2; exit}')

    grammar_spec=$(trash xml2 desc.xml | awk -F= '/\/grammar-files=/{print $2; exit}')
    if [[ -n "$grammar_spec" ]]; then
        grammar_list=$(trash glob "$grammar_spec" | tr -d '\r')
        if [[ -z "$grammar_list" ]]; then
            echo "No grammar files matched <grammar-files>: $grammar_spec" >&2
            return 1
        fi
        mapfile -t grammar_files <<< "$grammar_list"
    else
        grammar_name=$(trash xml2 desc.xml | awk -F= '/\/grammar-name=/{print $2; exit}')
        if [[ -n "$grammar_name" ]]; then
            grammar_files=()
            if [[ -f "$grammar_name.g4" ]]; then
                # A named combined grammar is sufficient on its own. For a
                # separate parser grammar, include its companion lexer first.
                if grep -Eq '^[[:space:]]*parser[[:space:]]+grammar[[:space:]]' "$grammar_name.g4"; then
                    lexer_file="${grammar_name%Parser}Lexer.g4"
                    if [[ -f "$lexer_file" ]]; then grammar_files+=("$lexer_file"); fi
                fi
                grammar_files+=("$grammar_name.g4")
            else
                if [[ -f "${grammar_name}Lexer.g4" ]]; then grammar_files+=("${grammar_name}Lexer.g4"); fi
                if [[ -f "${grammar_name}Parser.g4" ]]; then grammar_files+=("${grammar_name}Parser.g4"); fi
            fi
            if [[ ${#grammar_files[@]} -eq 0 ]]; then
                echo "No grammar files matched <grammar-name>: $grammar_name" >&2
                return 1
            fi
        else
            grammar_files=(./*.g4)
        fi
    fi
    for grammar_file in "${grammar_files[@]}"; do
        if [[ ! -f "$grammar_file" ]]; then
            echo "Grammar file not found: $grammar_file" >&2
            return 1
        fi
    done
    local interp_dir parse_status
    interp_dir=$(mktemp -d /tmp/trash-action-free.XXXXXXXX) || return 1
    interp_args=(-o "$interp_dir")
    if [[ -n "$start_rule" ]]; then
        interp_args+=(--start-rule "$start_rule")
    fi
    if ! trash parse "${grammar_files[@]}" | trash interp "${interp_args[@]}"; then
        rm -f -- "$interp_dir"/*
        rmdir -- "$interp_dir"
        return 1
    fi
    printf '%s\n' "${files[@]}" | trash wdog -t 200 trash parse -L "$interp_dir" -x --no-output --per-file
    parse_status=$?
    rm -f -- "$interp_dir"/*
    rmdir -- "$interp_dir"
    return "$parse_status"
}

root=`pwd`

# Read each grammar as soon as the finder prints it. Command substitution in
# a for-loop waits for the finder to finish before starting this loop.
while IFS= read -r g || [[ -n "$g" ]]
do
    desc=$g/desc.xml
    echo "$g"
    cd "$root/$g"; run_target
done < <(bash "$cwd/find-action-free-grammars.sh")
