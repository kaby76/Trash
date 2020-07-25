using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Trash
{
    class Program
    {
        static void Main(string[] args)
        {
            var shell = new Shell();
            shell.Run();
        }
    }

    public class Shell
    {
        private Dictionary<string, Func<int>> Aliases = new Dictionary<string, Func<int>>()
        {
            { "ls", null },
            { "clear", null }
        };

        public void Run()
        {
            string input = null;

            do
            {
                Console.Write("> ");
                input = Console.ReadLine();
                Execute(input);
            } while (input != "exit");
        }

        public int Execute(string input)
        {
            if (Aliases.Keys.Contains(input))
            {

                return 0;
            }

            Console.WriteLine($"{input} not found");
            return 1;
        }
    }
}
