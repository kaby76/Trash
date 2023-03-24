using Microsoft.VisualStudio.LanguageServer.Protocol;

namespace Server
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Threading.Tasks;

    internal partial class Program : IDisposable
    {
        private string logMessage;
        private readonly ObservableCollection<DiagnosticTag> tags = new ObservableCollection<DiagnosticTag>();
        private LSPServer languageServer;
        private string responseText;
        private string currentSettings;
        private MessageType messageType;
        private bool isDisposed;

        public static void Main(string[] args)
        {
            //TimeSpan delay = new TimeSpan(0, 0, 0, 20);
            //Console.Error.WriteLine("Waiting " + delay + " seconds...");
            //Thread.Sleep((int)delay.TotalMilliseconds);
            LoggerNs.Logger.Log.WriteLine("Starting");
            Program program = new Program();
#pragma warning disable VSTHRD002 // Avoid problematic synchronous waits
            program.MainAsync(args).GetAwaiter().GetResult();
#pragma warning restore VSTHRD002 // Avoid problematic synchronous waits
        }

#pragma warning disable IDE0060 // Remove unused parameter
#pragma warning disable CA1801
        private async Task MainAsync(string[] args)
#pragma warning restore CA1801
#pragma warning restore IDE0060 // Remove unused parameter
        {
            System.IO.Stream stdin = Console.OpenStandardInput();
            System.IO.Stream stdout = Console.OpenStandardOutput();
            stdin = new LspTools.LspHelpers.EchoStream(stdin, new Dup("editor"),
                    LspTools.LspHelpers.EchoStream.StreamOwnership.OwnNone);
            stdout = new LspTools.LspHelpers.EchoStream(stdout, new Dup("server"),
                    LspTools.LspHelpers.EchoStream.StreamOwnership.OwnNone);
            languageServer = new LSPServer(stdout, stdin);
            languageServer.Disconnected += OnDisconnected;
            languageServer.PropertyChanged += OnLanguageServerPropertyChanged;
            Tags.Add(new DiagnosticTag());
            LogMessage = string.Empty;
            ResponseText = string.Empty;
#pragma warning disable CA2007 // Do not directly await a Task
            await Task.Delay(-1);
#pragma warning restore CA2007 // Do not directly await a Task
        }

        private void OnLanguageServerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentSettings")
            {
                CurrentSettings = languageServer.CurrentSettings;
            }
        }

        internal void SendLogMessage()
        {
            languageServer.LogMessage(message: LogMessage, messageType: MessageType);
        }

        internal void SendMessage()
        {
            languageServer.ShowMessage(message: LogMessage, messageType: MessageType);
        }

        internal void SendMessageRequest()
        {
            _ = Task.Run(async () =>
              {
                  MessageActionItem selectedAction = await languageServer.ShowMessageRequestAsync(message: LogMessage, messageType: MessageType, actionItems: new string[] { "option 1", "option 2", "option 3" });
                  ResponseText = $"The user selected: {selectedAction?.Title ?? "cancelled"}";
              });
        }

        private void OnDisconnected(object sender, System.EventArgs e)
        {
            //MediaTypeNames.Application.Current.Dispatcher.BeginInvokeShutdown(DispatcherPriority.Normal);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<DiagnosticTag> Tags => tags;

        public string LogMessage
        {
            get => logMessage;
            set
            {
                logMessage = value;
                NotifyPropertyChanged(nameof(LogMessage));
            }
        }

        public string ResponseText
        {
            get => responseText;
            set
            {
                responseText = value;
                NotifyPropertyChanged(nameof(ResponseText));
            }
        }

        public string CustomText
        {
            get => languageServer.CustomText;
            set
            {
                languageServer.CustomText = value;
                NotifyPropertyChanged(nameof(CustomText));
            }
        }

        public MessageType MessageType
        {
            get => messageType;
            set
            {
                messageType = value;
                NotifyPropertyChanged(nameof(MessageType));
            }
        }

        public string CurrentSettings
        {
            get => currentSettings;
            set
            {
                currentSettings = value;
                NotifyPropertyChanged(nameof(CurrentSettings));
            }
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed) return;
            if (disposing)
            {
                // free managed resources
                languageServer.Dispose();
            }
            isDisposed = true;
        }

        ~Program()
        {
            // Finalizer calls Dispose(false)
            Dispose(false);
        }
    }

    public class MyDiagnosticTag : INotifyPropertyChanged
    {
        private string text;
        private DiagnosticSeverity severity;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Text
        {
            get => text;
            set
            {
                text = value;
                OnPropertyChanged(nameof(Text));
            }
        }

        public DiagnosticSeverity Severity
        {
            get => severity;
            set
            {
                severity = value;
                OnPropertyChanged(nameof(Severity));
            }
        }

        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
