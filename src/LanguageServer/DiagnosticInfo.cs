namespace LanguageServer
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class DiagnosticInfo
    {
        public enum Severity { Info, Warning, Error }

        private string document;
        private int start;
        private int end;
        private Severity severify;
        private string message;

        public string Document { get => document; set => document = value; }
        public int Start { get => start; set => start = value; }
        public int End { get => end; set => end = value; }
        public Severity Severify { get => severify; set => severify = value; }
        public string Message { get => message; set => message = value; }
    }
}
