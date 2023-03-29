//namespace LspAntlr
//{
//    using System;
//    using Window = System.Windows.Window;

//    public partial class AboutBox : Window
//    {
//        public AboutBox()
//        {
//            InitializeComponent();
//            _info.Text = "Author: Ken Domino";
//            _info.AppendText(Environment.NewLine);
//            _info.AppendText("AntlrVSIX version: " + LspAntlr.Constants.Version);
//            _info.AppendText(Environment.NewLine);
//            _info.AppendText("NET Runtime version: " + typeof(string).Assembly.ImageRuntimeVersion);
//            _info.AppendText(Environment.NewLine);
//            _info.AppendText("Visual Studio version: " + GetVSVersion.GetVisualStudioVersion());
//            _info.AppendText(Environment.NewLine);
//            var version = AntlrLanguageClient.CMVersion();
//            if (version == null) version = "LSP server dependent -- requires a server to be started.";
//            _info.AppendText("Antlr parser version: " + version);
//        }
//    }
//}
