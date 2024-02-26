namespace LspAntlr
{
    using LoggerNs;
    using LanguageServer;
    using Microsoft.VisualStudio.ComponentModelHost;
    using Microsoft.VisualStudio.LanguageServer.Client;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Threading;
    using Microsoft.VisualStudio.Utilities;
    using Options;
    using StreamJsonRpc;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Threading.Tasks;
    using Task = System.Threading.Tasks.Task;

    [ContentType("Antlr")]
    [Export(typeof(ILanguageClient))]
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(AntlrLanguageClient.PackageGuidString)]
    public class AntlrLanguageClient : ILanguageClient
    {
        public const string PackageGuidString = "49bf9144-398a-467c-9b87-ac26d1e62737";
        private static readonly MemoryStream _log_from_server = new MemoryStream();
        private static readonly MemoryStream _log_to_server = new MemoryStream();
        private static JsonRpc _rpc;

        public AntlrLanguageClient()
        {
            Logger.Log.CleanUpLogFile();
            Instance = this;
            IComponentModel componentModel = Package.GetGlobalService(typeof(SComponentModel)) as IComponentModel;
            //OptionsCommand.Initialize(this);
            //AboutCommand.Initialize(this);
        }

        public event AsyncEventHandler<EventArgs> StartAsync;
        public event AsyncEventHandler<EventArgs> StopAsync;
        public static AntlrLanguageClient Instance { get; set; }
        public IEnumerable<string> ConfigurationSections => null;
        public object CustomMessageTarget => null;
        public IEnumerable<string> FilesToWatch => null;
        public object InitializationOptions => null;
        public object MiddleLayer => null;
        public string Name => "Antlr language extension";

        string ILanguageClient.Name => "Antlr language extension";

        object ILanguageClient.InitializationOptions => null;

        IEnumerable<string> ILanguageClient.FilesToWatch => null;

        bool ILanguageClient.ShowNotificationOnInitializeFailed => true;
        public async Task OnLoadedAsync()
        {
            if (StartAsync != null)
            {
                await StartAsync.InvokeAsync(this, EventArgs.Empty);
            }
        }

        public async Task StopServerAsync()
        {
            if (StopAsync != null)
            {
                await StopAsync.InvokeAsync(this, EventArgs.Empty);
            }
        }

        public Task OnServerInitializedAsync()
        {
            return Task.CompletedTask;
        }

        protected async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            throw new NotImplementedException();
            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            //await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            //await Command1.InitializeAsync(this);
        }

        public async Task<Connection> ActivateAsync(CancellationToken token)
        {
            await Task.Yield();
            try
            {
                string cache_location = System.IO.Path.GetTempPath();
                Type t = typeof(AntlrLanguageClient);
                System.Reflection.Assembly a = t.Assembly;
                string f = System.IO.Path.GetFullPath(a.Location);
                string p = System.IO.Path.GetDirectoryName(f);
                //string antlr_executable = p + System.IO.Path.DirectorySeparatorChar
                //                            + @"Server\net8.0\LspServer.exe";
                string antlr_executable = @"c:\Users\Kenne\Documents\GitHub\Domemtech.Trash\src\LspServer\bin\Debug\net8.0\LspServer.exe";
                string workspace_path = cache_location;
                if (string.IsNullOrEmpty(workspace_path))
                {
                    workspace_path = cache_location;
                }

                ProcessStartInfo info = new ProcessStartInfo
                {
                    FileName = antlr_executable,
                    WorkingDirectory = workspace_path,
                    Arguments = workspace_path,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = !Option.GetBoolean("VisibleServerWindow")
                };
                Process process = new Process
                {
                    StartInfo = info
                };
                if (process.Start())
                {
                    bool debug = false;
                    Stream @out = process.StandardOutput.BaseStream;
                    Stream eout = debug
                        ? new LspTools.LspHelpers.EchoStream(@out, _log_from_server,
                            LspTools.LspHelpers.EchoStream.StreamOwnership.OwnNone)
                        : @out;
                    Stream @in = process.StandardInput.BaseStream;
                    Stream ein = debug
                        ? new LspTools.LspHelpers.EchoStream(@in, _log_to_server,
                            LspTools.LspHelpers.EchoStream.StreamOwnership.OwnNone)
                        : @in;

                    return new Connection(eout, ein);
                }
            }
            catch (Exception eeks)
            {
                //Logger.Log.Notify(eeks.ToString());
            }

            return null;
        }

        //        public async Task AttachForCustomMessageAsync(JsonRpc rpc)
        //        {
        //            await Task.Yield();
        //            _rpc = rpc;
        //        }

        //#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        //        public async Task OnLoadedAsync()
        //#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        //        {
        //            _ = StartAsync.InvokeAsync(this, EventArgs.Empty);
        //        }

        //        public Task OnServerInitializedAsync()
        //        {
        //            return Task.CompletedTask;
        //        }

        //        public Task OnServerInitializeFailedAsync(Exception e)
        //        {
        //            return Task.CompletedTask;
        //        }


        //        public static string CMVersion()
        //        {
        //            try
        //            {
        //                if (_rpc == null) return null;
        //                var context = ThreadHelper.JoinableTaskContext;
        //                var jtf = new JoinableTaskFactory(context);
        //                var result = jtf.Run(() => _rpc.InvokeAsync<string>("CMVersion"));
        //                return result;
        //            }
        //            catch (Exception)
        //            {
        //            }
        //            return null;
        //        }

        //        async Task ILanguageClient.OnLoadedAsync()
        //        {
        //            await StartAsync.InvokeAsync(this, EventArgs.Empty);
        //        }

        //        Task ILanguageClient.OnServerInitializedAsync()
        //        {
        //            return Task.CompletedTask;
        //        }

        //        Task<Connection> ILanguageClient.ActivateAsync(CancellationToken token)
        //        {
        //            return null;
        //        }

        //        Task<InitializationFailureContext> ILanguageClient.OnServerInitializeFailedAsync(ILanguageClientInitializationInfo initializationState)
        //        {
        //            return null;
        //        }
        public Task<InitializationFailureContext> OnServerInitializeFailedAsync(ILanguageClientInitializationInfo initializationState)
        {
            string message = "Oh no! Foo Language Client failed to activate, now we can't test LSP! :(";
            string exception = initializationState.InitializationException?.ToString() ?? string.Empty;
            message = $"{message}\n {exception}";

            var failureContext = new InitializationFailureContext()
            {
                FailureMessage = message,
            };

            return Task.FromResult(failureContext);
        }
    }
}
