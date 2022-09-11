# Trash, a shell for transforming grammars

## Commands
### tranalyze

Reads an Antlr4 grammar in the form of parse tree data from stdin,
searches for problems in the grammar, and outputs the results to stdout.

#### Usage

    tranalyze

#### Details

`tranalyze` performs a multi-pass search of a grammar in the
form of a parse result (from `trparse`), looking for problems in the
grammar.

* Classify each node type and output a count for each type.
* Check for unused symbols.
* Check for Unicode literals of the type '\unnnn' with
numbers that are over 32-bits.
* Check for common groupings in alts.
* Check for useless parentheses.
* Identify if a symbol derives the empty string, a non-empty string, or both.
* Check for unhalting nonterminals symbols in a single rule or group of rules.

#### Example

Consider the following combined grammar.

_Input to command_

	grammar Test;

	start : 'a' empty infinite0 infinite1 infinite2 ;
	unused : 'b';
	infinite0 : (infinite0 'c')* ;  // Not legal in Antlr4 MLR, but okay
	infinite1 : (infinite1 'c')+ ;  // Not legal in Antlr4 MLR, infinite
	infinite2 : ('c' infinite2)+ ;  // Not flagged by Antlr4, infinite
	empty : ;

_Command_

    trparse Test.g4 | tranalyze

_Output_

	6 occurrences of Antlr - nonterminal def
	7 occurrences of Antlr - nonterminal ref
	1 occurrences of Antlr - keyword
	5 occurrences of Antlr - literal
	Rule start is NonEmpty
	Rule unused is NonEmpty
	Rule infinite0 is NonEmpty
	Rule infinite1 is NonEmpty
	Rule infinite2 is NonEmpty
	Rule empty is Empty
	Rule infinite1 is malformed. It does not derive a string with referencing itself.
	Rule infinite2 is malformed. It does not derive a string with referencing itself.


#### Current version

0.17.0 -- Fixes for all tools (piped data structures), but in particular trparse. Add trperf, trpiggy.
### trcombine

Combine two grammars into one.
One grammar must be a lexer grammar, the other a parser grammar,
order is irrelevant. The output is parse tree data.

#### Usage

    trcombine <grammar1> <grammar2>

#### Details

`trcombine` combines grammars that are known as "split grammars"
(separate Antlr4 lexer and parser grammars)
into one grammar, known as a "combined grammar". This refactoring is
useful if a simplified grammar grammar is wanted, and if possible if
the split grammar does not use the "superClass" option in one or the other
grammars. The opposite refactoring is implemented by
[trsplit](https://github.com/kaby76/Domemtech.Trash/tree/main/trsplit).

The split refactoring performs several operations:

* Combine the two files together, parser grammar first, then lexer grammar.
* Remove the `grammarDecl` for the lexer rules, and change the `grammarDecl`
for the parser rules to be a combined grammar declaration. Rename the name
of the parser grammar to not have "Parser" at the tail of the name.
* Remove the `optionsSpec` for the lexer section.
* Remove any occurrence of "tokenVocab" from the `optionsSpec` of the parser section.
* If the `optionsSpec` is empty, it is removed.

The order of the two grammars is ignored: the parser rules always will appear
before the lexer rules.

Lexer modes will require manual fix-up.

### Example

Consider the following grammar that is split.

_Input to command_

Lexer grammar in ExpressionLexer.g4:

    lexer grammar ExpressionLexer;
    VARIABLE : VALID_ID_START VALID_ID_CHAR* ;
    fragment VALID_ID_START : ('a' .. 'z') | ('A' .. 'Z') | '_' ;
    fragment VALID_ID_CHAR : VALID_ID_START | ('0' .. '9') ;
    INT : ('0' .. '9')+ ;
    MUL : '*' ;
    DIV : '/' ;
    ADD : '+' ;
    SUB : '-' ;
    LP : '(' ;
    RP : ')' ;
    WS : [ \r\n\t] + -> skip ;

Parser grammar in ExpressionParser.g4:

    parser grammar ExpressionParser;
    e : e ('*' | '/') e
     | e ('+' | '-') e
     | '(' e ')'
     | ('-' | '+')* a
     ;
    a : number | variable ;
    number : INT ;
    variable : VARIABLE ;

_Command_

    trcombine ExpressionLexer.g4 ExpressionParser.g4 | trprint > Expression.g4

Combined grammar in Expression.g4:

    grammar Expression;
    e : e ('*' | '/') e
     | e ('+' | '-') e
     | '(' e ')'
     | ('-' | '+')* a
     ;
    a : number | variable ;
    number : INT ;
    variable : VARIABLE ;
    VARIABLE : VALID_ID_START VALID_ID_CHAR* ;
    fragment VALID_ID_START : ('a' .. 'z') | ('A' .. 'Z') | '_' ;
    fragment VALID_ID_CHAR : VALID_ID_START | ('0' .. '9') ;
    INT : ('0' .. '9')+ ;
    MUL : '*' ;
    DIV : '/' ;
    ADD : '+' ;
    SUB : '-' ;
    LP : '(' ;
    RP : ')' ;
    WS : [ \r\n\t] + -> skip ;

The original grammars are left unchanged.

#### Current version

0.17.0 -- Fixes for all tools (piped data structures), but in particular trparse. Add trperf, trpiggy.
### Trconvert

Reads a grammar from stdin and converts the grammar to/from Antlr version 4
syntax. The original grammar must be for a supported type (Antlr2, Antlr3,
Bison, W3C EBNF, Lark). The input and output are Parse Tree Data.

#### Usage

    trconvert [-t <type>]

#### Details

This command converts a grammar from one type to another. Most
conversions will handle only simple syntax differences. More complicated
scenarios are supported depending on the source and target grammar types.
For example, Bison is converted to Antlr4, but the reverse is not
implemented yet.

`trconvert` takes an option target type. If it is not used, the default
is to convert the input of whatever type to Antlr4 syntax. The output
of `trconvert` is a parse tree containing the converted grammar.

#### Examples

_Conversion of Antlr4 Abnf to Lark Abnf_

    grammar Abnf;

    rulelist : rule_* EOF ;
    rule_ : ID '=' '/'? elements ;
    elements : alternation ;
    alternation : concatenation ( '/' concatenation )* ;
    concatenation : repetition + ;
    repetition : repeat_? element ;
    repeat_ : INT | ( INT? '*' INT? ) ;
    element : ID | group | option | STRING | NumberValue | ProseValue ;
    group : '(' alternation ')' ;
    option : '[' alternation ']' ;
    NumberValue : '%' ( BinaryValue | DecimalValue | HexValue ) ;
    fragment BinaryValue : 'b' BIT+ ( ( '.' BIT+ )+ | ( '-' BIT+ ) )? ;
    fragment DecimalValue : 'd' DIGIT+ ( ( '.' DIGIT+ )+ | ( '-' DIGIT+ ) )? ;
    fragment HexValue : 'x' HEX_DIGIT+ ( ( '.' HEX_DIGIT+ )+ | ( '-' HEX_DIGIT+ ) )? ;
    ProseValue : '<' ( ~ '>' )* '>' ;
    ID : LETTER ( LETTER | DIGIT | '-' )* ;
    INT : '0' .. '9'+ ;
    COMMENT : ';' ~ ( '\n' | '\r' )* '\r'? '\n' -> channel ( HIDDEN ) ;
    WS : ( ' ' | '\t' | '\r' | '\n' ) -> channel ( HIDDEN ) ;
    STRING : ( '%s' | '%i' )? '"' ( ~ '"' )* '"' ;
    fragment LETTER : 'a' .. 'z' | 'A' .. 'Z' ;
    fragment BIT : '0' .. '1' ;
    fragment DIGIT : '0' .. '9' ;
    fragment HEX_DIGIT : ( '0' .. '9' | 'a' .. 'f' | 'A' .. 'F' ) ;

_Command_

    trparse Abnf.g4 | trconvert -t lark | trprint > Abnf.lark

_Output_

    rulelist :  rule_ * EOF 
    rule_ :  ID "=" "/" ? elements 
    elements :  alternation 
    alternation :  concatenation ( "/" concatenation ) * 
    concatenation :  repetition + 
    repetition :  repeat_ ? element 
    repeat_ :  INT | ( INT ? "*" INT ? ) 
    element :  ID | group | option | STRING | NUMBERVALUE | PROSEVALUE 
    group :  "(" alternation ")" 
    option :  "[" alternation "]" 
    NUMBERVALUE :  "%" ( BINARYVALUE | DECIMALVALUE | HEXVALUE ) 
    BINARYVALUE :  "b" BIT + ( ( "." BIT + ) + | ( "-" BIT + ) ) ? 
    DECIMALVALUE :  "d" DIGIT + ( ( "." DIGIT + ) + | ( "-" DIGIT + ) ) ? 
    HEXVALUE :  "x" HEX_DIGIT + ( ( "." HEX_DIGIT + ) + | ( "-" HEX_DIGIT + ) ) ? 
    PROSEVALUE :  "<" ( /(?!>)/ ) * ">" 
    ID :  LETTER ( LETTER | DIGIT | "-" ) * 
    INT :  "0" .. "9" + 
    COMMENT :  ";" /(?!\n|\r)/ * "\r" ? "\n" 
    WS :  ( " " | "\t" | "\r" | "\n" ) 
    STRING :  ( "%s" | "%i" ) ? "\"" ( /(?!")/ ) * "\"" 
    LETTER :  "a" .. "z" | "A" .. "Z" 
    BIT :  "0" .. "1" 
    DIGIT :  "0" .. "9" 
    HEX_DIGIT :  ( "0" .. "9" | "a" .. "f" | "A" .. "F" ) 

    %ignore COMMENT
    %ignore WS

#### Current version

0.17.0 -- Fixes for all tools (piped data structures), but in particular trparse. Add trperf, trpiggy.
### Delabel

Remove all labels from an Antlr4 grammar.

    "expr : lhs=expr (PLUS | MINUS) rhs=expr # foobar1 ....." => "expr : expr (PLUS | MINUS) expr ....."

#### Usage

    trdelabel

#### Example

    trparse A.g4 | delabel | trprint

#### Current version

0.17.0 -- Fixes for all tools (piped data structures), but in particular trparse. Add trperf, trpiggy.
### Trdelete

Reads a parse tree from stdin, deletes nodes in the tree using
the specified XPath expression, and writes the modified tree
to stdout. The input and output are Parse Tree Data.

#### Usage

    trdelete <string>

#### Example

Before:

    grammar Expression;
    e : e ('*' | '/') e
      | e ('+' | '-') e
      | '(' e ')'
      | ('-' | '+')* a
      ;
    a : INT ;
    INT : ('0' .. '9')+ ;
    MUL : '*' ;
    DIV : '/' ;
    ADD : '+' ;
    SUB : '-' ;
    LP : '(' ;
    RP : ')' ;
    WS : [ \r\n\t] + -> skip ;

Command:

    trparse Expression.g4 | trdelete "//parserRuleSpec[RULE_REF/text() = 'a']" | trprint

After:

    grammar Expression;
    e : e ('*' | '/') e
      | e ('+' | '-') e
      | '(' e ')'
      | ('-' | '+')* a
      ;
    a : INT ;
    INT : ('0' .. '9')+ ;
    MUL : '*' ;
    DIV : '/' ;
    ADD : '+' ;
    SUB : '-' ;
    LP : '(' ;
    RP : ')' ;
    WS : [ \r\n\t] + -> skip ;

#### Notes

If you are running MSYS2 on Windows, you may notice that XPaths are not being
processed by this command correctly. To avoid the Bash shell from altering
XPaths, type _export MSYS2_ARG_CONV_EXCL="*"_, then execute your command.

#### Current version

0.17.0 -- Fixes for all tools (piped data structures), but in particular trparse. Add trperf, trpiggy.
### trdot

Reads a tree from stdin and prints the tree as a Dot graph.

#### Usage

    trdot

#### Details

`trdot` reads parse tree data via stdin and outputs
a Dot graph specification. The stdout can be redirected to
save the output to a file. Or, you can copy the output and
use an online Dot graph visualizer to make a plot.
Any parse tree data can be converted to Dot, include a
parse of a grammar, the parse tree of a simple expression grammar,
or a list of parse tree nodes obtained via
[trxgrep](https://github.com/kaby76/Domemtech.Trash/tree/main/trxgrep).

#### Examples

Consider the Expression grammar, obtained via

    mkdir foo; cd foo; trgen; cd Generated; dotnet build

Let's parse the expression "1+2" and print the parse tree as a Dot graph:

    trparse -i "1+2" | trdot

The output will be:

    digraph G {
    Node18643596 [label="file_"];
    Node33574638 [label="expression"];
    Node33736294 [label="expression"];
    Node35191196 [label="atom"];
    Node48285313 [label="scientific"];
    Node31914638 [label="1"];
    Node18796293 [label="+"];
    Node34948909 [label="expression"];
    Node46104728 [label="atom"];
    Node12289376 [label="scientific"];
    Node43495525 [label="2"];
    Node55915408 [label="EOF"];
    Node18643596 -> Node33574638;
    Node18643596 -> Node55915408;
    Node33574638 -> Node33736294;
    Node33574638 -> Node18796293;
    Node33574638 -> Node34948909;
    Node34948909 -> Node46104728;
    Node46104728 -> Node12289376;
    Node12289376 -> Node43495525;
    Node33736294 -> Node35191196;
    Node35191196 -> Node48285313;
    Node48285313 -> Node31914638;
    }

#### Current version

0.17.0 -- Fixes for all tools (piped data structures), but in particular trparse. Add trperf, trpiggy.
### Trenum

#### Usage

#### Examples

#### Notes

#### Current version

0.17.0 -- Fixes for all tools (piped data structures), but in particular trparse. Add trperf, trpiggy.
### trfirst

Outputs first sets of a grammar.

#### Usage

trfirst k

#### Details

#### Example

#### Notes

If you are running MSYS2 on Windows, you may notice that XPaths are not being
processed by this command correctly. To avoid the Bash shell from altering
XPaths, type _export MSYS2_ARG_CONV_EXCL="*"_, then execute your command.

#### Current version

0.17.0 -- Fixes for all tools (piped data structures), but in particular trparse. Add trperf, trpiggy.
### Trfold

Reads a parse tree from stdin, replaces a sequence of symbols on
the RHS of a rule with the rule LHS symbol, and writes the modified tree
to stdout. The input and output are Parse Tree Data.

#### Usage

    trfold <string>

#### Example

    trparse A.g4 | trfold "//parserRuleSpec[RULE_REF/text() = 'normalAnnotation']"

#### Notes

If you are running MSYS2 on Windows, you may notice that XPaths are not being
processed by this command correctly. To avoid the Bash shell from altering
XPaths, type _export MSYS2_ARG_CONV_EXCL="*"_, then execute your command.

#### Current version

0.17.0 -- Fixes for all tools (piped data structures), but in particular trparse. Add trperf, trpiggy.
### Trfoldlit

Reads a parse tree from stdin, replaces a string literals on
the RHS of a rule with the lexer rule LHS symbol, and writes
the modified parsing result set to stdout. The input and
output are Parse Tree Data.

#### Usage

    trfoldlit

#### Examples

Before:

    grammar Expression;
    e : e ('*' | '/') e
      | e ('+' | '-') e
      | '(' e ')'
      | ('-' | '+')* a
      ;
    a : INT ;
    INT : ('0' .. '9')+ ;
    MUL : '*' ;
    DIV : '/' ;
    ADD : '+' ;
    SUB : '-' ;
    LP : '(' ;
    RP : ')' ;
    WS : [ \r\n\t] + -> skip ;

Command:

    trparse Expression.g4 | trfoldlit | trsponge -c

After:

    grammar Expression;
    e : e (MUL | DIV) e
      | e (ADD | SUB) e
      | LP e RP
      | (SUB | ADD)* a
      ;
    a : INT ;
    INT : ('0' .. '9')+ ;
    MUL : '*' ;
    DIV : '/' ;
    ADD : '+' ;
    SUB : '-' ;
    LP : '(' ;
    RP : ')' ;
    WS : [ \r\n\t] + -> skip ;

#### Notes

If you are running MSYS2 on Windows, you may notice that XPaths are not being
processed by this command correctly. To avoid the Bash shell from altering
XPaths, type _export MSYS2_ARG_CONV_EXCL="*"_, then execute your command.

#### Current version

0.17.0 -- Fixes for all tools (piped data structures), but in particular trparse. Add trperf, trpiggy.
### trprint

Read stdin and format the grammar.

#### Usage

    trformat

#### Examples

    trparse A.g4 | trformat

#### Current version

0.17.0 -- Fixes for all tools (piped data structures), but in particular trparse. Add trperf, trpiggy.
### Trgen

Generate a parser application using the Antlr tool and application templates.
The generated parser is placed in the directory <current-directory>/Generated/.
If there is a `pom.xml` file in the current directory, `trgen` will read
it for information on the grammar. If there is no `pom.xml` file, the start
rule must be provided. If the current directory is empty, `trgen` will
create a parser for the Arithmetic.g4 grammar.

#### Usage

    trgen <options>* 

#### Examples

    trgen

#### Current version

0.17.0 -- Fixes for all tools (piped data structures), but in particular trparse. Add trperf, trpiggy.
### Trgen2

Generate files from template and a parameterize list of names and values.
The generated parser is placed in the directory <current-directory>/Generated/.

#### Usage

    trgen2 <options>* 

#### Examples

    trgen

#### Current version

0.17.0 -- Fixes for all tools (piped data structures), but in particular trparse. Add trperf, trpiggy.
### Trgroup

Perform a recursive left- and right- factorization of alternatives for rules.

#### Usage

    trgroup <string>

#### Details

The command reads all parse tree data. Then, for each parse tree,
the XPath expression argument specified will be evaluated.

The nodes specified in the XPath arg must be for one or more
ruleAltList, lexerAltList, or altList. These node types contain
a sequence of children alternating with an "|"-operator
(`ruleAltList : labeledAlt ('|' labeledAlt)*`,
`lexerAltList : lexerAlt ('|' lexerAlt)*, and
`altList : alternative ('|' alternative)*`).

A "unification" of all the non-'|' children in the node is performed,
which results in a single sequence of elements with groupings. It is
possible for there to be multiple groups in the set of alternatives.

#### Examples

_Input to command (file "temp.g4")_

    grammar temp;
    a : 'X' 'B' 'Z' | 'X' 'C' 'Z' | 'X' 'D' 'Z' ;

_Command_

    trparse temp.g4 | trgroup "//parserRuleSpec[RULE_REF/text()='a']//ruleAltList" | trsponge -c true
    
    # Or, a file-wide group refactoring, over all parser and lexer rules:
    
    trparse temp.g4 | trgroup | trsponge -c true

_Output_

    grammar temp;
    a : 'X' ( 'B' | 'C' | 'D' ) 'Z' ;

### Notes

If you are running MSYS2 on Windows, you may notice that XPaths are not being
processed by this command correctly. To avoid the Bash shell from altering
XPaths, type _export MSYS2_ARG_CONV_EXCL="*"_, then execute your command.

#### Current version

0.17.0 -- Fixes for all tools (piped data structures), but in particular trparse. Add trperf, trpiggy.
### trinsert

Reads a parse tree from stdin, inserts text before or after
nodes in the tree using
the specified XPath expression, and writes the modified tree
to stdout. The input and output are Parse Tree Data.

#### Usage

    trinsert <-a>? <xpath-string> <text-string>

#### Details

The command reads all parse tree data. Then, for each parse tree,
the XPath expression argument specified will be evaluated.

The nodes specified in the XPath arg are for one or more
nodes of any type in a parse tree of any type.

For each node, the program inserts a string node in the parent's
list of children nodes prior to the node. Off-channel tokens occur
before the inserted text. If you specify the `-a` option, the text
is inserted after the node.

After performing the insert, if it is a grammar, the text is reparsed
and an entire new parse tree outputed.

#### Example

    trparse Java.g4 | trinsert "//parserRuleSpec[RULE_REF/text() = 'normalAnnotation']" " /* This is a comment */" | trtree | vim -

#### Notes

If you are running MSYS2 on Windows, you may notice that XPaths are not being
processed by this command correctly. To avoid the Bash shell from altering
XPaths, type _export MSYS2_ARG_CONV_EXCL="*"_, then execute your command.

#### Current version

0.17.0 -- Fixes for all tools (piped data structures), but in particular trparse. Add trperf, trpiggy.
### trdot

Reads a tree from stdin and prints the tree as a Dot graph.

#### Usage

    trdot

#### Details

`trdot` reads parse tree data via stdin and outputs
a Dot graph specification. The stdout can be redirected to
save the output to a file. Or, you can copy the output and
use an online Dot graph visualizer to make a plot.
Any parse tree data can be converted to Dot, include a
parse of a grammar, the parse tree of a simple expression grammar,
or a list of parse tree nodes obtained via
[trxgrep](https://github.com/kaby76/Domemtech.Trash/tree/main/trxgrep).

#### Examples

Consider the Expression grammar, obtained via

    mkdir foo; cd foo; trgen; cd Generated; dotnet build

Let's parse the expression "1+2" and print the parse tree as a Dot graph:

    trparse -i "1+2" | trdot

The output will be:

    digraph G {
    Node18643596 [label="file_"];
    Node33574638 [label="expression"];
    Node33736294 [label="expression"];
    Node35191196 [label="atom"];
    Node48285313 [label="scientific"];
    Node31914638 [label="1"];
    Node18796293 [label="+"];
    Node34948909 [label="expression"];
    Node46104728 [label="atom"];
    Node12289376 [label="scientific"];
    Node43495525 [label="2"];
    Node55915408 [label="EOF"];
    Node18643596 -> Node33574638;
    Node18643596 -> Node55915408;
    Node33574638 -> Node33736294;
    Node33574638 -> Node18796293;
    Node33574638 -> Node34948909;
    Node34948909 -> Node46104728;
    Node46104728 -> Node12289376;
    Node12289376 -> Node43495525;
    Node33736294 -> Node35191196;
    Node35191196 -> Node48285313;
    Node48285313 -> Node31914638;
    }

#### Current version

0.17.0 -- Fixes for all tools (piped data structures), but in particular trparse. Add trperf, trpiggy.
### Trjson

Read a tree from stdin and write a JSON represenation of it.

#### Usage

    trjson

#### Examples

    trparse A.g4 | trjson | less

#### Current version

0.17.0 -- Fixes for all tools (piped data structures), but in particular trparse. Add trperf, trpiggy.
### trkleene

Replace a rule with an EBNF form if it contains direct left or direct right recursion.

#### Usage

    trkleene <string>?

#### Details

`trkleene` refactors rules in a grammar with direct left or direct right
recursion. The program first reads from stdin the parse tree data of
grammar files(x). It then searches
the parse tree for the nodes identified by the XPath expression argument
or if none given, all parser and lexer rules in the grammar.
The XPath argument can select any node for the rule (e.g., the LHS symbol,
any RHS symbol, the colon in the rule, etc). The program will finally
replace the RHS of each rule selected with a "Kleene" version of the rule,
removing the recursion. The updated grammar(s) as parse tree data
is outputed to stdout.

#### Examples

    trparse A.g4 | trkleene
    trparse A.g4 | trkleene "//parserRuleSpec/RULE_REF[text()='packageOrTypeName']"

#### Notes

If you are running MSYS2 on Windows, you may notice that XPaths are not being
processed by this command correctly. To avoid the Bash shell from altering
XPaths, type _export MSYS2_ARG_CONV_EXCL="*"_, then execute your command.

#### Current version

0.17.0 -- Fixes for all tools (piped data structures), but in particular trparse. Add trperf, trpiggy.
### trmove

Reads a parse tree from stdin, moves
nodes in the tree using
the specified XPath expression, and writes the modified tree
to stdout. The input and output are Parse Tree Data.

#### Usage

    trmove <-a>? <xpath-string-1> <xpath-string-2>

#### Details

The command reads all parse tree data. Then, for each parse tree,
the XPath expression argument specified will be evaluated.

The nodes specified in the XPath arg are for one or more
nodes of any type in a parse tree of any type.

For each node, the program inserts a string node in the parent's
list of children nodes prior to the node. Off-channel tokens occur
before the inserted text. If you specify the `-a` option, the text
is inserted after the node.

After performing the insert, if it is a grammar, the text is reparsed
and an entire new parse tree outputed.

#### Example

    trparse Java.g4 | trinsert "//parserRuleSpec[RULE_REF/text() = 'normalAnnotation']" " /* This is a comment */" | trtree | vim -

#### Notes

If you are running MSYS2 on Windows, you may notice that XPaths are not being
processed by this command correctly. To avoid the Bash shell from altering
XPaths, type _export MSYS2_ARG_CONV_EXCL="*"_, then execute your command.

#### Current version

0.17.0 -- Fixes for all tools (piped data structures), but in particular trparse. Add trperf, trpiggy.
### Trparse

Parse files and output to stdout parse tree data.
The tool requires a pre-built parser via trgen for a grammar
for anything other than the standard parser grammars that
are supported. To specify the grammar, you can either
be in a trgen-generated parser directory, or use the -p option.

If using positional args on the command line, a file is parse
depending on the extension of the file name:

* `.g2` for an Antlr2
* `.g3` for an Antlr3
* `.g4` for an Antlr4
* `.y` for a Bison
* `.ebnf` for ISO EBNF

You can force the type of parse with
the `--type` command-line option:

* `antlr2` for Antlr2
* `antlr3` for Antlr3
* `antlr4` for Antlr4
* `bison` for Bison
* `ebnf` for ISO EBNF
* `gen` for the `Generated/` parser

#### Usage
    
    trparse (<string> | <options>)*
    -i, --input      Parse the given string as input.
    -t, --type       Specifies type of parse, antlr4, antlr3, antlr2, bison, ebnf, gen 
    -s, --start-rule Start rule name.
    -p, --parser     Location of pre-built parser (aka the trgen Generated/ directory)

#### Examples

    trparse Java.g2
    trparse -i "1+2+3"
    trparse Foobar.g -t antlr2
    echo "1+2+3" | trparse | trtree
    mkdir out; trparse MyParser.g4 MyLexer.g4 | trkleene | trsponge -o out

#### Current version

0.17.0 -- Fixes for all tools (piped data structures), but in particular trparse. Add trperf, trpiggy.
### Trperf

Parse files and output to stdout parse tree data using
performance analysis turned on.
The tool requires a pre-built parser via trgen for a grammar
for anything other than the standard parser grammars that
are supported. To specify the grammar, you can either
be in a trgen-generated parser directory, or use the -p option.

#### Usage
    
    trperf (<string> | <options>)*
    -i, --input      String to parse.
    -s, --start-rule Start rule name.
    -p, --parser     Location of pre-built parser (aka the trgen Generated/ directory)

#### Examples

    # print out performance data for a parse, ignore the header line, sort on "Max k", and output in a formatted table.
    trperf aggregate01.sql | tail -n +2 | sort -k6 -n -r | column -t

#### Current version

0.17.0 -- Fixes for all tools (piped data structures), but in particular trparse. Add trperf, trpiggy.
### Trpiggy

Read from stdin a parsing result set, modify the trees using a template engine, then
output the modified parsing result set. The command also reads a template to follow
as the first argument to the command. The template extends the well-known visitor
pattern used by Antlr with a template that contains strings and xpath expressions
that defines children.

#### Usage

    trpiggy template-spec

#### Examples

Assume "lua.g4" grammar.

Doc input "input.txt":
```
local   tbl = {
   SomeObject = {
      Key = "Value",
      AnotherKey = {
         Key1 = "Value 1",
         Key2 = "Value 2",
         Key3 = "Value 3",
      }
   },
   AnotherObject = {
      Key = "Value",
      AnotherKey = {
         Key1 = "Value 1",
         Key2 = "Value 2",
         Key3 = "Value 3",
      }
   }
}
```
Template input "templates.txt":
```
//chunk -> {{<block>}} ;
//block -> {{<stat>}} ;
//stat[attnamelist and explist] -> {{<explist>}} ;
//explist -> {{ {<exp>} }} ;
//exp[position()=1 and tableconstructor] -> {{<tableconstructor>}} ;
//exp[position()>1 and tableconstructor] -> {{, <tableconstructor>}} ;
//fieldlist -> {{<field>}} ;
//field[position()>1] -> {{, "<NAME>" : <exp> }} ;
//field[position()=1] -> {{ "<NAME>" : <exp> }} ;
```

    trparse input.txt  | trpiggy templates.txt | trprint

Output:
```
 {{ "SomeObject " : { "Key " : "Value"  , "AnotherKey " : { "Key1 " : "Value 1"  , "Key2 " : "Value 2" , "Key3 " : "Value 3"  } } , "AnotherObject " : { "Key " : "Value"  , "AnotherKey " : { "Key1 " : "Value 1"  , "Key2 " : "Value 2" , "Key3 " : "Value 3"  } } }}
```

#### Current version

0.17.0 -- Fixes for all tools (piped data structures), but in particular trparse. Add trperf, trpiggy.
### trprint

Read stdin and print out the text for the tree.

#### Usage

    trprint

#### Examples

    trparse A.g4 | trprint

#### Current version

0.17.0 -- Fixes for all tools (piped data structures), but in particular trparse. Add trperf, trpiggy.
### Trrename

Rename a symbol in a grammar.

#### Usage

    trrename -r <string>

#### Details

`trrename` renames rule symbols in a grammar.

The `-r` option is required. It
is a list of semi-colon delimited pairs of symbol names, which are separated
by a comma, e.g., `id,identifier;name,name_`. If you are using Bash,
make sure to enclose the argument as it contains semi-colons.

#### Examples

    trparse Foobar.g4 | trrename -r "a,b;c,d" | trprint > new-grammar.g4

#### Current version

0.17.0 -- Fixes for all tools (piped data structures), but in particular trparse. Add trperf, trpiggy.
### trreplace

Reads a parse tree from stdin, replaces nodes with text using
the specified XPath expression, and writes the modified tree
to stdout. The input and output are Parse Tree Data.

#### Usage

    trreplace <xpath-string> <text-string>

#### Example

    trparse Java.g4 | trreplace "//parserRuleSpec[RULE_REF/text() = 'normalAnnotation']" "nnn" | trtree | vim -

#### Notes

If you are running MSYS2 on Windows, you may notice that XPaths are not being
processed by this command correctly. To avoid the Bash shell from altering
XPaths, type _export MSYS2_ARG_CONV_EXCL="*"_, then execute your command.

#### Current version

0.17.0 -- Fixes for all tools (piped data structures), but in particular trparse. Add trperf, trpiggy.
### trrup

Remove useless parentheses from a grammar.

#### Usage

    trrup <string>?

#### Details

`trrup` removes useless parentheses in a grammar, at a specific point
in the grammar as specified by the xpath expression, or the entire
file if the xpath expression is not given.

#### Example

_Input to command_

grammar:

    grammar Expression;
    v : ( ( VALID_ID_START  ( VALID_ID_CHAR*) ) ) ;

_Command_

    trparse Expression.g4 | trrup | trprint

_Result_

    grammar Expression;
    v : VALID_ID_START VALID_ID_CHAR* ;

#### Notes

If you are running MSYS2 on Windows, you may notice that XPaths are not being
processed by this command correctly. To avoid the Bash shell from altering
XPaths, type _export MSYS2_ARG_CONV_EXCL="*"_, then execute your command.

#### Current version

0.17.0 -- Fixes for all tools (piped data structures), but in particular trparse. Add trperf, trpiggy.
### trrup

Remove useless parentheses from a grammar.

#### Usage

    trrup

#### Details

`trrup` removes useless parentheses in a grammar.

#### Example

_Input to command_

grammar:

    grammar Expression;
    v : ( ( VALID_ID_START  ( VALID_ID_CHAR*) ) ) ;

_Command_

    trparse Expression.g4 | trrup | trprint

_Result_

    grammar Expression;
    v : VALID_ID_START VALID_ID_CHAR* ;

#### Notes

If you are running MSYS2 on Windows, you may notice that XPaths are not being
processed by this command correctly. To avoid the Bash shell from altering
XPaths, type _export MSYS2_ARG_CONV_EXCL="*"_, then execute your command.

#### Current version

0.17.0 -- Fixes for all tools (piped data structures), but in particular trparse. Add trperf, trpiggy.
### trsem

Read a static semantics spec file and generate code.

#### Usage

    trsem

#### Examples

    trsem

#### Current version

0.17.0 -- Fixes for all tools (piped data structures), but in particular trparse. Add trperf, trpiggy.
### trsort

Reads a parse tree from stdin, move rules according to the named
operation, and writes the modified tree
to stdout. The input and output are Parse Tree Data.

#### Usage

    trsort bfs <string>
    trsort dfs <string>

#### Details

Reorder the parser rules according to the specified type and start rule.
For BFS and DFS, an XPath expression must be supplied to specify all the start
rule symbols. For alphabetic reordering, all parser rules are retained, and
simply reordered alphabetically. For BFS and DFS, if the rule is unreachable
from a start node set that is specified via <string>, then the rule is dropped
from the grammar.

#### Example

    trparse Java.g4 | trsort alpha | trtext
    trparse Java.g4 | trsort dfs ""//parserRuleSpec/RULE_REF[text()='libraryDefinition']"" | trtext

#### Notes

If you are running MSYS2 on Windows, you may notice that XPaths are not being
processed by this command correctly. To avoid the Bash shell from altering
XPaths, type _export MSYS2_ARG_CONV_EXCL="*"_, then execute your command.

#### Current version

0.17.0 -- Fixes for all tools (piped data structures), but in particular trparse. Add trperf, trpiggy.
### Trsplit

The split command splits one grammar into two. The input grammar
must be a combined lexer/parser grammar implemented in one file.
The transformation creates a lexer grammar and a parser grammar,
outputed as parse tree data with the two grammars.
[trsponge](https://github.com/kaby76/Domemtech.Trash/tree/main/trsponge)
is used to instantiate the two files in the file system.

#### Usage

    trsplit

#### Details

The `trsplit` application splits a combined grammar into two files.
It does this as follows:

* Partition the rules in the grammar into parser and lexer rules. This
is done by examining the LHS symbol: parser rules start with a lowercase
LHS symbol name; lexer rules start with an uppercase LHS symbol name.
* In the parser grammar, insert an `optionsSpec` declaration that
contains a `tokenVocab` specification for the name of the vocabulary
generated for the lexer grammar.
* Add `grammarDecl` statements to the top of the new files to declare
the parser and lexer grammars.

After splitting, use `trsponge` to output the files. The resulting files
may require hand-tweaking due to various constraints that split grammars
must follow, including:

* String literals that do not have a corresponding lexer rule must be
modified.
* Parser options do not apply to lexer grammars. Remove or replace.

#### Example

    trparse Arithmetic.g4 | trsplit | trsponge

#### Current version

0.17.0 -- Fixes for all tools (piped data structures), but in particular trparse. Add trperf, trpiggy.
### Trsponge

Read the parse tree data from stdin and write the
results to file(s).

#### Usage

    trsponge <options>

#### Example

    trparse Arithmetic.g4 | trsplit | trsponge

#### Current version

0.17.0 -- Fixes for all tools (piped data structures), but in particular trparse. Add trperf, trpiggy.
### trst

Output tree using the Antlr runtime ToStringTree().

#### Usage

    trst

#### Examples

    trparse A.g4 | trst

#### Current version

0.17.0 -- Fixes for all tools (piped data structures), but in particular trparse. Add trperf, trpiggy.
### trstrip

Read the parse tree data from stdin and strip the grammar
of all comments, labels, and action blocks.

#### Usage

    trstrip

#### Examples

    trparse A.g4 | trstrip

#### Current version

0.17.0 -- Fixes for all tools (piped data structures), but in particular trparse. Add trperf, trpiggy.
### trtext

Reads a tree from stdin and prints the source text. If 'line-number' is
specified, the line number range for the tree is printed.

#### Usage

    trtext line-number?

#### Examples

    trxgrep //lexerRuleSpec | trtext

#### Current version

0.17.0 -- Fixes for all tools (piped data structures), but in particular trparse. Add trperf, trpiggy.
### trtree

Reads a tree from stdin and prints the tree as an indented node list.

#### Usage

    trtree

#### Examples

    trparse A.g4 | trtree

#### Current version

0.17.0 -- Fixes for all tools (piped data structures), but in particular trparse. Add trperf, trpiggy.
### trtokens

The trtokens command reads standard in for a parsing result set and prints out
the tokens for each result. For each tree in a set, the first and last tokens
for the tree are computed and printed, with a blank line separator.

#### Usage

    trtokens

#### Examples

Input:

    Assume the Arithmetic.g4 parser has been built.

Command:

    trparse -i "1 * 2 + 3" | trxgrep " //expression" | trtokens

Output:

    Time to parse: 00:00:00.0778212
    # tokens per sec = 128.49968903075256
    [@4,4:4='2',<2>,1:4]

    [@0,0:0='1',<2>,1:0]

    [@8,8:8='3',<2>,1:8]

    [@0,0:0='1',<2>,1:0]
    [@1,1:1=' ',<15>,channel=1,1:1]
    [@2,2:2='*',<7>,1:2]
    [@3,3:3=' ',<15>,channel=1,1:3]
    [@4,4:4='2',<2>,1:4]

    [@0,0:0='1',<2>,1:0]
    [@1,1:1=' ',<15>,channel=1,1:1]
    [@2,2:2='*',<7>,1:2]
    [@3,3:3=' ',<15>,channel=1,1:3]
    [@4,4:4='2',<2>,1:4]
    [@5,5:5=' ',<15>,channel=1,1:5]
    [@6,6:6='+',<5>,1:6]
    [@7,7:7=' ',<15>,channel=1,1:7]
    [@8,8:8='3',<2>,1:8]

#### Current version

0.17.0 -- Fixes for all tools (piped data structures), but in particular trparse. Add trperf, trpiggy.
### trtree

Reads a tree from stdin and prints the tree as an indented node list.

#### Usage

    trtree

#### Examples

    trparse A.g4 | trtree

#### Current version

0.17.0 -- Fixes for all tools (piped data structures), but in particular trparse. Add trperf, trpiggy.
### Trull

The ulliteral command applies the upper- and lowercase string literal transform
to a collection of terminal nodes in the parse tree, which is identified with the supplied
xpath expression. If the xpath expression is not given, the transform is applied to the
whole file.

#### Usage

    trull <xpath>?

#### Examples

Before:

    grammar KeywordFun;
    a : 'abc';
    b : 'def';
    A : 'abc';
    B : 'def';
    C : 'uvw' 'xyz'?;
    D : 'uvw' 'xyz'+;

Command:

    trparse KeywordFun.g4 | trull "//lexerRuleSpec[TOKEN_REF/text() = 'A']//STRING_LITERAL" | trprint

After:

    grammar KeywordFun;
    a : 'abc';
    b : 'def';
    A :  [aA] [bB] [cC];
    B : 'def';
    C : 'uvw' 'xyz'?;
    D : 'uvw' 'xyz'+;

Command:

    trparse KeywordFun.g4 | trull | trprint

After:

    grammar KeywordFun;
    a : 'abc';
    b : 'def';
    A :  [aA] [bB] [cC];
    B :  [dD] [eE] [fF];
    C :  [uU] [vV] [wW] ( [xX] [yY] [zZ] )?;
    D :  [uU] [vV] [wW] ( [xX] [yY] [zZ] )+;

#### Notes

If you are running MSYS2 on Windows, you may notice that XPaths are not being
processed by this command correctly. To avoid the Bash shell from altering
XPaths, type _export MSYS2_ARG_CONV_EXCL="*"_, then execute your command.

#### Current version

0.17.0 -- Fixes for all tools (piped data structures), but in particular trparse. Add trperf, trpiggy.
### Trunfold

The unfold command applies the unfold transform to a collection of terminal nodes
in the parse tree, which is identified with the supplied xpath expression. Prior
to using this command, you must have the file parsed. An unfold operation substitutes
the right-hand side of a parser or lexer rule into a reference of the rule name that
occurs at the specified node.

#### Usage

    trunfold <string>

#### Examples

Before:

	grammar Expresion;
	s : e ;
	e : e '*' e       # Mult
	    | INT           # primary
	    ;
	INT : [0-9]+ ;
	WS : [ \t\n]+ -> skip ;

Command:

    trparse Expression.g4 | trunfold "//parserRuleSpec[RULE_REF/text() = 's']//labeledAlt//RULE_REF[text() = 'e']" | trsponge -c

After:

	grammar Expression;
	s : ( e '*' e | INT ) ;
	e : e '*' e           # Mult
		| INT               # primary
		;
	INT : [0-9]+ ;
	WS : [ \t\n]+ -> skip ;


#### Notes

If you are running MSYS2 on Windows, you may notice that XPaths are not being
processed by this command correctly. To avoid the Bash shell from altering
XPaths, type _export MSYS2_ARG_CONV_EXCL="*"_, then execute your command.

#### Current version

0.17.0 -- Fixes for all tools (piped data structures), but in particular trparse. Add trperf, trpiggy.

### trungroup

Perform an ungroup transformation of the 'element' node(s) specified by the string.

#### Usage

    trungroup <string>

#### Examples

    trparse A.g4 | trungroup "//parserRuleSpec[RULE_REF/text() = 'a']//ruleAltList"

#### Notes

If you are running MSYS2 on Windows, you may notice that XPaths are not being
processed by this command correctly. To avoid the Bash shell from altering
XPaths, type _export MSYS2_ARG_CONV_EXCL="*"_, then execute your command.

#### Current version

0.17.0 -- Fixes for all tools (piped data structures), but in particular trparse. Add trperf, trpiggy.
### trwdog

Execute a command with a watchdog timer.

#### Usage

    trwdog <arg>+

#### Examples

    trwdog make test

#### Current version

0.17.0 -- Fixes for all tools (piped data structures), but in particular trparse. Add trperf, trpiggy.
### trxgrep

Find all sub-trees in a parse tree using the given XPath expression.

#### Usage

    trxgrep <string>

#### Examples

    trparse A.g4 | trxgrep "//parserRuleSpec[RULE_REF/text() = 'normalAnnotation']"

#### Notes

If you are running MSYS2 on Windows, you may notice that XPaths are not being
processed by this command correctly. To avoid the Bash shell from altering
XPaths, type _export MSYS2_ARG_CONV_EXCL="*"_, then execute your command.

#### Current version

0.17.0 -- Fixes for all tools (piped data structures), but in particular trparse. Add trperf, trpiggy.
### trxml

Read a tree from stdin and write an XML represenation of it.

#### Usage

    trxml

#### Examples

    trparse A.g4 | trxml

#### Current version

0.17.0 -- Fixes for all tools (piped data structures), but in particular trparse. Add trperf, trpiggy.
### trxml2

Read an xml file and enumerate all paths to elements in xpath syntax.

#### Usage

    trxml2

#### Examples

    cat pom.xml | trxml2

#### Current version

0.17.0 -- Fixes for all tools (piped data structures), but in particular trparse. Add trperf, trpiggy.
