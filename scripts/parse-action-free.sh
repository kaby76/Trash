# set -x
set -o pipefail

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
    # <grammar-files> lists the entry grammars, not necessarily their delegates.
    # Supply every transitive import to trinterp so it can resolve the graph
    # and select the top-level grammar using desc.xml.
    local -A supplied=()
    local grammar_file import_name import_file
    for grammar_file in "${grammar_files[@]}"; do
        supplied["$(cd "$(dirname "$grammar_file")" && pwd)/$(basename "$grammar_file")"]=1
    done
    for ((i=0; i<${#grammar_files[@]}; i++)); do
        grammar_file=${grammar_files[i]}
        while IFS= read -r import_name; do
            import_file="$(dirname "$grammar_file")/$import_name.g4"
            if [[ ! -f "$import_file" ]]; then
                echo "Imported grammar $import_name.g4 required by $grammar_file was not found" >&2
                return 1
            fi
            import_file="$(cd "$(dirname "$import_file")" && pwd)/$(basename "$import_file")"
            if [[ ! -v supplied["$import_file"] ]]; then
                grammar_files+=("$import_file")
                supplied["$import_file"]=1
            fi
        done < <(awk '
            /^[[:space:]]*import[[:space:]]/ {
                line = $0
                while (index(line, ";") == 0 && getline) line = line " " $0
                sub(/^[[:space:]]*import[[:space:]]+/, "", line)
                sub(/;.*/, "", line)
                count = split(line, names, ",")
                for (j = 1; j <= count; j++) {
                    sub(/^.*=/, "", names[j])
                    gsub(/[[:space:]]/, "", names[j])
                    if (names[j] != "") print names[j]
                }
            }
        ' "$grammar_file")
    done
    local interp_dir parse_status
    local -a test_specs parse_args interp_args
    local spec grammar_name start_rule pattern spec_output
    spec_output=$(python "$cwd/descriptor-tests.py" desc.xml) || return 1
    [[ -n "$spec_output" ]] || return 1
    mapfile -t test_specs <<< "$spec_output"
    parse_status=0
    for spec in "${test_specs[@]}"; do
        grammar_name=${spec%%|*}
        spec=${spec#*|}
        start_rule=${spec%%|*}
        pattern=${spec#*|}
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
        interp_dir=$(mktemp -d /tmp/trash-action-free.XXXXXXXX) || return 1
        interp_args=(-o "$interp_dir")
        if [[ -n "$grammar_name" ]]; then interp_args+=(--grammar-name "$grammar_name"); fi
        if [[ -n "$start_rule" ]]; then interp_args+=(--start-rule "$start_rule"); fi
        if ! dotnet trash parse "${grammar_files[@]}" | dotnet trash interp "${interp_args[@]}"; then
            rm -f -- "$interp_dir"/*
            rmdir -- "$interp_dir"
            parse_status=1
            continue
        fi
        echo "files = ${files[*]} (start rule: ${start_rule:-default})" >&2
        parse_args=(-L "$interp_dir" -x --no-output --per-file)
        if [[ -n "$start_rule" ]]; then parse_args+=(--start-rule "$start_rule"); fi
        printf '%s\n' "${files[@]}" |
            dotnet trash wdog -t 200 dotnet trash parse "${parse_args[@]}" ||
            parse_status=1
        rm -f -- "$interp_dir"/*
        rmdir -- "$interp_dir"
    done
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
