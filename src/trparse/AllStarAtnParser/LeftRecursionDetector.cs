namespace AllStarAtnParser;

using Atn;

/// <summary>Finds cycles in the nullable left-corner relation of a parser ATN.</summary>
internal static class LeftRecursionDetector
{
    public static bool TryFindCycle(MyATN atn, out IReadOnlyList<int> cycle)
    {
        var nullable = ComputeNullableRules(atn);
        var corners = new HashSet<int>[atn.start.Length];
        for (int rule = 0; rule < corners.Length; rule++)
            corners[rule] = ComputeLeftCorners(atn, rule, nullable);

        var colors = new byte[corners.Length];
        var stack = new List<int>();
        for (int rule = 0; rule < corners.Length; rule++)
            if (colors[rule] == 0 &&
                FindCycle(rule, corners, colors, stack, out cycle))
                return true;

        cycle = Array.Empty<int>();
        return false;
    }

    private static bool[] ComputeNullableRules(MyATN atn)
    {
        var nullable = new bool[atn.start.Length];
        bool changed;
        do
        {
            changed = false;
            for (int rule = 0; rule < nullable.Length; rule++)
            {
                if (nullable[rule] || !CanReachStop(atn, rule, nullable))
                    continue;
                nullable[rule] = true;
                changed = true;
            }
        } while (changed);
        return nullable;
    }

    private static bool CanReachStop(MyATN atn, int rule, bool[] nullable)
    {
        var work = new Stack<MyATNState>();
        var visited = new HashSet<int>();
        work.Push(atn.start[rule]);
        while (work.Count > 0)
        {
            var state = work.Pop();
            if (!visited.Add(state.stateNumber)) continue;
            if (atn.stop.Contains(state) && state.ruleIndex == rule)
                return true;
            foreach (var transition in state.transitions)
            {
                if (transition is MyRuleTransition call)
                {
                    if (nullable[call.ruleIndex]) work.Push(call.target);
                }
                else if (!IsTerminal(transition))
                {
                    work.Push(transition.target);
                }
            }
        }
        return false;
    }

    private static HashSet<int> ComputeLeftCorners(
        MyATN atn, int rule, bool[] nullable)
    {
        var result = new HashSet<int>();
        var work = new Stack<MyATNState>();
        var visited = new HashSet<int>();
        work.Push(atn.start[rule]);
        while (work.Count > 0)
        {
            var state = work.Pop();
            if (!visited.Add(state.stateNumber)) continue;
            foreach (var transition in state.transitions)
            {
                if (transition is MyRuleTransition call)
                {
                    result.Add(call.ruleIndex);
                    if (nullable[call.ruleIndex]) work.Push(call.target);
                }
                else if (!IsTerminal(transition))
                {
                    work.Push(transition.target);
                }
            }
        }
        return result;
    }

    private static bool FindCycle(int rule, HashSet<int>[] corners,
        byte[] colors, List<int> stack, out IReadOnlyList<int> cycle)
    {
        colors[rule] = 1;
        stack.Add(rule);
        foreach (int target in corners[rule])
        {
            if (colors[target] == 0)
            {
                if (FindCycle(target, corners, colors, stack, out cycle))
                    return true;
            }
            else if (colors[target] == 1)
            {
                int first = stack.IndexOf(target);
                var found = stack.Skip(first).ToList();
                found.Add(target);
                cycle = found;
                return true;
            }
        }
        stack.RemoveAt(stack.Count - 1);
        colors[rule] = 2;
        cycle = Array.Empty<int>();
        return false;
    }

    private static bool IsTerminal(MyTransition transition) =>
        transition is MyAtomTransition or MyRangeTransition or
            MySetTransition or MyNotSetTransition or MyWildcardTransition;
}
