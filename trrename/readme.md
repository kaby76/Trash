# Trrename

Rename a symbol, the first parameter as specified by the xpath expression string,
to a new name, the second parameter as a string. The result may place all changed
grammars that use the symbol on the stack.

# Usage

    trrename <string> <string>

or

    trrename 

# Examples

    trparse Foobar.g4 | trrename " //parserRuleSpec//labeledAlt//RULE_REF[text() = 'e']" xxx | trprint > new-grammar.g4

# Current version

0.8.3 -- Updated trfoldlit, trgen and templates, trmvsr, trsponge, trunfold, trxml2
