namespace AllStarAtnParser;

using System.Globalization;
using System.Text;
using XQuery.DataModel;
using XQuery.Engine;
using XQuery.Parser;

/// <summary>
/// Applies a pure, per-input XQuery4 edit sequence before the lexer sees the
/// source. Edits use zero-based UTF-16 offsets in the original input.
/// </summary>
public sealed class SourceTransformHook
{
    private const string ContextNamespace = "urn:trash:context";

    public string Text { get; }
    public int[] OriginalOffsets { get; }

    private SourceTransformHook(string text, int[] originalOffsets)
    {
        Text = text;
        OriginalOffsets = originalOffsets;
    }

    public int OriginalOffset(int transformedOffset) =>
        OriginalOffsets[Math.Clamp(transformedOffset, 0, Text.Length)];

    public static SourceTransformHook Apply(string input, string queryPath)
    {
        var parser = new XQueryParser(File.ReadAllText(queryPath));
        parser.AddNamespace("ctx", ContextNamespace);
        var expression = parser.Parse();
        var context = EvaluationContext.CreateDefault();
        context.SetVariable("input", new XdmSequence(new XdmAtomicValue(input)));
        var name = new XdmQName(ContextNamespace, "java-unicode-edits", "ctx");
        context.RegisterFunction(name, 1, new XdmFunction(name, 1, args =>
            new XdmSequence(JavaUnicodeEdits(args[0].Single().StringValue))));

        var edits = new List<Edit>();
        foreach (var item in new XQueryEvaluator(context).Evaluate(expression))
        {
            if (item is not XdmElement { LocalName: "edit", NamespaceUri: "" } element)
                throw new InvalidDataException(
                    "A source hook must return a sequence of unqualified edit elements.");
            if (!int.TryParse(element.GetAttribute("start"),
                    NumberStyles.None, CultureInfo.InvariantCulture, out int start) ||
                !int.TryParse(element.GetAttribute("length"),
                    NumberStyles.None, CultureInfo.InvariantCulture, out int length) ||
                length <= 0)
                throw new InvalidDataException(
                    "Source-hook edits require nonnegative start and positive length attributes.");
            edits.Add(new Edit(start, length, element.StringValue));
        }

        var output = new StringBuilder(input.Length);
        var offsets = new List<int>(input.Length + 1);
        int position = 0;
        foreach (var edit in edits)
        {
            if (edit.Start < position || edit.Start > input.Length ||
                edit.Length > input.Length - edit.Start)
                throw new InvalidDataException(
                    "Source-hook edits must be ordered, non-overlapping, and within the input.");
            for (; position < edit.Start; position++)
            {
                offsets.Add(position);
                output.Append(input[position]);
            }
            foreach (char ch in edit.Text)
            {
                offsets.Add(edit.Start);
                output.Append(ch);
            }
            position += edit.Length;
        }
        for (; position < input.Length; position++)
        {
            offsets.Add(position);
            output.Append(input[position]);
        }
        offsets.Add(input.Length);
        return new SourceTransformHook(output.ToString(), offsets.ToArray());
    }

    private static IEnumerable<XdmItem> JavaUnicodeEdits(string input)
    {
        int trailingBackslashes = 0;
        bool previousWasTranslated = false;
        for (int i = 0; i < input.Length;)
        {
            char ch = input[i];
            bool eligible = ch == '\\' &&
                (previousWasTranslated || trailingBackslashes % 2 == 0);
            if (eligible && i + 1 < input.Length && input[i + 1] == 'u')
            {
                int hexStart = i + 1;
                while (hexStart < input.Length && input[hexStart] == 'u')
                    hexStart++;
                if (hexStart + 4 > input.Length ||
                    !int.TryParse(input.AsSpan(hexStart, 4),
                        NumberStyles.HexNumber, CultureInfo.InvariantCulture,
                        out int codePoint))
                    throw new InvalidDataException(
                        $"Invalid Java Unicode escape at input offset {i}.");
                char translated = (char)codePoint;
                var edit = new XdmElement("edit");
                edit.SetAttribute("start", i.ToString(CultureInfo.InvariantCulture));
                edit.SetAttribute("length",
                    (hexStart + 4 - i).ToString(CultureInfo.InvariantCulture));
                edit.AppendChild(new XdmText(translated.ToString()));
                yield return edit;
                i = hexStart + 4;
                trailingBackslashes = translated == '\\'
                    ? trailingBackslashes + 1 : 0;
                previousWasTranslated = true;
                continue;
            }
            trailingBackslashes = ch == '\\' ? trailingBackslashes + 1 : 0;
            previousWasTranslated = false;
            i++;
        }
    }

    private readonly record struct Edit(int Start, int Length, string Text);
}
