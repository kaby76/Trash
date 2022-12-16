#
# "Setting MSYS2_ARG_CONV_EXCL so that Trash XPaths do not get mutulated."
export MSYS2_ARG_CONV_EXCL="*"
date
trparse -d pegen ../examples/python-11.gram > orig.pt
cat orig.pt \
	| trdelete '//rule_[rulename/name/NAME/text()="invalid_kvpair"]' \
	| trdelete '//rule_[rulename/name/NAME/text()="invalid_arguments"]' \
	| trdelete '//rule_[rulename/name/NAME/text()="invalid_kwarg"]' \
	| trdelete '//rule_[rulename/name/NAME/text()="expression_without_invalid"]' \
	| trdelete '//rule_[rulename/name/NAME/text()="invalid_legacy_expression"]' \
	| trdelete '//rule_[rulename/name/NAME/text()="invalid_expression"]' \
	| trdelete '//rule_[rulename/name/NAME/text()="invalid_named_expression"]' \
	| trdelete '//rule_[rulename/name/NAME/text()="invalid_assignment"]' \
	| trdelete '//rule_[rulename/name/NAME/text()="invalid_ann_assign_target"]' \
	| trdelete '//rule_[rulename/name/NAME/text()="invalid_del_stmt"]' \
	| trdelete '//rule_[rulename/name/NAME/text()="invalid_block"]' \
	| trdelete '//rule_[rulename/name/NAME/text()="invalid_comprehension"]' \
	| trdelete '//rule_[rulename/name/NAME/text()="invalid_dict_comprehension"]' \
	| trdelete '//rule_[rulename/name/NAME/text()="invalid_parameters"]' \
	| trdelete '//rule_[rulename/name/NAME/text()="invalid_default"]' \
	| trdelete '//rule_[rulename/name/NAME/text()="invalid_star_etc"]' \
	| trdelete '//rule_[rulename/name/NAME/text()="invalid_kwds"]' \
	| trdelete '//rule_[rulename/name/NAME/text()="invalid_parameters_helper"]' \
	| trdelete '//rule_[rulename/name/NAME/text()="invalid_lambda_parameters"]' \
	| trdelete '//rule_[rulename/name/NAME/text()="invalid_lambda_parameters_helper"]' \
	| trdelete '//rule_[rulename/name/NAME/text()="invalid_lambda_star_etc"]' \
	| trdelete '//rule_[rulename/name/NAME/text()="invalid_lambda_kwds"]' \
	| trdelete '//rule_[rulename/name/NAME/text()="invalid_double_type_comments"]' \
	| trdelete '//rule_[rulename/name/NAME/text()="invalid_with_item"]' \
	| trdelete '//rule_[rulename/name/NAME/text()="invalid_for_target"]' \
	| trdelete '//rule_[rulename/name/NAME/text()="invalid_group"]' \
	| trdelete '//rule_[rulename/name/NAME/text()="invalid_import_from_targets"]' \
	| trdelete '//rule_[rulename/name/NAME/text()="invalid_with_stmt"]' \
	| trdelete '//rule_[rulename/name/NAME/text()="invalid_with_stmt_indent"]' \
	| trdelete '//rule_[rulename/name/NAME/text()="invalid_try_stmt"]' \
	| trdelete '//rule_[rulename/name/NAME/text()="invalid_except_stmt"]' \
	| trdelete '//rule_[rulename/name/NAME/text()="invalid_finally_stmt"]' \
	| trdelete '//rule_[rulename/name/NAME/text()="invalid_except_stmt_indent"]' \
	| trdelete '//rule_[rulename/name/NAME/text()="invalid_except_star_stmt_indent"]' \
	| trdelete '//rule_[rulename/name/NAME/text()="invalid_match_stmt"]' \
	| trdelete '//rule_[rulename/name/NAME/text()="invalid_case_block"]' \
	| trdelete '//rule_[rulename/name/NAME/text()="invalid_as_pattern"]' \
	| trdelete '//rule_[rulename/name/NAME/text()="invalid_class_pattern"]' \
	| trdelete '//rule_[rulename/name/NAME/text()="invalid_class_argument_pattern"]' \
	| trdelete '//rule_[rulename/name/NAME/text()="invalid_if_stmt"]' \
	| trdelete '//rule_[rulename/name/NAME/text()="invalid_elif_stmt"]' \
	| trdelete '//rule_[rulename/name/NAME/text()="invalid_else_stmt"]' \
	| trdelete '//rule_[rulename/name/NAME/text()="invalid_while_stmt"]' \
	| trdelete '//rule_[rulename/name/NAME/text()="invalid_for_stmt"]' \
	| trdelete '//rule_[rulename/name/NAME/text()="invalid_def_raw"]' \
	| trdelete '//rule_[rulename/name/NAME/text()="invalid_class_def_raw"]' \
	| trdelete '//rule_[rulename/name/NAME/text()="invalid_double_starred_kvpairs"]' \
	| trreplace '//name/NAME[text()="ENDMARKER"]' 'EOF' \
	| trreplace '//name/NAME[text()="TYPE_COMMENT"]' 'type_comment' \
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
cat << HERE > Python3Parser.g4
parser grammar Python3Parser;
options {
    superClass = Python3ParserBase;
    tokenVocab=Python3Lexer;
}
type_comment: ;
HERE
cat v2.txt | sed 's%# %// %' >> Python3Parser.g4
cp Python3Parser.g4 x
cp Python3Lexer.g4 x
date
rm -f v1.txt
rm -f v2.txt
rm -f o.pt

# find "item" nodes for things like ";".foobar+.
#  cat orig.pt | trxgrep ' //item/*[position()=1 and name()="atom" and following-sibling::DOT and following-sibling::DOT/following-sibling::atom and following-sibling::DOT/following-sibling::atom/following-sibling::PLUS]/..' | trtree
# find '|' on beginning of line.
#cat orig.pt | trxgrep ' //rule_//more_alts[reverse(preceding-sibling::*)[1]/name()="indent"]' | trtree
#	| trdelete '//more_alts[preceding-sibling::indent/preceding-sibling::newline/preceding-sibling::COLON]/VBAR' \
