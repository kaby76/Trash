using System.Text;
using System.Text.RegularExpressions;
using XQuery.DataModel;

namespace XQuery.Functions;

/// <summary>
/// Built-in function library for XPath/XQuery 4.0.
/// </summary>
public class FunctionLibrary
{
    private readonly Dictionary<(XdmQName, int), Func<XdmSequence[], EvaluationContext, XdmSequence>> _functions = new();

    public FunctionLibrary()
    {
        RegisterStringFunctions();
        RegisterNumericFunctions();
        RegisterBooleanFunctions();
        RegisterSequenceFunctions();
        RegisterNodeFunctions();
        RegisterMapFunctions();
        RegisterArrayFunctions();
        RegisterDateTimeFunctions();
        RegisterMiscFunctions();
    }

    public XdmSequence Call(XdmQName name, XdmSequence[] args, EvaluationContext context)
    {
        // Try exact match
        if (_functions.TryGetValue((name, args.Length), out var func))
            return func(args, context);

        // Try with fn namespace
        if (!name.HasNamespace)
        {
            var fnName = new XdmQName(XdmQName.FnNamespace, name.LocalName, "fn");
            if (_functions.TryGetValue((fnName, args.Length), out func))
                return func(args, context);
        }

        throw XdmException.UndefinedFunction(name.ToString(), args.Length);
    }

    public XdmFunction? GetFunction(XdmQName name, int arity)
    {
        if (_functions.TryGetValue((name, arity), out var func))
        {
            return new XdmFunction(name, arity, args => func(args, EvaluationContext.CreateDefault()));
        }

        if (!name.HasNamespace)
        {
            var fnName = new XdmQName(XdmQName.FnNamespace, name.LocalName, "fn");
            if (_functions.TryGetValue((fnName, arity), out func))
            {
                return new XdmFunction(fnName, arity, args => func(args, EvaluationContext.CreateDefault()));
            }
        }

        return null;
    }

    private void Register(string localName, int arity, Func<XdmSequence[], EvaluationContext, XdmSequence> implementation)
    {
        var name = new XdmQName(XdmQName.FnNamespace, localName, "fn");
        _functions[(name, arity)] = implementation;
    }

    private void RegisterMap(string localName, int arity, Func<XdmSequence[], EvaluationContext, XdmSequence> implementation)
    {
        var name = new XdmQName(XdmQName.MapNamespace, localName, "map");
        _functions[(name, arity)] = implementation;
    }

    private void RegisterArray(string localName, int arity, Func<XdmSequence[], EvaluationContext, XdmSequence> implementation)
    {
        var name = new XdmQName(XdmQName.ArrayNamespace, localName, "array");
        _functions[(name, arity)] = implementation;
    }

    #region String Functions

    private void RegisterStringFunctions()
    {
        // fn:string
        Register("string", 0, (args, ctx) =>
        {
            if (ctx.ContextItem == null) return new XdmSequence(new XdmAtomicValue(""));
            return new XdmSequence(new XdmAtomicValue(ctx.ContextItem.StringValue));
        });

        Register("string", 1, (args, ctx) =>
        {
            if (args[0].IsEmpty) return new XdmSequence(new XdmAtomicValue(""));
            return new XdmSequence(new XdmAtomicValue(args[0].First!.StringValue));
        });

        // fn:concat
        for (int i = 2; i <= 10; i++)
        {
            int arity = i;
            Register("concat", arity, (args, ctx) =>
            {
                var sb = new StringBuilder();
                foreach (var arg in args)
                {
                    if (!arg.IsEmpty)
                        sb.Append(arg.Atomize().StringValue);
                }
                return new XdmSequence(new XdmAtomicValue(sb.ToString()));
            });
        }

        // fn:string-join
        Register("string-join", 1, (args, ctx) =>
        {
            var strings = args[0].Atomize().Select(i => i.StringValue);
            return new XdmSequence(new XdmAtomicValue(string.Join("", strings)));
        });

        Register("string-join", 2, (args, ctx) =>
        {
            var strings = args[0].Atomize().Select(i => i.StringValue);
            var sep = args[1].IsEmpty ? "" : args[1].Atomize().StringValue;
            return new XdmSequence(new XdmAtomicValue(string.Join(sep, strings)));
        });

        // fn:substring
        Register("substring", 2, (args, ctx) =>
        {
            if (args[0].IsEmpty) return new XdmSequence(new XdmAtomicValue(""));
            var str = args[0].Atomize().StringValue;
            var start = (int)Math.Round((args[1].Single() as XdmAtomicValue)!.AsDouble()) - 1;
            if (start < 0) start = 0;
            if (start >= str.Length) return new XdmSequence(new XdmAtomicValue(""));
            return new XdmSequence(new XdmAtomicValue(str.Substring(start)));
        });

        Register("substring", 3, (args, ctx) =>
        {
            if (args[0].IsEmpty) return new XdmSequence(new XdmAtomicValue(""));
            var str = args[0].Atomize().StringValue;
            var start = Math.Round((args[1].Single() as XdmAtomicValue)!.AsDouble());
            var length = Math.Round((args[2].Single() as XdmAtomicValue)!.AsDouble());

            int startIdx = (int)start - 1;
            int endIdx = (int)(start + length) - 1;

            if (startIdx < 0) startIdx = 0;
            if (endIdx > str.Length) endIdx = str.Length;
            if (startIdx >= str.Length || endIdx <= startIdx)
                return new XdmSequence(new XdmAtomicValue(""));

            return new XdmSequence(new XdmAtomicValue(str.Substring(startIdx, endIdx - startIdx)));
        });

        // fn:string-length
        Register("string-length", 0, (args, ctx) =>
        {
            var str = ctx.ContextItem?.StringValue ?? "";
            return new XdmSequence(new XdmAtomicValue((long)str.Length));
        });

        Register("string-length", 1, (args, ctx) =>
        {
            var str = args[0].IsEmpty ? "" : args[0].Atomize().StringValue;
            return new XdmSequence(new XdmAtomicValue((long)str.Length));
        });

        // fn:normalize-space
        Register("normalize-space", 0, (args, ctx) =>
        {
            var str = ctx.ContextItem?.StringValue ?? "";
            return new XdmSequence(new XdmAtomicValue(NormalizeSpace(str)));
        });

        Register("normalize-space", 1, (args, ctx) =>
        {
            var str = args[0].IsEmpty ? "" : args[0].Atomize().StringValue;
            return new XdmSequence(new XdmAtomicValue(NormalizeSpace(str)));
        });

        // fn:upper-case
        Register("upper-case", 1, (args, ctx) =>
        {
            var str = args[0].IsEmpty ? "" : args[0].Atomize().StringValue;
            return new XdmSequence(new XdmAtomicValue(str.ToUpperInvariant()));
        });

        // fn:lower-case
        Register("lower-case", 1, (args, ctx) =>
        {
            var str = args[0].IsEmpty ? "" : args[0].Atomize().StringValue;
            return new XdmSequence(new XdmAtomicValue(str.ToLowerInvariant()));
        });

        // fn:contains
        Register("contains", 2, (args, ctx) =>
        {
            var str = args[0].IsEmpty ? "" : args[0].Atomize().StringValue;
            var search = args[1].IsEmpty ? "" : args[1].Atomize().StringValue;
            return new XdmSequence(new XdmAtomicValue(str.Contains(search)));
        });

        // fn:starts-with
        Register("starts-with", 2, (args, ctx) =>
        {
            var str = args[0].IsEmpty ? "" : args[0].Atomize().StringValue;
            var prefix = args[1].IsEmpty ? "" : args[1].Atomize().StringValue;
            return new XdmSequence(new XdmAtomicValue(str.StartsWith(prefix)));
        });

        // fn:ends-with
        Register("ends-with", 2, (args, ctx) =>
        {
            var str = args[0].IsEmpty ? "" : args[0].Atomize().StringValue;
            var suffix = args[1].IsEmpty ? "" : args[1].Atomize().StringValue;
            return new XdmSequence(new XdmAtomicValue(str.EndsWith(suffix)));
        });

        // fn:substring-before
        Register("substring-before", 2, (args, ctx) =>
        {
            var str = args[0].IsEmpty ? "" : args[0].Atomize().StringValue;
            var search = args[1].IsEmpty ? "" : args[1].Atomize().StringValue;
            var idx = str.IndexOf(search);
            return new XdmSequence(new XdmAtomicValue(idx < 0 ? "" : str.Substring(0, idx)));
        });

        // fn:substring-after
        Register("substring-after", 2, (args, ctx) =>
        {
            var str = args[0].IsEmpty ? "" : args[0].Atomize().StringValue;
            var search = args[1].IsEmpty ? "" : args[1].Atomize().StringValue;
            var idx = str.IndexOf(search);
            return new XdmSequence(new XdmAtomicValue(idx < 0 ? "" : str.Substring(idx + search.Length)));
        });

        // fn:replace
        Register("replace", 3, (args, ctx) =>
        {
            var input = args[0].IsEmpty ? "" : args[0].Atomize().StringValue;
            var pattern = args[1].Atomize().StringValue;
            var replacement = args[2].IsEmpty ? "" : args[2].Atomize().StringValue;

            try
            {
                var result = Regex.Replace(input, pattern, replacement);
                return new XdmSequence(new XdmAtomicValue(result));
            }
            catch (ArgumentException ex)
            {
                throw XdmException.InvalidRegex(ex.Message);
            }
        });

        // fn:matches
        Register("matches", 2, (args, ctx) =>
        {
            var input = args[0].IsEmpty ? "" : args[0].Atomize().StringValue;
            var pattern = args[1].Atomize().StringValue;

            try
            {
                var matches = Regex.IsMatch(input, pattern);
                return new XdmSequence(new XdmAtomicValue(matches));
            }
            catch (ArgumentException ex)
            {
                throw XdmException.InvalidRegex(ex.Message);
            }
        });

        // fn:tokenize
        Register("tokenize", 1, (args, ctx) =>
        {
            var input = args[0].IsEmpty ? "" : args[0].Atomize().StringValue;
            var tokens = input.Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            return new XdmSequence(tokens.Select(t => new XdmAtomicValue(t)).Cast<XdmItem>());
        });

        Register("tokenize", 2, (args, ctx) =>
        {
            var input = args[0].IsEmpty ? "" : args[0].Atomize().StringValue;
            var pattern = args[1].Atomize().StringValue;

            try
            {
                var tokens = Regex.Split(input, pattern).Where(s => !string.IsNullOrEmpty(s));
                return new XdmSequence(tokens.Select(t => new XdmAtomicValue(t)).Cast<XdmItem>());
            }
            catch (ArgumentException ex)
            {
                throw XdmException.InvalidRegex(ex.Message);
            }
        });

        // fn:translate
        Register("translate", 3, (args, ctx) =>
        {
            var input = args[0].IsEmpty ? "" : args[0].Atomize().StringValue;
            var from = args[1].Atomize().StringValue;
            var to = args[2].IsEmpty ? "" : args[2].Atomize().StringValue;

            var sb = new StringBuilder();
            foreach (var c in input)
            {
                var idx = from.IndexOf(c);
                if (idx < 0)
                    sb.Append(c);
                else if (idx < to.Length)
                    sb.Append(to[idx]);
                // else character is deleted
            }
            return new XdmSequence(new XdmAtomicValue(sb.ToString()));
        });

        // fn:compare
        Register("compare", 2, (args, ctx) =>
        {
            if (args[0].IsEmpty || args[1].IsEmpty)
                return XdmSequence.Empty;

            var s1 = args[0].Atomize().StringValue;
            var s2 = args[1].Atomize().StringValue;
            var cmp = string.Compare(s1, s2, StringComparison.Ordinal);
            return new XdmSequence(new XdmAtomicValue((long)Math.Sign(cmp)));
        });
    }

    private static string NormalizeSpace(string s)
    {
        return Regex.Replace(s.Trim(), @"\s+", " ");
    }

    #endregion

    #region Numeric Functions

    private void RegisterNumericFunctions()
    {
        // fn:number
        Register("number", 0, (args, ctx) =>
        {
            if (ctx.ContextItem == null)
                return new XdmSequence(new XdmAtomicValue(double.NaN));
            try
            {
                var value = double.Parse(ctx.ContextItem.StringValue, System.Globalization.CultureInfo.InvariantCulture);
                return new XdmSequence(new XdmAtomicValue(value));
            }
            catch
            {
                return new XdmSequence(new XdmAtomicValue(double.NaN));
            }
        });

        Register("number", 1, (args, ctx) =>
        {
            if (args[0].IsEmpty)
                return new XdmSequence(new XdmAtomicValue(double.NaN));
            try
            {
                var atomized = args[0].Atomize().Single();
                if (atomized is XdmAtomicValue av && av.IsNumeric)
                    return new XdmSequence(new XdmAtomicValue(av.AsDouble()));

                var value = double.Parse(atomized.StringValue, System.Globalization.CultureInfo.InvariantCulture);
                return new XdmSequence(new XdmAtomicValue(value));
            }
            catch
            {
                return new XdmSequence(new XdmAtomicValue(double.NaN));
            }
        });

        // fn:abs
        Register("abs", 1, (args, ctx) =>
        {
            if (args[0].IsEmpty) return XdmSequence.Empty;
            var av = args[0].Single() as XdmAtomicValue ?? throw XdmException.TypeError("Expected numeric value");
            if (av.IsInteger)
                return new XdmSequence(new XdmAtomicValue(Math.Abs(av.AsInteger())));
            if (av.IsDecimal)
                return new XdmSequence(new XdmAtomicValue(Math.Abs(av.AsDecimal())));
            return new XdmSequence(new XdmAtomicValue(Math.Abs(av.AsDouble())));
        });

        // fn:ceiling
        Register("ceiling", 1, (args, ctx) =>
        {
            if (args[0].IsEmpty) return XdmSequence.Empty;
            var av = args[0].Single() as XdmAtomicValue ?? throw XdmException.TypeError("Expected numeric value");
            return new XdmSequence(new XdmAtomicValue(Math.Ceiling(av.AsDouble())));
        });

        // fn:floor
        Register("floor", 1, (args, ctx) =>
        {
            if (args[0].IsEmpty) return XdmSequence.Empty;
            var av = args[0].Single() as XdmAtomicValue ?? throw XdmException.TypeError("Expected numeric value");
            return new XdmSequence(new XdmAtomicValue(Math.Floor(av.AsDouble())));
        });

        // fn:round
        Register("round", 1, (args, ctx) =>
        {
            if (args[0].IsEmpty) return XdmSequence.Empty;
            var av = args[0].Single() as XdmAtomicValue ?? throw XdmException.TypeError("Expected numeric value");
            return new XdmSequence(new XdmAtomicValue(Math.Round(av.AsDouble(), MidpointRounding.AwayFromZero)));
        });

        // fn:round-half-to-even
        Register("round-half-to-even", 1, (args, ctx) =>
        {
            if (args[0].IsEmpty) return XdmSequence.Empty;
            var av = args[0].Single() as XdmAtomicValue ?? throw XdmException.TypeError("Expected numeric value");
            return new XdmSequence(new XdmAtomicValue(Math.Round(av.AsDouble(), MidpointRounding.ToEven)));
        });

        Register("round-half-to-even", 2, (args, ctx) =>
        {
            if (args[0].IsEmpty) return XdmSequence.Empty;
            var av = args[0].Single() as XdmAtomicValue ?? throw XdmException.TypeError("Expected numeric value");
            var precision = (int)(args[1].Single() as XdmAtomicValue)!.AsInteger();
            return new XdmSequence(new XdmAtomicValue(Math.Round(av.AsDouble(), precision, MidpointRounding.ToEven)));
        });

        // fn:sum
        Register("sum", 1, (args, ctx) =>
        {
            if (args[0].IsEmpty) return new XdmSequence(new XdmAtomicValue(0L));
            var atomized = args[0].Atomize();
            double sum = 0;
            foreach (var item in atomized)
            {
                if (item is XdmAtomicValue av)
                    sum += av.AsDouble();
            }
            return new XdmSequence(new XdmAtomicValue(sum));
        });

        Register("sum", 2, (args, ctx) =>
        {
            if (args[0].IsEmpty) return args[1];
            var atomized = args[0].Atomize();
            double sum = 0;
            foreach (var item in atomized)
            {
                if (item is XdmAtomicValue av)
                    sum += av.AsDouble();
            }
            return new XdmSequence(new XdmAtomicValue(sum));
        });

        // fn:avg
        Register("avg", 1, (args, ctx) =>
        {
            if (args[0].IsEmpty) return XdmSequence.Empty;
            var atomized = args[0].Atomize();
            double sum = 0;
            int count = 0;
            foreach (var item in atomized)
            {
                if (item is XdmAtomicValue av)
                {
                    sum += av.AsDouble();
                    count++;
                }
            }
            return new XdmSequence(new XdmAtomicValue(sum / count));
        });

        // fn:min
        Register("min", 1, (args, ctx) =>
        {
            if (args[0].IsEmpty) return XdmSequence.Empty;
            var atomized = args[0].Atomize();
            XdmAtomicValue? min = null;
            foreach (var item in atomized)
            {
                if (item is XdmAtomicValue av)
                {
                    if (min == null || av.CompareTo(min) < 0)
                        min = av;
                }
            }
            return min != null ? new XdmSequence(min) : XdmSequence.Empty;
        });

        // fn:max
        Register("max", 1, (args, ctx) =>
        {
            if (args[0].IsEmpty) return XdmSequence.Empty;
            var atomized = args[0].Atomize();
            XdmAtomicValue? max = null;
            foreach (var item in atomized)
            {
                if (item is XdmAtomicValue av)
                {
                    if (max == null || av.CompareTo(max) > 0)
                        max = av;
                }
            }
            return max != null ? new XdmSequence(max) : XdmSequence.Empty;
        });
    }

    #endregion

    #region Boolean Functions

    private void RegisterBooleanFunctions()
    {
        // fn:boolean
        Register("boolean", 1, (args, ctx) =>
        {
            return new XdmSequence(new XdmAtomicValue(args[0].EffectiveBooleanValue));
        });

        // fn:not
        Register("not", 1, (args, ctx) =>
        {
            return new XdmSequence(new XdmAtomicValue(!args[0].EffectiveBooleanValue));
        });

        // fn:true
        Register("true", 0, (args, ctx) => new XdmSequence(XdmAtomicValue.True));

        // fn:false
        Register("false", 0, (args, ctx) => new XdmSequence(XdmAtomicValue.False));
    }

    #endregion

    #region Sequence Functions

    private void RegisterSequenceFunctions()
    {
        // fn:empty
        Register("empty", 1, (args, ctx) =>
        {
            return new XdmSequence(new XdmAtomicValue(args[0].IsEmpty));
        });

        // fn:exists
        Register("exists", 1, (args, ctx) =>
        {
            return new XdmSequence(new XdmAtomicValue(!args[0].IsEmpty));
        });

        // fn:head
        Register("head", 1, (args, ctx) =>
        {
            var first = args[0].First;
            return first != null ? new XdmSequence(first) : XdmSequence.Empty;
        });

        // fn:tail
        Register("tail", 1, (args, ctx) =>
        {
            return args[0].Subsequence(2);
        });

        // fn:count
        Register("count", 1, (args, ctx) =>
        {
            return new XdmSequence(new XdmAtomicValue((long)args[0].Count));
        });

        // fn:position
        Register("position", 0, (args, ctx) =>
        {
            return new XdmSequence(new XdmAtomicValue((long)ctx.ContextPosition));
        });

        // fn:last
        Register("last", 0, (args, ctx) =>
        {
            return new XdmSequence(new XdmAtomicValue((long)ctx.ContextSize));
        });

        // fn:reverse
        Register("reverse", 1, (args, ctx) =>
        {
            return args[0].Reverse();
        });

        // fn:subsequence
        Register("subsequence", 2, (args, ctx) =>
        {
            var start = (int)Math.Round((args[1].Single() as XdmAtomicValue)!.AsDouble());
            return args[0].Subsequence(start);
        });

        Register("subsequence", 3, (args, ctx) =>
        {
            var start = (int)Math.Round((args[1].Single() as XdmAtomicValue)!.AsDouble());
            var length = (int)Math.Round((args[2].Single() as XdmAtomicValue)!.AsDouble());
            return args[0].Subsequence(start, length);
        });

        // fn:insert-before
        Register("insert-before", 3, (args, ctx) =>
        {
            var pos = (int)(args[1].Single() as XdmAtomicValue)!.AsInteger();
            return args[0].InsertBefore(pos, args[2]);
        });

        // fn:remove
        Register("remove", 2, (args, ctx) =>
        {
            var pos = (int)(args[1].Single() as XdmAtomicValue)!.AsInteger();
            return args[0].Remove(pos);
        });

        // fn:distinct-values
        Register("distinct-values", 1, (args, ctx) =>
        {
            return args[0].Atomize().Distinct();
        });

        // fn:index-of
        Register("index-of", 2, (args, ctx) =>
        {
            var search = args[1].Atomize().Single() as XdmAtomicValue;
            var atomized = args[0].Atomize();
            var indices = new List<XdmItem>();
            int pos = 1;
            foreach (var item in atomized)
            {
                if (item is XdmAtomicValue av && av.ValueEquals(search!))
                    indices.Add(new XdmAtomicValue((long)pos));
                pos++;
            }
            return new XdmSequence(indices);
        });

        // fn:sort
        Register("sort", 1, (args, ctx) =>
        {
            var items = args[0].Atomize().ToList();
            items.Sort((a, b) =>
            {
                if (a is XdmAtomicValue av1 && b is XdmAtomicValue av2)
                    return av1.CompareTo(av2);
                return 0;
            });
            return new XdmSequence(items);
        });

        // fn:for-each
        Register("for-each", 2, (args, ctx) =>
        {
            var sequence = args[0];
            var func = args[1].Single() as XdmFunction
                ?? throw XdmException.TypeError("Second argument must be a function");

            var results = new List<XdmItem>();
            foreach (var item in sequence)
            {
                var result = func.Invoke(new XdmSequence(item));
                results.AddRange(result);
            }
            return new XdmSequence(results);
        });

        // fn:filter
        Register("filter", 2, (args, ctx) =>
        {
            var sequence = args[0];
            var predicate = args[1].Single() as XdmFunction
                ?? throw XdmException.TypeError("Second argument must be a function");

            var results = new List<XdmItem>();
            foreach (var item in sequence)
            {
                var result = predicate.Invoke(new XdmSequence(item));
                if (result.EffectiveBooleanValue)
                    results.Add(item);
            }
            return new XdmSequence(results);
        });

        // fn:fold-left
        Register("fold-left", 3, (args, ctx) =>
        {
            var sequence = args[0];
            var zero = args[1];
            var func = args[2].Single() as XdmFunction
                ?? throw XdmException.TypeError("Third argument must be a function");

            var accumulator = zero;
            foreach (var item in sequence)
            {
                accumulator = func.Invoke(accumulator, new XdmSequence(item));
            }
            return accumulator;
        });

        // fn:fold-right
        Register("fold-right", 3, (args, ctx) =>
        {
            var sequence = args[0].Reverse();
            var zero = args[1];
            var func = args[2].Single() as XdmFunction
                ?? throw XdmException.TypeError("Third argument must be a function");

            var accumulator = zero;
            foreach (var item in sequence)
            {
                accumulator = func.Invoke(new XdmSequence(item), accumulator);
            }
            return accumulator;
        });
    }

    #endregion

    #region Node Functions

    private void RegisterNodeFunctions()
    {
        // fn:name
        Register("name", 0, (args, ctx) =>
        {
            if (ctx.ContextItem is XdmNode node && node.NodeName != null)
                return new XdmSequence(new XdmAtomicValue(node.NodeName.PrefixedName));
            return new XdmSequence(new XdmAtomicValue(""));
        });

        Register("name", 1, (args, ctx) =>
        {
            if (args[0].IsEmpty) return new XdmSequence(new XdmAtomicValue(""));
            if (args[0].First is XdmNode node && node.NodeName != null)
                return new XdmSequence(new XdmAtomicValue(node.NodeName.PrefixedName));
            return new XdmSequence(new XdmAtomicValue(""));
        });

        // fn:local-name
        Register("local-name", 0, (args, ctx) =>
        {
            if (ctx.ContextItem is XdmNode node && node.NodeName != null)
                return new XdmSequence(new XdmAtomicValue(node.NodeName.LocalName));
            return new XdmSequence(new XdmAtomicValue(""));
        });

        Register("local-name", 1, (args, ctx) =>
        {
            if (args[0].IsEmpty) return new XdmSequence(new XdmAtomicValue(""));
            if (args[0].First is XdmNode node && node.NodeName != null)
                return new XdmSequence(new XdmAtomicValue(node.NodeName.LocalName));
            return new XdmSequence(new XdmAtomicValue(""));
        });

        // fn:namespace-uri
        Register("namespace-uri", 0, (args, ctx) =>
        {
            if (ctx.ContextItem is XdmNode node && node.NodeName != null)
                return new XdmSequence(new XdmAtomicValue(node.NodeName.NamespaceUri));
            return new XdmSequence(new XdmAtomicValue(""));
        });

        Register("namespace-uri", 1, (args, ctx) =>
        {
            if (args[0].IsEmpty) return new XdmSequence(new XdmAtomicValue(""));
            if (args[0].First is XdmNode node && node.NodeName != null)
                return new XdmSequence(new XdmAtomicValue(node.NodeName.NamespaceUri));
            return new XdmSequence(new XdmAtomicValue(""));
        });

        // fn:root
        Register("root", 0, (args, ctx) =>
        {
            if (ctx.ContextItem is XdmNode node)
                return new XdmSequence(node.Root);
            throw XdmException.TypeError("Context item is not a node");
        });

        Register("root", 1, (args, ctx) =>
        {
            if (args[0].IsEmpty) return XdmSequence.Empty;
            if (args[0].First is XdmNode node)
                return new XdmSequence(node.Root);
            throw XdmException.TypeError("Argument is not a node");
        });

        // fn:path
        Register("path", 0, (args, ctx) =>
        {
            if (ctx.ContextItem is XdmNode node)
                return new XdmSequence(new XdmAtomicValue(GetNodePath(node)));
            return XdmSequence.Empty;
        });

        Register("path", 1, (args, ctx) =>
        {
            if (args[0].IsEmpty) return XdmSequence.Empty;
            if (args[0].First is XdmNode node)
                return new XdmSequence(new XdmAtomicValue(GetNodePath(node)));
            return XdmSequence.Empty;
        });

        // fn:data
        Register("data", 0, (args, ctx) =>
        {
            if (ctx.ContextItem == null) return XdmSequence.Empty;
            return ctx.ContextItem.Atomize();
        });

        Register("data", 1, (args, ctx) =>
        {
            return args[0].Atomize();
        });

        // fn:deep-equal
        Register("deep-equal", 2, (args, ctx) =>
        {
            return new XdmSequence(new XdmAtomicValue(DeepEqual(args[0], args[1])));
        });
    }

    private string GetNodePath(XdmNode node)
    {
        var parts = new List<string>();
        var current = node;

        while (current != null)
        {
            switch (current.NodeKind)
            {
                case XdmNodeKind.Document:
                    parts.Insert(0, "");
                    break;
                case XdmNodeKind.Element:
                    var elem = (XdmElement)current;
                    var siblings = current.Parent?.Children.OfType<XdmElement>()
                        .Where(e => e.NodeName == elem.NodeName).ToList();
                    var pos = siblings?.IndexOf(elem) ?? 0;
                    var posStr = siblings?.Count > 1 ? $"[{pos + 1}]" : "";
                    parts.Insert(0, $"/{elem.NodeName?.ClarkNotation}{posStr}");
                    break;
                case XdmNodeKind.Attribute:
                    var attr = (XdmAttribute)current;
                    parts.Insert(0, $"/@{attr.NodeName?.ClarkNotation}");
                    break;
                case XdmNodeKind.Text:
                    parts.Insert(0, "/text()");
                    break;
                case XdmNodeKind.Comment:
                    parts.Insert(0, "/comment()");
                    break;
                case XdmNodeKind.ProcessingInstruction:
                    var pi = (XdmProcessingInstruction)current;
                    parts.Insert(0, $"/processing-instruction({pi.Target})");
                    break;
            }
            current = current.Parent;
        }

        return string.Join("", parts);
    }

    private bool DeepEqual(XdmSequence seq1, XdmSequence seq2)
    {
        if (seq1.Count != seq2.Count) return false;

        for (int i = 0; i < seq1.Count; i++)
        {
            var item1 = seq1[i];
            var item2 = seq2[i];

            if (item1 is XdmNode node1 && item2 is XdmNode node2)
            {
                if (!node1.DeepEquals(node2)) return false;
            }
            else if (item1 is XdmAtomicValue av1 && item2 is XdmAtomicValue av2)
            {
                if (!av1.ValueEquals(av2)) return false;
            }
            else if (item1 is XdmMap map1 && item2 is XdmMap map2)
            {
                if (!map1.DeepEquals(map2)) return false;
            }
            else if (item1 is XdmArray arr1 && item2 is XdmArray arr2)
            {
                if (!arr1.DeepEquals(arr2)) return false;
            }
            else
            {
                return false;
            }
        }

        return true;
    }

    #endregion

    #region Map Functions

    private void RegisterMapFunctions()
    {
        // map:size
        RegisterMap("size", 1, (args, ctx) =>
        {
            var map = args[0].Single() as XdmMap ?? throw XdmException.TypeError("Expected map");
            return new XdmSequence(new XdmAtomicValue((long)map.Count));
        });

        // map:keys
        RegisterMap("keys", 1, (args, ctx) =>
        {
            var map = args[0].Single() as XdmMap ?? throw XdmException.TypeError("Expected map");
            return new XdmSequence(map.Keys.Cast<XdmItem>());
        });

        // map:contains
        RegisterMap("contains", 2, (args, ctx) =>
        {
            var map = args[0].Single() as XdmMap ?? throw XdmException.TypeError("Expected map");
            var key = args[1].Atomize().Single() as XdmAtomicValue ?? throw XdmException.TypeError("Expected atomic key");
            return new XdmSequence(new XdmAtomicValue(map.ContainsKey(key)));
        });

        // map:get
        RegisterMap("get", 2, (args, ctx) =>
        {
            var map = args[0].Single() as XdmMap ?? throw XdmException.TypeError("Expected map");
            var key = args[1].Atomize().Single() as XdmAtomicValue ?? throw XdmException.TypeError("Expected atomic key");
            return map[key];
        });

        // map:put
        RegisterMap("put", 3, (args, ctx) =>
        {
            var map = args[0].Single() as XdmMap ?? throw XdmException.TypeError("Expected map");
            var key = args[1].Atomize().Single() as XdmAtomicValue ?? throw XdmException.TypeError("Expected atomic key");
            return new XdmSequence(map.Put(key, args[2]));
        });

        // map:remove
        RegisterMap("remove", 2, (args, ctx) =>
        {
            var map = args[0].Single() as XdmMap ?? throw XdmException.TypeError("Expected map");
            var keys = args[1].Atomize();
            var result = map;
            foreach (var key in keys)
            {
                if (key is XdmAtomicValue av)
                    result = result.Remove(av);
            }
            return new XdmSequence(result);
        });

        // map:merge
        RegisterMap("merge", 1, (args, ctx) =>
        {
            var result = XdmMap.Empty;
            foreach (var item in args[0])
            {
                if (item is XdmMap map)
                    result = result.Merge(map);
            }
            return new XdmSequence(result);
        });

        // map:entry
        RegisterMap("entry", 2, (args, ctx) =>
        {
            var key = args[0].Atomize().Single() as XdmAtomicValue ?? throw XdmException.TypeError("Expected atomic key");
            return new XdmSequence(XdmMap.Empty.Put(key, args[1]));
        });

        // map:for-each
        RegisterMap("for-each", 2, (args, ctx) =>
        {
            var map = args[0].Single() as XdmMap ?? throw XdmException.TypeError("Expected map");
            var func = args[1].Single() as XdmFunction ?? throw XdmException.TypeError("Expected function");

            var results = new List<XdmItem>();
            foreach (var entry in map)
            {
                var result = func.Invoke(new XdmSequence(entry.Key), entry.Value);
                results.AddRange(result);
            }
            return new XdmSequence(results);
        });
    }

    #endregion

    #region Array Functions

    private void RegisterArrayFunctions()
    {
        // array:size
        RegisterArray("size", 1, (args, ctx) =>
        {
            var array = args[0].Single() as XdmArray ?? throw XdmException.TypeError("Expected array");
            return new XdmSequence(new XdmAtomicValue((long)array.Count));
        });

        // array:get
        RegisterArray("get", 2, (args, ctx) =>
        {
            var array = args[0].Single() as XdmArray ?? throw XdmException.TypeError("Expected array");
            var pos = (int)(args[1].Single() as XdmAtomicValue)!.AsInteger();
            return array.Get(pos);
        });

        // array:put
        RegisterArray("put", 3, (args, ctx) =>
        {
            var array = args[0].Single() as XdmArray ?? throw XdmException.TypeError("Expected array");
            var pos = (int)(args[1].Single() as XdmAtomicValue)!.AsInteger();
            return new XdmSequence(array.Put(pos, args[2]));
        });

        // array:append
        RegisterArray("append", 2, (args, ctx) =>
        {
            var array = args[0].Single() as XdmArray ?? throw XdmException.TypeError("Expected array");
            return new XdmSequence(array.Append(args[1]));
        });

        // array:subarray
        RegisterArray("subarray", 2, (args, ctx) =>
        {
            var array = args[0].Single() as XdmArray ?? throw XdmException.TypeError("Expected array");
            var start = (int)(args[1].Single() as XdmAtomicValue)!.AsInteger();
            return new XdmSequence(array.Subarray(start));
        });

        RegisterArray("subarray", 3, (args, ctx) =>
        {
            var array = args[0].Single() as XdmArray ?? throw XdmException.TypeError("Expected array");
            var start = (int)(args[1].Single() as XdmAtomicValue)!.AsInteger();
            var length = (int)(args[2].Single() as XdmAtomicValue)!.AsInteger();
            return new XdmSequence(array.Subarray(start, length));
        });

        // array:remove
        RegisterArray("remove", 2, (args, ctx) =>
        {
            var array = args[0].Single() as XdmArray ?? throw XdmException.TypeError("Expected array");
            var positions = args[1].Atomize().Select(i => (int)(i as XdmAtomicValue)!.AsInteger()).OrderByDescending(p => p);
            var result = array;
            foreach (var pos in positions)
            {
                result = result.Remove(pos);
            }
            return new XdmSequence(result);
        });

        // array:insert-before
        RegisterArray("insert-before", 3, (args, ctx) =>
        {
            var array = args[0].Single() as XdmArray ?? throw XdmException.TypeError("Expected array");
            var pos = (int)(args[1].Single() as XdmAtomicValue)!.AsInteger();
            return new XdmSequence(array.InsertBefore(pos, args[2]));
        });

        // array:head
        RegisterArray("head", 1, (args, ctx) =>
        {
            var array = args[0].Single() as XdmArray ?? throw XdmException.TypeError("Expected array");
            return array.Head();
        });

        // array:tail
        RegisterArray("tail", 1, (args, ctx) =>
        {
            var array = args[0].Single() as XdmArray ?? throw XdmException.TypeError("Expected array");
            return new XdmSequence(array.Tail());
        });

        // array:reverse
        RegisterArray("reverse", 1, (args, ctx) =>
        {
            var array = args[0].Single() as XdmArray ?? throw XdmException.TypeError("Expected array");
            return new XdmSequence(array.Reverse());
        });

        // array:join
        RegisterArray("join", 1, (args, ctx) =>
        {
            var result = XdmArray.Empty;
            foreach (var item in args[0])
            {
                if (item is XdmArray arr)
                    result = result.Join(arr);
            }
            return new XdmSequence(result);
        });

        // array:flatten
        RegisterArray("flatten", 1, (args, ctx) =>
        {
            return Flatten(args[0]);
        });

        // array:for-each
        RegisterArray("for-each", 2, (args, ctx) =>
        {
            var array = args[0].Single() as XdmArray ?? throw XdmException.TypeError("Expected array");
            var func = args[1].Single() as XdmFunction ?? throw XdmException.TypeError("Expected function");

            var builder = XdmArray.Builder();
            foreach (var member in array)
            {
                var result = func.Invoke(member);
                builder.Add(result);
            }
            return new XdmSequence(builder.Build());
        });

        // array:filter
        RegisterArray("filter", 2, (args, ctx) =>
        {
            var array = args[0].Single() as XdmArray ?? throw XdmException.TypeError("Expected array");
            var predicate = args[1].Single() as XdmFunction ?? throw XdmException.TypeError("Expected function");

            var builder = XdmArray.Builder();
            foreach (var member in array)
            {
                var result = predicate.Invoke(member);
                if (result.EffectiveBooleanValue)
                    builder.Add(member);
            }
            return new XdmSequence(builder.Build());
        });
    }

    private XdmSequence Flatten(XdmSequence seq)
    {
        var results = new List<XdmItem>();
        foreach (var item in seq)
        {
            if (item is XdmArray arr)
            {
                var flattened = Flatten(arr.Flatten());
                results.AddRange(flattened);
            }
            else
            {
                results.Add(item);
            }
        }
        return new XdmSequence(results);
    }

    #endregion

    #region DateTime Functions

    private void RegisterDateTimeFunctions()
    {
        // fn:current-dateTime
        Register("current-dateTime", 0, (args, ctx) =>
        {
            return new XdmSequence(new XdmAtomicValue(DateTime.Now));
        });

        // fn:current-date
        Register("current-date", 0, (args, ctx) =>
        {
            return new XdmSequence(new XdmAtomicValue(DateOnly.FromDateTime(DateTime.Now)));
        });

        // fn:current-time
        Register("current-time", 0, (args, ctx) =>
        {
            return new XdmSequence(new XdmAtomicValue(TimeOnly.FromDateTime(DateTime.Now)));
        });

        // fn:year-from-dateTime
        Register("year-from-dateTime", 1, (args, ctx) =>
        {
            if (args[0].IsEmpty) return XdmSequence.Empty;
            var dt = (args[0].Single() as XdmAtomicValue)!.AsDateTime();
            return new XdmSequence(new XdmAtomicValue((long)dt.Year));
        });

        // fn:month-from-dateTime
        Register("month-from-dateTime", 1, (args, ctx) =>
        {
            if (args[0].IsEmpty) return XdmSequence.Empty;
            var dt = (args[0].Single() as XdmAtomicValue)!.AsDateTime();
            return new XdmSequence(new XdmAtomicValue((long)dt.Month));
        });

        // fn:day-from-dateTime
        Register("day-from-dateTime", 1, (args, ctx) =>
        {
            if (args[0].IsEmpty) return XdmSequence.Empty;
            var dt = (args[0].Single() as XdmAtomicValue)!.AsDateTime();
            return new XdmSequence(new XdmAtomicValue((long)dt.Day));
        });

        // fn:hours-from-dateTime
        Register("hours-from-dateTime", 1, (args, ctx) =>
        {
            if (args[0].IsEmpty) return XdmSequence.Empty;
            var dt = (args[0].Single() as XdmAtomicValue)!.AsDateTime();
            return new XdmSequence(new XdmAtomicValue((long)dt.Hour));
        });

        // fn:minutes-from-dateTime
        Register("minutes-from-dateTime", 1, (args, ctx) =>
        {
            if (args[0].IsEmpty) return XdmSequence.Empty;
            var dt = (args[0].Single() as XdmAtomicValue)!.AsDateTime();
            return new XdmSequence(new XdmAtomicValue((long)dt.Minute));
        });

        // fn:seconds-from-dateTime
        Register("seconds-from-dateTime", 1, (args, ctx) =>
        {
            if (args[0].IsEmpty) return XdmSequence.Empty;
            var dt = (args[0].Single() as XdmAtomicValue)!.AsDateTime();
            return new XdmSequence(new XdmAtomicValue((decimal)dt.Second + (decimal)dt.Millisecond / 1000));
        });
    }

    #endregion

    #region Miscellaneous Functions

    private void RegisterMiscFunctions()
    {
        // fn:error
        Register("error", 0, (args, ctx) =>
        {
            throw new XdmException("FOER0000", "Error raised by fn:error()");
        });

        Register("error", 1, (args, ctx) =>
        {
            var code = args[0].IsEmpty ? "FOER0000" : (args[0].Single() as XdmAtomicValue)!.AsQName().LocalName;
            throw new XdmException(code, $"Error raised by fn:error()");
        });

        Register("error", 2, (args, ctx) =>
        {
            var code = args[0].IsEmpty ? "FOER0000" : (args[0].Single() as XdmAtomicValue)!.AsQName().LocalName;
            var message = args[1].IsEmpty ? "" : args[1].Atomize().StringValue;
            throw new XdmException(code, message);
        });

        // fn:trace
        Register("trace", 1, (args, ctx) =>
        {
            Console.WriteLine($"trace: {args[0]}");
            return args[0];
        });

        Register("trace", 2, (args, ctx) =>
        {
            var label = args[1].IsEmpty ? "" : args[1].Atomize().StringValue;
            Console.WriteLine($"{label}: {args[0]}");
            return args[0];
        });

        // fn:generate-id
        Register("generate-id", 0, (args, ctx) =>
        {
            if (ctx.ContextItem is XdmNode node)
                return new XdmSequence(new XdmAtomicValue($"N{node.NodeId}"));
            return new XdmSequence(new XdmAtomicValue(""));
        });

        Register("generate-id", 1, (args, ctx) =>
        {
            if (args[0].IsEmpty)
                return new XdmSequence(new XdmAtomicValue(""));
            if (args[0].First is XdmNode node)
                return new XdmSequence(new XdmAtomicValue($"N{node.NodeId}"));
            return new XdmSequence(new XdmAtomicValue(""));
        });

        // fn:format-number
        Register("format-number", 2, (args, ctx) =>
        {
            var value = (args[0].Single() as XdmAtomicValue)!.AsDouble();
            var pattern = args[1].Atomize().StringValue;
            // Simplified implementation
            return new XdmSequence(new XdmAtomicValue(value.ToString(pattern, System.Globalization.CultureInfo.InvariantCulture)));
        });

        // fn:parse-json
        Register("parse-json", 1, (args, ctx) =>
        {
            var json = args[0].Atomize().StringValue;
            // This would need proper JSON parsing - simplified for now
            return new XdmSequence(new XdmAtomicValue(json));
        });

        // fn:serialize
        Register("serialize", 1, (args, ctx) =>
        {
            var sb = new StringBuilder();
            SerializeSequence(args[0], sb);
            return new XdmSequence(new XdmAtomicValue(sb.ToString()));
        });

        // fn:environment-variable
        Register("environment-variable", 1, (args, ctx) =>
        {
            var name = args[0].Atomize().StringValue;
            var value = Environment.GetEnvironmentVariable(name);
            return value != null ? new XdmSequence(new XdmAtomicValue(value)) : XdmSequence.Empty;
        });

        // fn:available-environment-variables
        Register("available-environment-variables", 0, (args, ctx) =>
        {
            var vars = Environment.GetEnvironmentVariables();
            var names = vars.Keys.Cast<string>().Select(k => new XdmAtomicValue(k)).Cast<XdmItem>();
            return new XdmSequence(names);
        });
    }

    private void SerializeSequence(XdmSequence seq, StringBuilder sb)
    {
        foreach (var item in seq)
        {
            if (item is XdmNode node)
                SerializeNode(node, sb);
            else
                sb.Append(item.StringValue);
        }
    }

    private void SerializeNode(XdmNode node, StringBuilder sb)
    {
        switch (node.NodeKind)
        {
            case XdmNodeKind.Document:
                foreach (var child in node.Children)
                    SerializeNode(child, sb);
                break;

            case XdmNodeKind.Element:
                var elem = (XdmElement)node;
                sb.Append('<');
                sb.Append(elem.NodeName!.PrefixedName);
                foreach (var attr in elem.Attributes)
                {
                    sb.Append(' ');
                    sb.Append(attr.NodeName!.PrefixedName);
                    sb.Append("=\"");
                    sb.Append(EscapeXml(attr.Value));
                    sb.Append('"');
                }
                if (elem.Children.Count == 0)
                {
                    sb.Append("/>");
                }
                else
                {
                    sb.Append('>');
                    foreach (var child in elem.Children)
                        SerializeNode(child, sb);
                    sb.Append("</");
                    sb.Append(elem.NodeName.PrefixedName);
                    sb.Append('>');
                }
                break;

            case XdmNodeKind.Text:
                sb.Append(EscapeXml(((XdmText)node).Value));
                break;

            case XdmNodeKind.Comment:
                sb.Append("<!--");
                sb.Append(((XdmComment)node).Value);
                sb.Append("-->");
                break;

            case XdmNodeKind.ProcessingInstruction:
                var pi = (XdmProcessingInstruction)node;
                sb.Append("<?");
                sb.Append(pi.Target);
                if (!string.IsNullOrEmpty(pi.Data))
                {
                    sb.Append(' ');
                    sb.Append(pi.Data);
                }
                sb.Append("?>");
                break;
        }
    }

    private string EscapeXml(string text)
    {
        return text
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("\"", "&quot;");
    }

    #endregion
}
