using System.Collections.Generic;
using System.Linq;

namespace Workspaces
{
    using System.IO;

    public class Util
    {
        private static Dictionary<string, string> cache = new Dictionary<string, string>();

        public static string GetProperDirectoryCapitalization(DirectoryInfo dirInfo)
        {
            DirectoryInfo parentDirInfo = dirInfo.Parent;
            if (null == parentDirInfo)
            {
                return dirInfo.Name.ToUpper();
            }

            return Path.Combine(GetProperDirectoryCapitalization(parentDirInfo),
                                parentDirInfo.GetDirectories(dirInfo.Name)[0].Name);
        }

        public static string GetProperFilePathCapitalization(string filename)
        {
            if (cache.TryGetValue(filename, out string new_filename))
                return new_filename;
            FileInfo fileInfo = new FileInfo(filename);
            if (!fileInfo.Exists)
            {
                cache[filename] = filename;
                return filename;
            }
            DirectoryInfo dirInfo = fileInfo.Directory;
            if (dirInfo.GetFiles(fileInfo.Name).Length == 0)
            {
                cache[filename] = filename;
                return filename;
            }
            string result = Path.Combine(GetProperDirectoryCapitalization(dirInfo),
                            dirInfo.GetFiles(fileInfo.Name)[0].Name);
            cache[filename] = result;
            return result;
        }
    }
}
