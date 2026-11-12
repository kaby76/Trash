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
    f2=`dotnet trash xml2 desc.xml | grep inputs | head -1 | awk -F = '{print $2}'`
    echo f2 = "'""$f2""'"
    if [[ "$f2" == "" ]]
    then
        f2=examples/
    fi
    files2=`trash glob "$f2" | tr -d '\r' | grep -v '[.]errors$' | grep -v '[.]tree$' | grep -v '[.]trq$'`
    echo files2 = $files
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
    interp_args=(-o interp)
    if [[ -n "$start_rule" ]]; then
        interp_args+=(--start-rule "$start_rule")
    fi
    trash parse *.g4 | trash interp "${interp_args[@]}"
    printf '%s\n' "${files[@]}" | trash parse -L interp -x --no-output --per-file
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
