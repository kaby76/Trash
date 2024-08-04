using Antlr4.Runtime;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using System.Collections.Generic;

namespace Trash;

internal class BuildHistogram : ITextExtractionStrategy
{
    private Config config;
    public SortedDictionary<int, int> Histogram = new SortedDictionary<int, int>();
    private FilterParser.StartContext tree;

    public BuildHistogram(Config c)
    {
        config = c;
        // Parse config.Filter string to get l1 and l2 limitations.
        ICharStream str = new AntlrInputStream(config.Filter);
        FilterLexer lexer = new FilterLexer(str);
        CommonTokenStream tokens = new CommonTokenStream(lexer);
        FilterParser parser = new FilterParser(tokens);
        ErrorListener<int> listener_lexer = new ErrorListener<int>(false, false, System.Console.Error);
        ErrorListener<IToken> listener_parser = new ErrorListener<IToken>(false, false, System.Console.Error);
        lexer.RemoveErrorListeners();
        parser.RemoveErrorListeners();
        lexer.AddErrorListener(listener_lexer);
        parser.AddErrorListener(listener_parser);
        tree = parser.start();
    }

    private static int xxxxxxx = 0;
    public virtual void EventOccurred(IEventData data, EventType type)
    {
        if (type.Equals(EventType.RENDER_TEXT))
        {
            TextRenderInfo renderInfo = (TextRenderInfo)data;
            LineSegment segment = renderInfo.GetBaseline();
            Vector start = segment.GetStartPoint();
            Vector end = segment.GetEndPoint();
            var s0 = (int)start.Get(0);
            var s1 = (int)start.Get(1);
            var e0 = (int)end.Get(0);
            var e1 = (int)end.Get(1);
            FilterEvaluator visitor = new FilterEvaluator(s0, s1, e0, e1);
            var t = tree.Accept(visitor);
            if (t.IsT1)
            {
                if (t.AsT1)
                {
                    return;
                }
            }

            var text = renderInfo.GetText();
            if (config.OutputPreflight) System.Console.WriteLine("s0 " + s0 + " s1 " + s1 + "e0 " + e0 + " e1 " + e1 + " text " + text);
            Histogram[s0] = Histogram.ContainsKey(s0) ? Histogram[s0] + 1 : 1;
        }
    }

    public virtual ICollection<EventType> GetSupportedEvents()
    {
        return null;
    }

    public string GetResultantText()
    {
        return "";
    }
}
