#
# "Setting MSYS2_ARG_CONV_EXCL so that Trash XPaths do not get mutulated."
export MSYS2_ARG_CONV_EXCL="*"
date
trparse -d pegen ../examples/python-11.gram > orig.pt
cat orig.pt \
	| trdelete '//meta/(AT | name | string | newline)' \
	| trdelete '//rule_[rulename/name/NAME[contains(text(),"invalid_") or contains(text(),"expression_without_invalid")]]' \
	| trdelete '//action' \
	| trreplace '//attribute' ' ' \
	| trreplace '//attribute_name' ' ' \
	| trreplace '//lookahead' ' ' \
	| trdelete '//memoflag' \
	| trdelete '//forced_atom/AMPER' \
	| trdelete '//alt[items/named_item/item/atom/name/NAME[contains(text(),"invalid_")]]' \
	| trdelete '//VBAR[not(following-sibling::*)]' \
	| trtext \
	| grep -E -v '^[ \t]+[|]$' \
	| sed "s#default : '=' expression |#default : '=' expression#" \
	> stripped.gram
date

#	| trdelete '//VBAR[following-sibling::*[1][name()="VBAR"]]' \
