<ruleNames>{
    for $name in //parserRuleSpec/RULE_REF/text()
    return <ruleName>{string($name)}</ruleName>
}</ruleNames>
