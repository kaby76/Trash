namespace LspAntlr
{
    using Microsoft.VisualStudio.Text;
    using System.Threading.Tasks;

    public static class Helpers
    {
        public static string GetFFN(this ITextBuffer buffer)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            if (buffer == null)
            {
                return null;
            }

            Microsoft.VisualStudio.Text.Projection.IElisionBuffer projection = buffer as Microsoft.VisualStudio.Text.Projection.IElisionBuffer;
            if (projection != null)
            {
                ITextBuffer source_buffer = projection.SourceBuffer;
                return source_buffer.GetFFN();
            }
            buffer.Properties.TryGetProperty(typeof(Microsoft.VisualStudio.TextManager.Interop.IVsTextBuffer), out Microsoft.VisualStudio.TextManager.Interop.IVsTextBuffer bufferAdapter);
            if (bufferAdapter != null)
            {
                Microsoft.VisualStudio.Shell.Interop.IPersistFileFormat persistFileFormat = bufferAdapter as Microsoft.VisualStudio.Shell.Interop.IPersistFileFormat;
                string ppzsFilename = null;
                if (persistFileFormat != null)
                {
                    persistFileFormat.GetCurFile(out ppzsFilename, out uint iii);
                }

                return ppzsFilename;
            }
            return null;
        }
    }
}