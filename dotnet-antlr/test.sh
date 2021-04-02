    #
    shopt -s expand_aliases
    ls -l ~/.bash_profile
    if [ -f ~/.bash_profile ]
    then
        echo sourcing
        . ~/.bash_profile
    fi

    for i in CSharp Java JavaScript Dart Python3 Go
    do
        echo $i
        rm -rf Generated
        dotnet-antlr -t $i -m
        if [[ "$?" != "0" ]]
        then
            rm -rf Generated
            continue
        fi
        pushd Generated
        ls
        make
        if [[ "$?" != "0" ]]
        then
            rm -rf Generated
            continue
        fi
        if [[ -d ../examples ]]
        then
            for file in `find ../examples -type f`
            do
    # NOTE: Python3 has a tendency to just hang with compile errors.
    # So, we avoid reading from stdin for Python3, but may as well do for
    # others.
    #           cat $file | make run
                make run RUNARGS="-file $file"
            done
        else
            make run RUNARGS="-input 1+2 -tree"
        fi
        popd
        rm -rf Generated
    done
