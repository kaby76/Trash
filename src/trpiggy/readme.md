# trpiggy

## Summary

Perform a parse tree rewrite

## Description

Read from stdin a parsing result set, modify the trees using a template engine, then
output the modified parsing result set. The command also reads a template to follow
as the first argument to the command. The template extends the well-known visitor
pattern used by Antlr with a template that contains strings and xpath expressions
that defines children.

## Usage

    trpiggy template-spec

## Examples

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

## Current version

0.20.23 Added trgen StringTemplate meta data file processing. NB: not all Trash tools supported yet.
