namespace LspAntlr
{
    using Microsoft.VisualStudio.Editor;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.TextManager.Interop;
    using Microsoft.VisualStudio.Utilities;
    using System;
    using System.ComponentModel.Composition;
    using System.IO;
    using System.Linq;


    [Export(typeof(IVsTextViewCreationListener))]
    [ContentType("any")]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    public class ViewCreationListener : IVsTextViewCreationListener
    {
        [Import]
        internal IVsEditorAdaptersFactoryService AdaptersFactory = null;

        [Import]
        internal IContentTypeRegistryService ContentTypeRegistryService { get; set; }

        public void VsTextViewCreated(IVsTextView textViewAdapter)
        {
            try
            {
                if (!(AdaptersFactory.GetWpfTextView(textViewAdapter) is IWpfTextView view)) return;
                view.Closed += OnViewClosed;
                Microsoft.VisualStudio.Text.ITextBuffer buffer = view.TextBuffer;
                if (buffer == null) return;
                string ffn = buffer.GetFFN();
                if (ffn == null) return;
                if (!(Path.GetExtension(ffn) == ".g4"
                    || Path.GetExtension(ffn) == ".g3"
                    || Path.GetExtension(ffn) == ".g2"
                    || Path.GetExtension(ffn) == ".g"
                    || Path.GetExtension(ffn) == ".ebnf"
                    || Path.GetExtension(ffn) == ".y")) return;
                if (!Options.Option.GetBoolean("OverrideAntlrPluggins")) return;
                IContentType content_type = buffer.ContentType;
                System.Collections.Generic.List<IContentType> content_types = ContentTypeRegistryService.ContentTypes.ToList();
                IContentType new_content_type = content_types.Find(ct => ct.TypeName == "Antlr");
                Type type_of_content_type = new_content_type.GetType();
                System.Reflection.Assembly assembly = type_of_content_type.Assembly;
                buffer.ChangeContentType(new_content_type, null);
            }
            catch (Exception)
            {
            }
        }

        private void OnViewClosed(object sender, EventArgs e)
        {
            if (!(sender is IWpfTextView view)) return;
            view.Closed -= OnViewClosed;
        }
    }
}
