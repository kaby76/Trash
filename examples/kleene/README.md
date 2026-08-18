# lr-rr-to-kleene

Demonstrates rewriting left-recursive and right-recursive parser rules into
equivalent EBNF using the Kleene star operator, without modifying `trkleene`.

## Grammar

`Kleene.g4` contains two recursive rules:

```antlr
a : a ';' e | e ;   (directly left-recursive)
b : e ';' b | e ;   (directly right-recursive)
```

## Transforms

### kleene-lr.xq

Finds all directly left-recursive rules via XPath:

```xpath
//parserRuleSpec[
    RULE_REF/text() = ruleBlock/ruleAltList/labeledAlt/alternative
                          /*[name()="element"][1]/atom/ruleref/*[1]/text()
]
```

Rewrites `a : a SUFFIX | BASE` as `a : ( BASE ) ( SUFFIX )* `.

### kleene-rr.xq

Finds all directly right-recursive rules by checking the **last** element of
each alternative, then rewrites `b : PREFIX b | BASE` as
`b : ( PREFIX )* ( BASE )`.

## Expected output

```antlr
a : ( e ) ( ';' e )* ;
b : ( e ';' )* ( e ) ;
```

## Running

```sh
bash run-example.sh
```

Or step by step:

```sh
dotnet trash parse Kleene.g4 \
  | dotnet trash xquery kleene-lr.xq \
  | dotnet trash xquery kleene-rr.xq \
  | dotnet trash print \
  | dotnet trash sponge -o Kleene-out.g4
```
