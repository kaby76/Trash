#!/usr/bin/bash
set -e
#set -x
for i in [a-z]*
do
	if [ ! -d $i ]
	then
		continue
	fi
	pushd $i > /dev/null 2>&1
	cd Generated-*
	dos2unix Test.cs
	sed -i -e "s/SetupParse2(string input, bool quiet = false)/SetupParse2(string input, string fn, bool quiet = false)/g" Test.cs
	dos2unix Test.cs
	awk '
/TokenStream = tokens;/ && !done {
    print
    print "        ((AntlrInputStream)(lexer.InputStream)).name = fn;"
    done = 1
    next
}
{ print }
' Test.cs > tmp && mv tmp Test.cs
	unix2dos Test.cs
	popd > /dev/null 2>&1
done
