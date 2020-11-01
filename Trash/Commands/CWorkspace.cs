namespace Trash.Commands
{
    using Workspaces;

    class CWorkspace
    {
        public void Help()
        {
            System.Console.WriteLine(@"workspace
Create a new workspace.

Example:
    workspace
");
        }

        public void Execute(Repl repl, ReplParser.WorkspaceContext tree, bool piped)
        {
            repl._workspace = new Workspace();
        }
    }
}
