//ruleref[elementOptions[LT and INT]] -> {{ <string-join(for $i in (1 to floor(number(elementOptions/INT/text()))) return RULE_REF/text(), ' ')> }};
