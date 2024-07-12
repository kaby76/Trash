using Antlr4.Runtime;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using System;
using System.Collections.Generic;
using System.Text;


namespace Trash;

public class ExtractText : ITextExtractionStrategy
{
    private Vector lastStart;
    private Vector lastEnd;
    private Config config;
    private int key_;
    private int value_;
    private FilterParser.StartContext tree;

    public ExtractText(Config c, int key, int value)
    {
        config = c;
        key_ = key;
        value_ = value;
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

    private readonly StringBuilder result = new StringBuilder();
    private static string emphasis = "";

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
            if (lastStart == null)
            {
                lastStart = start;
            }
            if (lastEnd == null)
            {
                lastEnd = end;
            }
            var text = renderInfo.GetText();

            FilterEvaluator visitor = new FilterEvaluator(s0, s1, e0, e1);
            var t = tree.Accept(visitor);
            if (t.IsT1)
            {
                if (t.AsT1)
                {
                    return;
                }
            }

            var f = renderInfo.GetFont();
            var fp = f.GetFontProgram();
            var fn = fp.GetFontNames();
            var italic = false;
            var bold = false;
            if (fn != null)
            {
                var y = fn.GetFontName();
                if (y != null && y.Contains("italic", StringComparison.OrdinalIgnoreCase))
                {
                    italic = true;
                }
                else if (y != null && y.Contains("bold", StringComparison.OrdinalIgnoreCase))
                {
                    bold = true;
                }
            }

            var start_tag = false;
            /* If we are not in a bold or italic section, then a start tag must be added. */
            if (emphasis == "")
            {
                start_tag = true;
            }
            else
            {
                /* If we are in a bold or italic section, then we need to check
                 * if we are still in that section. If not, end the section and
                 * start a new tag section.
                 */
                if (italic)
                {
                    if (emphasis != "</i>")
                    {
                        if (config.OutputMarkup) AppendTextChunk(emphasis);
                        emphasis = "";
                        start_tag = true;
                    }
                }
                else if (bold)
                {
                    if (emphasis != "</b>")
                    {
                        if (config.OutputMarkup) AppendTextChunk(emphasis);
                        emphasis = "";
                        start_tag = true;
                    }
                }
                else
                {
                    /* If we are not in a bold or italic section anymore,
                     * end the section. The text that follows is plain text.
                     */
                    if (config.OutputMarkup) AppendTextChunk(emphasis);
                    emphasis = "";
                    start_tag = false;
                }
            }

            bool hardReturn = false;

            //System.Console.WriteLine("Start: " + start + " End: " + end + " Text: " + renderInfo.GetText() + " Font: " + f.GetFontProgram().GetFontNames().GetFontName());

            // Determine if a new line should be started.
            // We only care about the up/down coordinates.
            // "0", used in start.Get(0), is the "X" coordinate, increases from left to right.
            // "1", used in start.Get(1), is the "Y" coordinate, increases from bottom to top.
            // The page contents starts from the top of the page, those with highest "Y" coordinate.
            // There is a basic assumption that text events will proceed in top to bottom,
            // left to right fashion.
            Vector x1 = lastStart;
            if (x1 == null)
            {
                x1 = start;
            }

            Vector x2 = lastEnd;
            
            // Check diff in Y coordinates.
            float diff = lastStart.Get(1) - start.Get(1);
            var dist = diff * diff;

            // we should probably base this on the current font metrics, but 1 pt seems to be sufficient for the time being
            float sameLineThreshold = 1f;
            if (dist > sameLineThreshold)
            {
                hardReturn = true;
            }

            if (hardReturn)
            {
                //System.Console.WriteLine("<< Hard Return >>");
                AppendTextChunk("\n");
                var s = start;
                {
                    var spacing = (int)((s0 - key_) / renderInfo.GetSingleSpaceWidth() / 2);
                    if (spacing > 0)
                    {
                        AppendTextChunk(new string(' ', spacing));
                    }
                }
            }
            else
            {
                if (result.Length != 0)
                {
                    // we only insert a blank space if the trailing character of the previous string wasn't a space, and the leading character of the current string isn't a space
                    if (result[result.Length - 1] != ' ' && renderInfo.GetText().Length > 0 &&
                        renderInfo.GetText()[0] != ' ')
                    {
                        float spacing = lastEnd.Subtract(start).Length();
                        if (spacing > renderInfo.GetSingleSpaceWidth() / 2f)
                        {
                            if (config.OutputMarkup) AppendTextChunk(emphasis);
                            emphasis = "";
                            start_tag = true;
                            AppendTextChunk(new string(" ")); //, (int)spacing));
                        }
                    }
                }
            }

            //System.Console.WriteLine("Inserting implied space before '" + renderInfo.GetText() + "'");
            if (start_tag)
            {
                if (italic)
                {
                    if (config.OutputMarkup) AppendTextChunk("<i>");
                    emphasis = "</i>";
                }
                else if (bold)
                {
                    if (config.OutputMarkup) AppendTextChunk("<b>");
                    emphasis = "</b>";
                }
            }

            AppendTextChunk(text);
            lastStart = start;
            lastEnd = end;
        }
    }

    public virtual ICollection<EventType> GetSupportedEvents()
    {
        return null;
    }

    public virtual String GetResultantText()
    {
        var str = result.ToString();
        result.Clear();
        return str;
    }

    protected internal void AppendTextChunk(String text)
    {
        result.Append(text);
    }
}
