namespace LspAntlr
{
    using Microsoft.VisualStudio.Shell;
    using System;
    using System.ComponentModel.Design;

    public class OptionsCommand
    {
        private readonly Microsoft.VisualStudio.Shell.Package _package;
        private readonly MenuCommand _menu_item1;

        private OptionsCommand(Microsoft.VisualStudio.Shell.Package package)
        {
            _package = package ?? throw new ArgumentNullException(nameof(package));
            OleMenuCommandService commandService = ((IServiceProvider)ServiceProvider).GetService(
                typeof(IMenuCommandService)) as OleMenuCommandService ?? throw new Exception("Command service not found.");

            {
                CommandID menuCommandID = new CommandID(new Guid(LspAntlr.Constants.guidMenuAndCommandsCmdSet), 0x7007);
                _menu_item1 = new MenuCommand(MenuItemCallback, menuCommandID)
                {
                    Enabled = true,
                    Visible = true
                };
                commandService.AddCommand(_menu_item1);
            }
        }

        public static OptionsCommand Instance { get; private set; }

        private IServiceProvider ServiceProvider => _package;

        public static void Initialize(Microsoft.VisualStudio.Shell.Package package)
        {
            Instance = new OptionsCommand(package);
        }

        private void MenuItemCallback(object sender, EventArgs e)
        {
            //OptionsBox inputDialog = new OptionsBox();
            //inputDialog.Show();
        }
    }
}
