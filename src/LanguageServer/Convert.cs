namespace LanguageServer
{
    using System.Collections.Generic;

    public class Convert
    {

        public static Dictionary<string, string> RenameGrammars(List<string> args)
        {
            Dictionary<string, string> results = new Dictionary<string, string>();
            foreach (string f in args)
            {
                if (!System.IO.File.Exists(f)) continue;
                var input = System.IO.File.ReadAllText(f);
                if (f.EndsWith(".y"))
                {
                    var imp = new ConvertBison();
                    results = imp.Try(f, input);
                }
                else if (f.EndsWith(".ebnf"))
                {
                    var imp = new ConvertW3Cebnf();
                    results = imp.Try(f, input);
                }
                else if (f.EndsWith(".g2"))
                {
                    var imp = new ConvertAntlr2();
                    results = imp.Try(f, input);
                }
                else if (f.EndsWith(".g3"))
                {
                    var imp = new ConvertAntlr3();
                    results = imp.Try(f, input);
                }
                else if (f.EndsWith(".g4"))
                {
                    var imp = new ConvertAntlr4();
                    results = imp.Try(f, input);
                }
                else if (f.EndsWith(".lark"))
                {
                    var imp = new ConvertLark();
                    results = imp.Try(f, input);
                }
		else if (f.EndsWith(".pest"))
		{
			var imp = new ConvertPest();
			results = imp.Try(f, input);
		}
            }
            return results;
        }
    }
}
