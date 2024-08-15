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

using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using System;
using System.IO;
using System.Text;

namespace Trash;

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
                    try
                    {
                        PdfTextExtractor.GetTextFromPage(pdfPage, histogram);
                    }
                    catch (Exception)
                    {
                    }
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
                var strategy = new ExtractText(config, key, max);
                for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
                {
                    var pdfPage = pdfDoc.GetPage(i);
                    text.Append(PdfTextExtractor.GetTextFromPage(pdfPage, strategy));
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

