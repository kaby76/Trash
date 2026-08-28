declare variable $start external;

(: ------------------------------------------------------------------ :)
(: Grammar lookup                                                     :)
(: ------------------------------------------------------------------ :)

declare function local:rule($rules, $name) {
    ($rules[string(RULE_REF[1]) = $name])[1]
};

declare function local:alternatives($container) {
    (
        $container/ruleAltList/labeledAlt/alternative,
        $container/altList/alternative
    )
};

(: ------------------------------------------------------------------ :)
(: Nullability                                                         :)
(: ------------------------------------------------------------------ :)

declare function local:elements-nullable(
    $elements,
    $nullable
) {
    if (empty($elements)) then
        true()
    else if (
        local:element-nullable($elements[1], $nullable)
    ) then
        local:elements-nullable(
            subsequence($elements, 2),
            $nullable
        )
    else
        false()
};

declare function local:alternative-nullable(
    $alternative,
    $nullable
) {
    local:elements-nullable(
        $alternative/element,
        $nullable
    )
};

declare function local:any-alternative-nullable(
    $alternatives,
    $nullable
) {
    if (empty($alternatives)) then
        false()
    else if (
        local:alternative-nullable(
            $alternatives[1],
            $nullable
        )
    ) then
        true()
    else
        local:any-alternative-nullable(
            subsequence($alternatives, 2),
            $nullable
        )
};

declare function local:block-nullable(
    $block,
    $nullable
) {
    local:any-alternative-nullable(
        local:alternatives($block),
        $nullable
    )
};

declare function local:element-nullable(
    $element,
    $nullable
) {
    if (
        exists($element/ebnfSuffix/QUESTION)
        or exists($element/ebnfSuffix/STAR)
    ) then
        true()
    else if (
        exists($element/labeledElement/ebnfSuffix/QUESTION)
        or exists($element/labeledElement/ebnfSuffix/STAR)
    ) then
        true()
    else if (
        exists($element/atom/ruleref/RULE_REF)
    ) then
        string($element/atom/ruleref/RULE_REF) = $nullable
    else if (
        exists($element/labeledElement/atom/ruleref/RULE_REF)
    ) then
        string(
            $element/labeledElement/atom/ruleref/RULE_REF
        ) = $nullable
    else if (
        exists($element/ebnf/block)
    ) then
        local:block-nullable(
            $element/ebnf/block,
            $nullable
        )
    else if (
        exists($element/labeledElement/block)
    ) then
        local:block-nullable(
            $element/labeledElement/block,
            $nullable
        )
    else if (
        exists($element/actionBlock)
        or exists($element/actionBlock/QUESTION)
    ) then
        true()
    else
        false()
};

declare function local:rule-nullable(
    $rule,
    $nullable
) {
    local:any-alternative-nullable(
        local:alternatives($rule/ruleBlock),
        $nullable
    )
};

declare function local:nullable-step(
    $rules,
    $nullable
) {
    distinct-values(
        (
            $nullable,

            for $rule in $rules
            let $name := string($rule/RULE_REF[1])
            where local:rule-nullable($rule, $nullable)
            return $name
        )
    )
};

declare function local:nullable-fixed-point(
    $rules,
    $nullable
) {
    let $next := local:nullable-step($rules, $nullable)
    return
        if (count($next) = count($nullable)) then
            $next
        else
            local:nullable-fixed-point($rules, $next)
};

(: ------------------------------------------------------------------ :)
(: FIRST-set facts                                                     :)
(:                                                                    :)
(: A fact is represented as:                                          :)
(:     rule-name|||terminal                                           :)
(: ------------------------------------------------------------------ :)

declare function local:fact(
    $rule-name,
    $terminal
) {
    concat($rule-name, "|||", $terminal)
};

declare function local:fact-rule($fact) {
    substring-before($fact, "|||")
};

declare function local:fact-terminal($fact) {
    substring-after($fact, "|||")
};

declare function local:first-for-rule(
    $facts,
    $rule-name
) {
    distinct-values(
        for $fact in $facts
        where local:fact-rule($fact) = $rule-name
        return local:fact-terminal($fact)
    )
};

(: ------------------------------------------------------------------ :)
(: Terminal representation                                            :)
(: ------------------------------------------------------------------ :)

declare function local:not-set-symbol($not-set) {
    let $parts :=
        for $node in (
            $not-set/setElement/TOKEN_REF,
            $not-set/setElement/STRING_LITERAL,
            $not-set/blockSet/setElement/TOKEN_REF,
            $not-set/blockSet/setElement/STRING_LITERAL
        )
        return string($node)
    return concat(
        "~(",
        string-join($parts, " | "),
        ")"
    )
};

declare function local:atom-first(
    $atom,
    $facts
) {
    if (exists($atom/terminalDef/TOKEN_REF)) then
        string($atom/terminalDef/TOKEN_REF)
    else if (exists($atom/terminalDef/STRING_LITERAL)) then
        string($atom/terminalDef/STRING_LITERAL)
    else if (exists($atom/ruleref/RULE_REF)) then
        local:first-for-rule(
            $facts,
            string($atom/ruleref/RULE_REF)
        )
    else if (exists($atom/notSet)) then
        local:not-set-symbol($atom/notSet)
    else if (exists($atom/wildcard/DOT)) then
        "."
    else
        ()
};

(: ------------------------------------------------------------------ :)
(: FIRST of blocks, alternatives, and elements                        :)
(: ------------------------------------------------------------------ :)

declare function local:elements-first(
    $elements,
    $nullable,
    $facts
) {
    if (empty($elements)) then
        ()
    else
        let $head := $elements[1]
        let $head-first :=
            local:element-first(
                $head,
                $nullable,
                $facts
            )
        return
            if (
                local:element-nullable($head, $nullable)
            ) then
                distinct-values(
                    (
                        $head-first,
                        local:elements-first(
                            subsequence($elements, 2),
                            $nullable,
                            $facts
                        )
                    )
                )
            else
                $head-first
};

declare function local:alternative-first(
    $alternative,
    $nullable,
    $facts
) {
    local:elements-first(
        $alternative/element,
        $nullable,
        $facts
    )
};

declare function local:block-first(
    $block,
    $nullable,
    $facts
) {
    distinct-values(
        for $alternative in local:alternatives($block)
        return
            local:alternative-first(
                $alternative,
                $nullable,
                $facts
            )
    )
};

declare function local:element-first(
    $element,
    $nullable,
    $facts
) {
    if (exists($element/atom)) then
        local:atom-first(
            $element/atom,
            $facts
        )
    else if (exists($element/labeledElement/atom)) then
        local:atom-first(
            $element/labeledElement/atom,
            $facts
        )
    else if (exists($element/ebnf/block)) then
        local:block-first(
            $element/ebnf/block,
            $nullable,
            $facts
        )
    else if (exists($element/labeledElement/block)) then
        local:block-first(
            $element/labeledElement/block,
            $nullable,
            $facts
        )
    else
        ()
};

declare function local:rule-first(
    $rule,
    $nullable,
    $facts
) {
    distinct-values(
        for $alternative in
            local:alternatives($rule/ruleBlock)
        return
            local:alternative-first(
                $alternative,
                $nullable,
                $facts
            )
    )
};

(: ------------------------------------------------------------------ :)
(: FIRST-set fixed point                                               :)
(: ------------------------------------------------------------------ :)

declare function local:first-step(
    $rules,
    $nullable,
    $facts
) {
    distinct-values(
        (
            $facts,

            for $rule in $rules
            let $name := string($rule/RULE_REF[1])
            for $terminal in
                local:rule-first(
                    $rule,
                    $nullable,
                    $facts
                )
            return local:fact($name, $terminal)
        )
    )
};

declare function local:first-fixed-point(
    $rules,
    $nullable,
    $facts
) {
    let $next :=
        local:first-step(
            $rules,
            $nullable,
            $facts
        )
    return
        if (count($next) = count($facts)) then
            $next
        else
            local:first-fixed-point(
                $rules,
                $nullable,
                $next
            )
};

(: ------------------------------------------------------------------ :)
(: Reachability                                                        :)
(: ------------------------------------------------------------------ :)

declare function local:references($definition) {
    distinct-values(
        for $reference in
            $definition//ruleref/RULE_REF
        return string($reference)
    )
};

declare function local:closure(
    $rules,
    $pending,
    $seen
) {
    if (empty($pending)) then
        $seen
    else
        let $name := string($pending[1])
        let $remaining := subsequence($pending, 2)
        return
            if ($name = $seen) then
                local:closure(
                    $rules,
                    $remaining,
                    $seen
                )
            else
                let $definition :=
                    local:rule($rules, $name)
                let $references :=
                    local:references($definition)
                return
                    local:closure(
                        $rules,
                        ($remaining, $references),
                        ($seen, $name)
                    )
};

(: ------------------------------------------------------------------ :)
(: Main                                                                :)
(: ------------------------------------------------------------------ :)

let $rules := //parserRuleSpec
let $nullable :=
    local:nullable-fixed-point($rules, ())
let $facts :=
    local:first-fixed-point(
        $rules,
        $nullable,
        ()
    )
let $reachable :=
    local:closure($rules, ($start), ())
let $lines :=
    for $name in sort($reachable)
    let $definition := local:rule($rules, $name)
    let $terminals :=
        sort(local:first-for-rule($facts, $name))
    let $epsilon :=
        if ($name = $nullable) then
            "ε"
        else
            ()
    where exists($definition)
    return
        concat(
            $name,
            " -> {",
            string-join(($terminals, $epsilon), ", "),
            "}"
        )
return string-join($lines, "; ")
