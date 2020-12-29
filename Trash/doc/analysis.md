# Analysis

## Recursion

### Has direct/indirect recursion

Determine whether a rule has left or right, direct or indirect, recursion.

_Grammar_

    grammar Kleene;
    s : a ;
    a : a ';' e | e ;
    b : e ';' b | e ;
    e : e '*' e | INT ;
    INT : [0-9]+ ;
    WS : [ \t\n]+ -> skip ;

_[Trash command](Trash.md#Has)_

    has dr left "//(parserRuleSpec | lexerRuleSpec)/(RULE_REF | TOKEN_REF)"

_Result_

    s False
    a True
    b False
    e True
    INT False
    WS False

_[Trash command](Trash.md#Has)_

    has dr right "//(parserRuleSpec | lexerRuleSpec)/(RULE_REF | TOKEN_REF)"

_Result_

    s False
    a False
    b True
    e True
    INT False
    WS False

