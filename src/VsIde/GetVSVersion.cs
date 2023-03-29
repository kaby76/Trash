namespace LspAntlr
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text.RegularExpressions;

    public static class GetVSVersion
    {
        /// Setup to get the VS version number including the correct minor version, 
        /// since the DTE.Version does not seem to accurately reflect that. 
        public static string GetVisualStudioVersion()
        {
            FileVersionInfo versionInfo;
            try
            {
                string msenvPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "msenv.dll");
                versionInfo = FileVersionInfo.GetVersionInfo(msenvPath);
            }
            catch (FileNotFoundException)
            { return null; }

            // Extract the version number from the string in the format "D16.2", "D16.3", etc.
            Match version = Regex.Match(versionInfo.FileVersion, @"D([\d\.]+)");
            return version.Success ? version.Groups[1].Value : null;
        }
    }
}
