namespace LanguageServer
{
    using System;

    public class DocumentSymbol
    {
        /**
	     * The name of this symbol. Will be displayed in the user interface and therefore must not be
	     * an empty string or a string only consisting of white spaces.
	     */
        public string name;

        /**
	     * The kind of this symbol.
	     */
        public int kind;

        /**
	     * The range enclosing this symbol not including leading/trailing whitespace but everything else
	     * like comments. This information is typically used to determine if the clients cursor is
	     * inside the symbol to reveal in the symbol in the UI.
	     */
        public Workspaces.Range range;

        public Uri Document;
    }
}
