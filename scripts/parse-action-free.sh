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
    grammar_spec=$(dotnet trash xml2 desc.xml | awk -F= '/\/grammar-files=/{print $2; exit}')
    if [[ -n "$grammar_spec" ]]; then
        grammar_list=$(dotnet trash glob "$grammar_spec" | tr -d '\r')
        if [[ -z "$grammar_list" ]]; then
            echo "No grammar files matched <grammar-files>: $grammar_spec" >&2
            return 1
        fi
        mapfile -t grammar_files <<< "$grammar_list"
    else
        grammar_name=$(dotnet trash xml2 desc.xml | awk -F= '/\/grammar-name=/{print $2; exit}')
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
    if ! dotnet trash parse "${grammar_files[@]}" | dotnet trash interp -o "$interp_dir"; then
        rm -f -- "$interp_dir"/*
        rmdir -- "$interp_dir"
        return 1
    fi
    local -a test_specs parse_args
    local spec start_rule pattern
    mapfile -t test_specs < <(dotnet trash xml2 desc.xml | tr -d '\r' | awk -F= '
        $1 == "/desc/test/name" {
            if (seen) print rule "|" input
            seen = 1; rule = ""; input = ""
        }
        $1 == "/desc/test/entry-point" { rule = $2 }
        $1 == "/desc/test/inputs" { input = $2 }
        END { if (seen) print rule "|" input }
    ')
    if [[ ${#test_specs[@]} -eq 0 ]]; then
        f2=$(dotnet trash xml2 desc.xml | awk -F= '$1 == "/desc/inputs" {print $2; exit}')
        if [[ -z "$f2" ]]; then f2=examples/; fi
        test_specs+=("|$f2")
    fi
    parse_status=0
    for spec in "${test_specs[@]}"; do
        start_rule=${spec%%|*}
        pattern=${spec#*|}
        if [[ -z "$pattern" ]]; then pattern=examples/; fi
        files=()
        while IFS= read -r f; do
            [[ -f "$f" ]] && files+=("$f")
        done < <(dotnet trash glob "$pattern" | tr -d '\r' |
            grep -v '[.]errors$' | grep -v '[.]tree$' | grep -v '[.]trq$')
        if [[ ${#files[@]} -eq 0 ]]; then
            echo "No inputs matched $pattern" >&2
            parse_status=1
            continue
        fi
        echo "files = ${files[*]} (start rule: ${start_rule:-default})" >&2
        parse_args=(-L "$interp_dir" -x --no-output --per-file)
        if [[ -n "$start_rule" ]]; then parse_args+=(--start-rule "$start_rule"); fi
        printf '%s\n' "${files[@]}" |
            dotnet trash wdog -t 200 dotnet trash parse "${parse_args[@]}" ||
            parse_status=1
    done
    rm -f -- "$interp_dir"/*
    rmdir -- "$interp_dir"
    return "$parse_status"
}

root=`pwd`
failed=0

# Read each grammar as soon as the finder prints it. Command substitution in
# a for-loop waits for the finder to finish before starting this loop.
while IFS= read -r g || [[ -n "$g" ]]
do
    desc=$g/desc.xml
    echo "$g"
    if ! (cd "$root/$g" && run_target); then
        failed=1
    fi
done < <(bash "$cwd/find-action-free-grammars.sh")

exit "$failed"
