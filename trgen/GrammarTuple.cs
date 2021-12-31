namespace Trash
{
    public class GrammarTuple
    {
        public GrammarTuple(string source_grammar_file_name, string target_grammar_file_name, string grammar_name, string generated_file_name, string generated_include_file_name, string grammar_autom_name, string grammar_go_new_name, string antlr_args)
        {
            OriginalSourceFileName = source_grammar_file_name;
            GrammarFileName = target_grammar_file_name;
            GrammarName = grammar_name;
            GeneratedFileName = generated_file_name;
            GeneratedIncludeFileName = generated_include_file_name;
            GrammarAutomName = grammar_autom_name;
            GrammarGoNewName = grammar_go_new_name;
            AntlrArgs = antlr_args;
        }
        public string OriginalSourceFileName;
        public string GrammarFileName { get; set; }
        public string GrammarName { get; set; }
        public string GeneratedFileName { get; set; }
        public string GeneratedIncludeFileName { get; set; }
        public string GrammarAutomName { get; set; }
        public string GrammarGoNewName { get; set; }
        public string AntlrArgs { get; set; }
    }

}
