/*
MIT License

Copyright (c) 2023 Ken Domino

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

*/

namespace Trash
{
    using iText.Kernel.Geom;
    using iText.Kernel.Pdf;
    using iText.Kernel.Pdf.Canvas.Parser;
    using iText.Kernel.Pdf.Canvas.Parser.Data;
    using iText.Kernel.Pdf.Canvas.Parser.Listener;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    class Command
    {
        public string Help()
        {
            using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("tritext.readme.md"))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        static int max = 0;
        static int key = 0;

        static string GetTextFromPDF(string src_file_name, Config config)
        {
            // Perform two passes on the PDF file. The first pass is to determine
            // the start of the left-hand side margin. The second pass extracts the
            // text from the PDF file, using the left-hand side margin to make sure
            // we have the appropriate number of spaces at the beginning of each line.

            StringBuilder text = new StringBuilder();
            {
                {
                    var pdfDoc = new PdfDocument(new PdfReader(src_file_name));
                    var histogram = new BuildHistogram(config);
                    for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
                    {
                        var pdfPage = pdfDoc.GetPage(i);
                        PdfTextExtractor.GetTextFromPage(pdfPage, histogram);
                    }
                    foreach (var h in histogram.Histogram)
                    {
                        if (h.Value > max)
                        {
                            max = h.Value;
                            key = h.Key;
                        }
                    }
                }
                {
                    PdfDocument pdfDoc = new PdfDocument(new PdfReader(src_file_name));
                    for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
                    {
                        var pdfPage = pdfDoc.GetPage(i);
                        text.Append(PdfTextExtractor.GetTextFromPage(pdfPage, new MySimpleTextExtractionStrategy(config, key, max)));
                    }
                }
            }
            return text.ToString();
        }


        public void Execute(Config config)
        {
            if (config.Verbose)
            {
                System.Console.Error.WriteLine("reading from file >>>" + String.Join(", ", config.Files) + "<<<");
            }
            foreach (var f in config.Files)
            {
                System.Console.WriteLine(GetTextFromPDF(f, config));
            }
        }
    }

    internal class BuildHistogram : ITextExtractionStrategy
    {
        private Config config;
        public SortedDictionary<int, int> Histogram = new SortedDictionary<int, int>();
        public BuildHistogram(Config c)
        {
            config = c;
        }
        public virtual void EventOccurred(IEventData data, EventType type)
        {
            if (type.Equals(EventType.RENDER_TEXT))
            {
                TextRenderInfo renderInfo = (TextRenderInfo)data;
                LineSegment segment = renderInfo.GetBaseline();
                Vector start = segment.GetStartPoint();
                Vector end = segment.GetEndPoint();
                var l1 = (int)start.Get(0);
                var l2 = (int)end.Get(1);
                //var l3 = (int)end.Get(3);
                var text = renderInfo.GetText();
                //System.Console.WriteLine("l1 " + l1 + " l2 " + l2 + " text " + text);
                Histogram[l1] = Histogram.ContainsKey(l1) ? Histogram[l1] + 1 : 1;
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

    public class MySimpleTextExtractionStrategy : ITextExtractionStrategy
    {
        private Vector lastStart;
        private Vector lastEnd;
        private Config config;
        private int key_;
        private int value_;

        public MySimpleTextExtractionStrategy(Config c, int key, int value)
        {
            config = c;
            key_ = key;
            value_ = value;
        }

        private readonly StringBuilder result = new StringBuilder();
        private static string emphasis = "";

        public virtual void EventOccurred(IEventData data, EventType type)
        {
            if (type.Equals(EventType.RENDER_TEXT))
            {
                TextRenderInfo renderInfo = (TextRenderInfo)data;
                var x = renderInfo.GetText();
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
                else {
                    /* If we are in a bold or italic section, then we need to check
                     * if we are still in that section. If not, end the section and
                     * start a new tag section.
                     */
                    if (italic)
                    {
                        if (emphasis != "</i>")
                        {
                            if (config.OutputMarkup) AppendTextChunk(emphasis);
                            start_tag = true;
                        }
                    }
                    else if (bold)
                    {
                        if (emphasis != "</b>")
                        {
                            if (config.OutputMarkup) AppendTextChunk(emphasis);
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

                bool firstRender = result.Length == 0;
                bool hardReturn = false;
                LineSegment segment = renderInfo.GetBaseline();
                Vector start = segment.GetStartPoint();
                Vector end = segment.GetEndPoint();

                //System.Console.WriteLine("Start: " + start + " End: " + end + " Text: " + renderInfo.GetText() + " Font: " + f.GetFontProgram().GetFontNames().GetFontName());

                if (!firstRender)
                {
                    Vector x1 = lastStart;
                    Vector x2 = lastEnd;
                    // see http://mathworld.wolfram.com/Point-LineDistance2-Dimensional.html
                    float dist = (x2.Subtract(x1)).Cross((x1.Subtract(start))).LengthSquared() / x2.Subtract(x1).LengthSquared
                        ();
                    // we should probably base this on the current font metrics, but 1 pt seems to be sufficient for the time being
                    float sameLineThreshold = 1f;
                    if (dist > sameLineThreshold)
                    {
                        hardReturn = true;
                    }
                }
                // Note:  Technically, we should check both the start and end positions, in case the angle of the text changed without any displacement
                // but this sort of thing probably doesn't happen much in reality, so we'll leave it alone for now
                if (hardReturn)
                {
                    //System.Console.WriteLine("<< Hard Return >>");
                    AppendTextChunk("\n");
                    var s = start;
                    var l1 = (int)start.Get(0);
                    var l2 = (int)end.Get(1);
                    {
                        var spacing = (int)((l1 - key_) / renderInfo.GetSingleSpaceWidth() / 2);
                        if (spacing > 0)
                        {
                            AppendTextChunk(new string(' ', spacing));
                        }
                    }
                    //{
                    //    if (spacing > renderInfo.GetSingleSpaceWidth() / 2f)
                    //    {
                    //        AppendTextChunk(new string(' ', (int)spacing));
                    //    }
                    //}
                }
                else
                {
                    if (!firstRender)
                    {
                        // we only insert a blank space if the trailing character of the previous string wasn't a space, and the leading character of the current string isn't a space
                        if (result[result.Length - 1] != ' ' && renderInfo.GetText().Length > 0 && renderInfo.GetText()[0] != ' ')
                        {
                            float spacing = lastEnd.Subtract(start).Length();
                            if (spacing > renderInfo.GetSingleSpaceWidth() / 2f)
                            {
                                AppendTextChunk(new string(" "));//, (int)spacing));
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
                AppendTextChunk(x);
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
            return result.ToString();
        }

        protected internal void AppendTextChunk(String text)
        {
            result.Append(text);
        }
    }

}
