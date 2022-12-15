#
# "Setting MSYS2_ARG_CONV_EXCL so that Trash XPaths do not get mutulated."
export MSYS2_ARG_CONV_EXCL="*"
date
trparse -d pegen ../examples/python-11.gram \
	| trxgrep '//rule_' \
	| trdelete '//attribute' \
	| trdelete '//action' \
	| trdelete '//attribute_name/(EQUAL | name/NAME)' \
	| trinsert '//items/named_item[lookahead/TILDE]' '/*' \
	> o.pt
cat o.pt \
	| trinsert -v -a '//items[named_item/lookahead/TILDE]/following-sibling::items' '*/' \
	> o2.pt
	exit
	| trreplace '//item/LSQB' '(' \
	| trreplace '//item/RSQB' ')?' \
	| trdelete '//more_alts[preceding-sibling::indent/preceding-sibling::newline/preceding-sibling::COLON]/VBAR' \
	| trinsert -a '//rule_/newline[not(following-sibling::*)]' ';' \
	| trinsert '//dedent' '
;
' \
	| trtext > v1.txt
dos2unix v1.txt
cat v1.txt | sed 's%^\([ \t]*\)#%\1//%' > v2.txt
cat v2.txt | sed 's%# %// %' > v3.txt
date
