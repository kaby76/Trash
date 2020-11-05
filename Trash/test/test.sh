#!/bin/sh
exe=../bin/Debug/netcoreapp3.1/Trash.exe

echo ======== TEST 1 HELP ========

cat << HERE | $exe > test1.out
help
help alias
help analyze
help bang
help cd
help combine
help convert
help delabel
help delete
help dot
help find
help fold
help foldlit
help group
help has
help help
help history
help kleene
help ls
help mvsr
help parse
help pop
help print
help pwd
help quit
help read
help rename
help reorder
help rotate
help rr
help run
help rup
help set
help split
help st
help stack
help text
help ulliteral
help unalias
help unfold
help ungroup
help unulliteral
help version
help workspace
help write
quit
HERE
diff test1.out test1.std
if [ $? = 0 ]
then
	echo TEST 1 PASSED
else
	echo TEST 1 FAILED
fi

echo ======== TEST 2 READ, PARSE, PRINT ========
cat << HERE | $exe > test2.out
read Expr.g4
print
parse
.
quit
HERE
diff test2.out test2.std
if [ $? = 0 ]
then
	echo TEST 2 PASSED
else
	echo TEST 2 FAILED
fi

echo ======== TEST 3 DOT, FIND, TEXT, ST ========
cat << HERE | $exe > test3.out
read Expr.g4
parse
. | find "//parserRuleSpec[RULE_REF/text()='a']"
. | find "//parserRuleSpec[RULE_REF/text()='a']" | text
. | find "//parserRuleSpec[RULE_REF/text()='a']" | st
quit
HERE
diff test3.out test3.std
if [ $? = 0 ]
then
	echo TEST 3 PASSED
else
	echo TEST 3 FAILED
fi

echo ======== TEST 4 ULLITERAL, UNULLITERAL ========
cat << HERE | $exe > test4.out
read Literals.g4
parse
print
ulliteral
print
unulliteral lc
print
ulliteral
unulliteral uc
print
quit
HERE
diff test4.out test4.std
if [ $? = 0 ]
then
	echo TEST 4 PASSED
else
	echo TEST 4 FAILED
fi

