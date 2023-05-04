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

        static string GetTextFromPDF(string src_file_name)
        {
            StringBuilder text = new StringBuilder();
            //          string src = @"n4296.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(src_file_name));
            {
                for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
                {
                    var pdfPage = pdfDoc.GetPage(i);
                    text.Append(PdfTextExtractor.GetTextFromPage(pdfPage, new MySimpleTextExtractionStrategy()));
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
                System.Console.WriteLine(GetTextFromPDF(f));
            }
        }
    }

    public class MySimpleTextExtractionStrategy : ITextExtractionStrategy
    {
        private Vector lastStart;

        private Vector lastEnd;

        /// <summary>used to store the resulting String.</summary>
        private readonly StringBuilder result = new StringBuilder();

        public virtual void EventOccurred(IEventData data, EventType type)
        {
            if (type.Equals(EventType.RENDER_TEXT))
            {
                TextRenderInfo renderInfo = (TextRenderInfo)data;
                bool firstRender = result.Length == 0;
                bool hardReturn = false;
                LineSegment segment = renderInfo.GetBaseline();
                Vector start = segment.GetStartPoint();
                Vector end = segment.GetEndPoint();
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
                                AppendTextChunk(" ");
                            }
                        }
                    }
                }
                //System.Console.WriteLine("Inserting implied space before '" + renderInfo.GetText() + "'");
                AppendTextChunk(renderInfo.GetText());
                lastStart = start;
                lastEnd = end;
            }
        }

        public virtual ICollection<EventType> GetSupportedEvents()
        {
            return null;
        }

        /// <summary>Returns the result so far.</summary>
        /// <returns>a String with the resulting text.</returns>
        public virtual String GetResultantText()
        {
            return result.ToString();
        }

        /// <summary>Used to actually append text to the text results.</summary>
        /// <remarks>
        /// Used to actually append text to the text results.  Subclasses can use this to insert
        /// text that wouldn't normally be included in text parsing (e.g. result of OCR performed against
        /// image content)
        /// </remarks>
        /// <param name="text">the text to append to the text results accumulated so far</param>
        protected internal void AppendTextChunk(String text)
        {
            result.Append(text);
        }
    }

}
