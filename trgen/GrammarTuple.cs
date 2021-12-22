namespace Trash
{
    public class GrammarTuple
    {
        public GrammarTuple(string source_grammar_file_name, string target_grammar_file_name, string grammar_name, string generated_file_name, string generated_include_file_name, string grammar_autom_name)
        {
            OriginalSourceFileName = source_grammar_file_name;
            GrammarFileName = target_grammar_file_name;
            GrammarName = grammar_name;
            GeneratedFileName = generated_file_name;
            GeneratedIncludeFileName = generated_include_file_name;
            GrammarAutomName = grammar_autom_name;
        }
        public string OriginalSourceFileName;
        public string GrammarFileName { get; set; }
        public string GrammarName { get; set; }
        public string GeneratedFileName { get; set; }
        public string GeneratedIncludeFileName { get; set; }
        public string GrammarAutomName { get; set; }
    }

}
