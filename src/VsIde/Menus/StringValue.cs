namespace LspAntlr
{
    public class StringValue
    {
        public StringValue(string s)
        {
            Ffn = s;
            _bison = System.IO.Path.GetFileName(Ffn);
            if (System.IO.Path.GetFileName(Ffn).EndsWith(".y"))
                _antlr = System.IO.Path.GetFileNameWithoutExtension(Ffn) + ".g4";
            if (System.IO.Path.GetFileName(Ffn).EndsWith(".g3"))
                _antlr = System.IO.Path.GetFileNameWithoutExtension(Ffn) + ".g4";
            if (System.IO.Path.GetFileName(Ffn).EndsWith(".g2"))
                _antlr = System.IO.Path.GetFileNameWithoutExtension(Ffn) + ".g4";
            if (System.IO.Path.GetFileName(Ffn).EndsWith(".g"))
                _antlr = System.IO.Path.GetFileNameWithoutExtension(Ffn) + ".g4";
            if (System.IO.Path.GetFileName(Ffn).EndsWith(".ebnf"))
                _antlr = System.IO.Path.GetFileNameWithoutExtension(Ffn) + ".g4";
        }
        //public string FFN { get { return _ffn; } set { _ffn = value; } }
        public string Original { get => _bison; set => _bison = value; }
        public string Antlr4 { get => _antlr; set => _antlr = value; }
        public string Ffn { get => ffn; set => ffn = value; }

        private string ffn;
        private string _antlr;
        private string _bison;
    }
}
