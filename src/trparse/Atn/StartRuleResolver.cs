namespace Atn;

public static class StartRuleResolver
{
    public static int Resolve(MyATN parserAtn, ParsedInterp parserInterp,
        string requestedRule)
    {
        if (!string.IsNullOrEmpty(requestedRule))
        {
            for (int rule = 0; rule < parserInterp.RuleNames.Length; rule++)
                if (parserInterp.RuleNames[rule] == requestedRule &&
                    rule < parserAtn.start.Length && parserAtn.start[rule] != null)
                    return rule;

            throw new ArgumentException(
                $"Start rule '{requestedRule}' was not found in the parser .interp file. " +
                "Available rules: " + string.Join(", ", parserInterp.RuleNames));
        }

        if (parserInterp.StartStateNumber < 0) return 0;
        for (int rule = 0; rule < parserAtn.start.Length; rule++)
            if (parserAtn.start[rule]?.stateNumber == parserInterp.StartStateNumber)
                return rule;
        throw new InvalidOperationException(
            $"Start state {parserInterp.StartStateNumber} not found in deserialized parser ATN.");
    }
}
