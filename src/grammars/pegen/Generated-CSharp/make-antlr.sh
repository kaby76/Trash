#
# "Setting MSYS2_ARG_CONV_EXCL so that Trash XPaths do not get mutulated."
export MSYS2_ARG_CONV_EXCL="*"
date
trparse -d pegen ../examples/python-11.gram > orig.pt

cat orig.pt > o.pt
cat o.pt | trtext > o.txt
cat o.pt | trtree > o.tree
dos2unix o.*

# This refactoring removes occurrences of the "invalid_..." symbols.
# Usually, these occur just after a VBAR.
# Example:
#   params[arguments_ty]:
#    | invalid_parameters
#    | parameters
#
#    =>
#
#   params[arguments_ty]:
#    |
#    | parameters
#
# The main problem this refactoring is the production of "naked VBARs".
# 
#        ( newline
#        ) 
#        ( indent
#        ) 
#        ( more_alts
#          ( intertoken text:\r\n     tt:-1
#          ) 
#          ( VBAR
#            (  text:| tt:0 chnl:DEFAULT_TOKEN_CHANNEL
#          ) ) 
#          ( alts
#            ( intertoken text:\r\n     tt:-1
#            ) 
#            ( VBAR
#              (  text:| tt:0 chnl:DEFAULT_TOKEN_CHANNEL
#            ) ) 
#            ( alt
#              ( items
#                ( named_item
#                  ( item
#                    ( atom
#                      ( name
#                        ( intertoken text:  tt:-1
#                        ) 
#                        ( NAME
#                          (  text:parameters tt:0 chnl:DEFAULT_TOKEN_CHANNEL
#          ) ) ) ) ) ) ) ) ) 
#          ( newline
#        ) ) 
#        ( dedent
#      ) ) 

cat o.pt \
	| trdelete '//alt[items/named_item/item/atom/name/NAME[contains(text(),"invalid_")]]' \
> o2.pt
cat o2.pt | trtext > o2.txt
cat o2.pt | trtree > o2.tree
dos2unix o2.*

cat o2.pt \
	| trdelete '//VBAR[following-sibling::*[1][name()="alts"][*[1]VBAR]]' \
	> o3.pt
cat o3.pt | trtext > o3.txt
cat o3.pt | trtree > o3.tree
dos2unix o3.*
exit	

# This refactoring removes the first VBAR in a new line of alts.
# Example:
#   params[arguments_ty]:
#    | invalid_parameters
#    | parameters
#
#    =>
#
#   params[arguments_ty]:
#     invalid_parameters
#    | parameters
#
# Note, this refactoring affects the search for invalid_* uses.
cat o.pt \
	| trdelete '//more_alts[contains(@Before,"
")]/VBAR' \
> o2.pt

cat o.pt | trtext > o.txt
cat o.pt | trtree > o.tree
cat o2.pt | trtext > o2.txt
cat o2.pt | trtree > o2.tree
dos2unix o.* o2.*
exit
	| trdelete '//VBAR[not(following-sibling::*)]' \
	| trdelete '//meta/(AT | name | string | newline)' \
	| trdelete '//rule_[rulename/name/NAME[contains(text(),"invalid_") or contains(text(),"expression_without_invalid")]]' \
	| trreplace '//name/NAME[text()="ENDMARKER"]' 'EOF' \
	| trreplace '//name/NAME[text()="TYPE_COMMENT"]' 'type_comment' \
	| trdelete '//action' \
	| trreplace '//attribute' ' ' \
	| trreplace '//attribute_name' ' ' \
	| trreplace '//lookahead' ' ' \
	| trreplace '//item/LSQB' '(' \
	| trreplace '//item/RSQB' ')?' \
	| trinsert -a '//rule_/newline[not(following-sibling::*)]' ';' \
	| trinsert '//dedent' '
;
' \
	| trdelete '//memoflag' \
	| trdelete '//forced_atom/AMPER' \
	> o.pt
cat o.pt \
	| trtext \
	> v1.txt
dos2unix v1.txt
strings=`cat o.pt | trquery 'grep //STRING/text()' | grep '"'`
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
exit
rm -f v1.txt
rm -f v2.txt
rm -f o.pt

# find "item" nodes for things like ";".foobar+.
#  cat orig.pt | trquery 'grep //item/*[position()=1 and name()="atom" and following-sibling::DOT and following-sibling::DOT/following-sibling::atom and following-sibling::DOT/following-sibling::atom/following-sibling::PLUS]/..' | trtree
# find '|' on beginning of line.
#cat orig.pt | trquery 'grep //rule_//more_alts[reverse(preceding-sibling::*)[1]/name()="indent"]' | trtree
#	| trdelete '//more_alts[preceding-sibling::indent/preceding-sibling::newline/preceding-sibling::COLON]/VBAR' \
