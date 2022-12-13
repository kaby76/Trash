#
date
trparse -d pegen ../examples/python-11.gram \
	| trxgrep ' //rule_' \
	| trdelete ' //attribute' \
	| trdelete ' //action' \
	| trdelete ' //attribute_name' \
	| trdelete ' //more_alts[preceding-sibling::indent/preceding-sibling::newline/preceding-sibling::COLON]/VBAR' \
	| trtext > new-python-11.gram
date
