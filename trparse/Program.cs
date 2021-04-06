using System;
using CommandLine;

namespace Trash
{

	public class Program
	{

		public static void Main(string[] args)
		{
			try
			{
				new Program().MainInternal(args);
			}
			catch (Exception e)
			{
				System.Console.Error.WriteLine(e.ToString());
				System.Environment.Exit(1);
			}
		}

		public void MainInternal(string[] args)
		{
			var config = new Config();
			var result = Parser.Default.ParseArguments<Config>(args);
			bool stop = false;
			result.WithNotParsed(
                o =>
                {
                    stop = true;
                });
            if (stop) return;
            result.WithParsed(o =>
            {
                var ty = typeof(Config);
                foreach (var prop in ty.GetProperties())
                {
                    if (prop.GetValue(o, null) != null)
                    {
                        prop.SetValue(config, prop.GetValue(o, null));
                    }
                }

                if (o.Grammars != null) config.Grammars = o.Grammars;
                if (o.Type != null) config.Type = o.Type;
            });
			new CParse().Execute(config);
		}
	}
}
