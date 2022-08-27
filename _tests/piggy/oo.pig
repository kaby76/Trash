//elementOptions[LT and INT] -> {{ <string-join(for $i in (1 to floor(number(INT/text()))) return preceding-sibling::*/text(), ' ')> }};
