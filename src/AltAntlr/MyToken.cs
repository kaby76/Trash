namespace AltAntlr
{
    using Antlr4.Runtime;

    public class MyToken : IToken
    {
        public string Text { get; set; }
        public int Type { get; set; }
        private int _line;
        public int Line { get { return _line; } set { if (value < 0) throw new System.Exception(); _line = value; } }
        public int Column { get; set; }
        public int Channel { get; set; }
        public int TokenIndex { get; set; }
        private int _startindex;
        public int StartIndex { get { return _startindex; } set { if (value < 0) throw new System.Exception(); _startindex = value; } }
        public int StopIndex { get; set; }
        public ITokenSource TokenSource { get; set; }
        public ICharStream InputStream { get; set; }

        public override string ToString()
        {
            string channelStr = string.Empty;
            if (Channel > 0)
            {
                channelStr = ",channel=" + Channel;
            }
            string txt = Text;
            if (txt != null)
            {
                txt = txt.Replace("\n", "\\n");
                txt = txt.Replace("\r", "\\r");
                txt = txt.Replace("\t", "\\t");
            }
            else
            {
                txt = "<no text>";
            }
            return "[@" + TokenIndex + "," + StartIndex + ":" + StopIndex + "='" + txt + "',<" + Type + ">" + channelStr + "," + Line + ":" + Column + "]";
        }
    }
}
