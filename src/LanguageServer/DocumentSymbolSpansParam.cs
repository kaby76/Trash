namespace LanguageServer
{
    using LspAntlr;

    public class CMClassifierInformation
    {
        public CMClassifierInformation() { }
        public int start { get; set; }
        public int end { get; set; }
        public int Kind { get; set; }
    }

    public class CMGotoResult
    {
        public CMGotoResult() { }
        public System.Uri TextDocument;
        public int Start;
    }

}
