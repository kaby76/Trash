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

0.23.7 Fixed trcover when started outside of Generated directory. Fixed analysis of grammars in trgen.

## License

The MIT License

Copyright (c) 2024 Ken Domino

Permission is hereby granted, free of charge, 
to any person obtaining a copy of this software and 
associated documentation files (the "Software"), to 
deal in the Software without restriction, including 
without limitation the rights to use, copy, modify, 
merge, publish, distribute, sublicense, and/or sell 
copies of the Software, and to permit persons to whom 
the Software is furnished to do so, 
subject to the following conditions:

The above copyright notice and this permission notice 
shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR 
ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
