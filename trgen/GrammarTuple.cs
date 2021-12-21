namespace Trash
{
    public class GrammarTuple
    {
        public GrammarTuple(string grammar_file_name, string generated_file_name, string generated_include_file_name, string grammar_autom_name)
        {
            GrammarFileName = grammar_file_name;
            GeneratedFileName = generated_file_name;
            GeneratedIncludeFileName = generated_include_file_name;
            GrammarAutomName = grammar_autom_name;
        }
        public string GrammarFileName { get; set; }
        public string GeneratedFileName { get; set; }
        public string GeneratedIncludeFileName { get; set; }
        public string GrammarAutomName { get; set; }
    }

}
