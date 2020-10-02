using System;
using System.Collections.Generic;
using System.IO;
using Workspaces;

namespace Trash
{
    public class Grun
    {
        public Grun()
        {
        }

        public List<Document> Grammars
        {
            get; set;
        }

        public List<Document> ImportGrammars
        {
            get; set;
        }

        public List<Document> SupportCode
        {
            get; set;
        }

        public void Generate()
        {
            string random_name;
            while (true)
            {
                var dir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                random_name = dir + "antlrvsix" + new Random().Next();
                if (!System.IO.Directory.Exists(random_name)) break;
            }
            Directory.CreateDirectory(random_name);
            foreach (var g in Grammars)
            {

            }
        }
    }
}
