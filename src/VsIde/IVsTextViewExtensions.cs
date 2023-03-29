namespace LspAntlr
{
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.TextManager.Interop;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Runtime.InteropServices;
    using IObjectWithSite = Microsoft.VisualStudio.OLE.Interop.IObjectWithSite;
    using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
    using IServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

    public static class IVsTextViewExtensions
    {
        // Many of these methods are from https://github.com/madskristensen/WebPackTaskRunner/blob/master/src/TaskRunner/VsTextViewUtil.cs
        // and are here because VsShellUtilities.IsDocumentOpen() opens a new view even if there is an existing
        // view. Worse, it creates a whole new buffer from disk, shadowing a view/buffer that may have been modified.
        // Therefore, call ShowFrame() only if there really is no view for the file.
        public static IVsTextView FindTextViewFor(string filePath)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            IVsWindowFrame frame = FindWindowFrame(filePath);
            if (frame != null)
            {

                if (GetTextViewFromFrame(frame, out IVsTextView textView))
                {
                    return textView;
                }
            }
            return null;
        }

        public static IVsCodeWindow GetCodeWindow(this IVsTextView text_view)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            Contract.Requires<ArgumentNullException>(text_view != null, "textView");

            if (!(text_view is IObjectWithSite object_with_site))
            {
                return null;
            }

            Guid riid = typeof(IOleServiceProvider).GUID;
            object_with_site.GetSite(ref riid, out IntPtr ppvSite);
            if (ppvSite == IntPtr.Zero)
            {
                return null;
            }

            IOleServiceProvider ole_service_provider = null;
            try
            {
                ole_service_provider = Marshal.GetObjectForIUnknown(ppvSite) as IOleServiceProvider;
            }
            finally
            {
                Marshal.Release(ppvSite);
            }

            if (ole_service_provider == null)
            {
                return null;
            }

            Guid guid_service = typeof(SVsWindowFrame).GUID;
            riid = typeof(IVsWindowFrame).GUID;
            if (ErrorHandler.Failed(ole_service_provider.QueryService(ref guid_service, ref riid, out IntPtr ppvObject)) || ppvObject == IntPtr.Zero)
            {
                return null;
            }

            IVsWindowFrame frame = null;
            try
            {
                frame = Marshal.GetObjectForIUnknown(ppvObject) as IVsWindowFrame;
            }
            finally
            {
                Marshal.Release(ppvObject);
            }

            riid = typeof(IVsCodeWindow).GUID;
            if (ErrorHandler.Failed(frame.QueryViewInterface(ref riid, out ppvObject)) || ppvObject == IntPtr.Zero)
            {
                return null;
            }

            try
            {
                var code_window = Marshal.GetObjectForIUnknown(ppvObject) as IVsCodeWindow;
                return code_window;
            }
            finally
            {
                Marshal.Release(ppvObject);
            }
        }


        internal static IEnumerable<IVsWindowFrame> EnumerateDocumentWindowFrames()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (Package.GetGlobalService(typeof(SVsUIShell)) is IVsUIShell shell)
            {
                int hr = shell.GetDocumentWindowEnum(out IEnumWindowFrames framesEnum);
                if (hr == VSConstants.S_OK && framesEnum != null)
                {
                    IVsWindowFrame[] frames = new IVsWindowFrame[1];
                    while (framesEnum.Next(1, frames, out uint fetched) == VSConstants.S_OK && fetched == 1)
                    {
                        yield return frames[0];
                    }
                }
            }
        }

        internal static IVsTextView OpenStupidFile(IServiceProvider isp, string full_file_name)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (isp == null) return null;
            ServiceProvider sp = new ServiceProvider(isp);
            if (!VsShellUtilities.IsDocumentOpen(sp, full_file_name, Guid.Empty, out _, out _, out _))
            {
                VsShellUtilities.OpenDocument(sp, full_file_name);
            }
            return FindTextViewFor(full_file_name);
        }

        internal static void ShowFrame(string full_file_name)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            object dte2 = Package.GetGlobalService(typeof(SDTE));
            if (OpenStupidFile(dte2 as Microsoft.VisualStudio.OLE.Interop.IServiceProvider, full_file_name) == null) return;
            ServiceProvider sp = new ServiceProvider(dte2 as Microsoft.VisualStudio.OLE.Interop.IServiceProvider);
            _ = VsShellUtilities.IsDocumentOpen(sp, full_file_name, Guid.Empty, out _, out _, out IVsWindowFrame ivswf);
            ivswf?.Show();
        }

        internal static IVsWindowFrame FindWindowFrame(string filePath)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            foreach (IVsWindowFrame currentFrame in EnumerateDocumentWindowFrames())
            {
                if (IsFrameForFilePath(currentFrame, filePath))
                {
                    return currentFrame;
                }
            }

            return null;
        }

        private static bool GetPhysicalPathFromFrame(IVsWindowFrame frame, out string frameFilePath)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            int hr = frame.GetProperty((int)__VSFPROPID.VSFPROPID_pszMkDocument, out object propertyValue);
            if (hr == VSConstants.S_OK && propertyValue != null)
            {
                frameFilePath = propertyValue.ToString();
                return true;
            }

            frameFilePath = null;
            return false;
        }

        private static bool GetTextViewFromFrame(IVsWindowFrame frame, out IVsTextView textView)
        {
            textView = VsShellUtilities.GetTextView(frame);

            return textView != null;
        }

        private static bool IsFrameForFilePath(IVsWindowFrame frame, string filePath)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (GetPhysicalPathFromFrame(frame, out string frameFilePath))
            {
                return string.Equals(filePath, frameFilePath, StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        //public static SnapshotPoint GetPointInLine(this ITextBuffer textBuffer, int line, int column)
        //{
        //    return textBuffer.CurrentSnapshot.GetPointInLine(line, column);
        //}
        //public static SnapshotPoint GetPointInLine(this ITextSnapshot snapshot, int line, int column)
        //{
        //    ITextSnapshotLine snapshotLine = snapshot.GetLineFromLineNumber(line);
        //    return snapshotLine.Start.Add(column);
        //}

        //public static int GetIndex(this ITextBuffer textBuffer, int line, int column)
        //{
        //    SnapshotPoint point = GetPointInLine(textBuffer, line, column);
        //    int index = point.Position;
        //    return index;
        //}
    }
}