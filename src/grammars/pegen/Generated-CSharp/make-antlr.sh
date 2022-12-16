#
# "Setting MSYS2_ARG_CONV_EXCL so that Trash XPaths do not get mutulated."
export MSYS2_ARG_CONV_EXCL="*"
date
trparse -d pegen ../examples/python-11.gram > orig.pt
cat orig.pt \
	| trxgrep '//rule_' \
	| trdelete '//action' \
	| trreplace '//attribute' ' ' \
	| trreplace '//attribute_name' ' ' \
	| trinsert '//lookahead' ' /* (lookahead) ' \
	| trinsert -a '//lookahead' ' */ ' \
	| trreplace '//item/LSQB' '(' \
	| trreplace '//item/RSQB' ')?' \
	| trdelete '//more_alts[contains(@Before,"
")]/VBAR' \
	| trinsert -a '//rule_/newline[not(following-sibling::*)]' ';' \
	| trinsert '//dedent' '
;
' \
	| trdelete '//memoflag' \
	| trdelete '//forced_atom/AMPER' \
	> o.pt
cat o.pt \
	| trtext > v1.txt
dos2unix v1.txt

strings=`cat o.pt | trxgrep ' //STRING/text()' | grep '"'`
for i in $strings
do
	echo $i
	substring=`echo -n $i | sed 's/"//g'`
	echo $substring
	cat v1.txt | sed 's#"'$substring'"#'"'"$substring"'"'#g' > temp-v1.txt
	mv temp-v1.txt v1.txt
done

cat v1.txt | sed 's%^\([ \t]*\)#%\1//%' > v2.txt
echo "grammar Python3;" > v3.txt
cat v2.txt | sed 's%# %// %' >> v3.txt
cp v3.txt x/Python3.g4
date

# find "item" nodes for things like ";".foobar+.
#  cat orig.pt | trxgrep ' //item/*[position()=1 and name()="atom" and following-sibling::DOT and following-sibling::DOT/following-sibling::atom and following-sibling::DOT/following-sibling::atom/following-sibling::PLUS]/..' | trtree
# find '|' on beginning of line.
#cat orig.pt | trxgrep ' //rule_//more_alts[reverse(preceding-sibling::*)[1]/name()="indent"]' | trtree
#	| trdelete '//more_alts[preceding-sibling::indent/preceding-sibling::newline/preceding-sibling::COLON]/VBAR' \
