#!/bin/sh
exe="../bin/Debug/net5-windows/Trash.exe -noprompt -echo"

echo ======== TEST 1 HELP ========

cat << HERE | $exe > test1.out
help
help agl
help alias
help analyze
help bang
help cat
help cd
help combine
help convert
help delabel
help delete
help dot
help echo
help fold
help foldlit
help generate
help group
help has
help help
help history
help json
help kleene
help ls
help mvsr
help parse
help period
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
help xgrep
help xml
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
. | tree
quit
HERE
diff test2.out test2.std
if [ $? = 0 ]
then
	echo TEST 2 PASSED
else
	echo TEST 2 FAILED
fi

echo ======== TEST 3 PERIOD, XGREP, TEXT, ST ========
cat << HERE | $exe > test3.out
read Expr.g4
parse
. | xgrep "//parserRuleSpec[RULE_REF/text()='a']" | tree
. | xgrep "//parserRuleSpec[RULE_REF/text()='a']" | text
. | xgrep "//parserRuleSpec[RULE_REF/text()='a']" | st
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

echo ======== TEST 5 FOLDLIT ========
cat << HERE | $exe > test5.out
read Fold.g4
parse
print
foldlit "//lexerRuleSpec/TOKEN_REF[text()='MUL']"
print
foldlit "//lexerRuleSpec/TOKEN_REF"
print
quit
HERE
diff test5.out test5.std
if [ $? = 0 ]
then
	echo TEST 5 PASSED
else
	echo TEST 5 FAILED
fi


echo ======== TEST 6 UNFOLD ========
cat << HERE | $exe > test6.out
read Fold.g4
parse
print
unfold "//parserRuleSpec//labeledAlt//RULE_REF[text()='a']"
print
quit
HERE
diff test6.out test6.std
if [ $? = 0 ]
then
	echo TEST 6 PASSED
else
	echo TEST 6 FAILED
fi


echo ======== TEST 7 DOT, XML, JSON ========
cat << HERE | $exe > test7.out
read Expr.g4
parse
. | dot
. | xml
. | json
quit
HERE
diff test7.out test7.std
if [ $? = 0 ]
then
	echo TEST 7 PASSED
else
	echo TEST 7 FAILED
fi


echo ======== TEST 8 GENERATE, DOT ========
cat << HERE | $exe | grep -v 'Restored' | grep -v 'Time Elapsed' > test8.out
read Expr.g4
parse
generate e
echo "1+2*3+4/5*6/7-8/9++10/-11" | run | dot
quit
HERE
diff test8.out test8.std
if [ $? = 0 ]
then
	echo TEST 8 PASSED
else
	echo TEST 8 FAILED
fi


