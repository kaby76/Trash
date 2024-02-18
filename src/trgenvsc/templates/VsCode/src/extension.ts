
import * as vscode from 'vscode';
import * as vscodelc from 'vscode-languageclient/node';


/**
 * Method to get workspace configuration option
 * @param option name of the option (e.g. for antlr.path should be path)
 * @param defaultValue default value to return if option is not set
 */
function getConfig<T>(option: string, defaultValue?: any): T
{
    const config = vscode.workspace.getConfiguration('uni-vscode');
    return config.get<T>(option, defaultValue);
}

namespace SwitchSourceHeaderRequest
{
    export const type = new vscodelc.RequestType<vscodelc.TextDocumentIdentifier, string|undefined,
                             void, void>('textDocument/switchSourceHeader');
}

class FileStatus
{
    private statuses = new Map<string, any>();
    private readonly statusBarItem =
        vscode.window.createStatusBarItem(vscode.StatusBarAlignment.Left, 10);

    onFileUpdated(fileStatus: any)
    {
        const filePath = vscode.Uri.parse(fileStatus.uri);
        this.statuses.set(filePath.fsPath, fileStatus);
        this.updateStatus();
    }

    updateStatus()
    {
        const path = vscode.window.activeTextEditor.document.fileName;
        const status = this.statuses.get(path);
        if (!status)
        {
            this.statusBarItem.hide();
            return;
        }
        this.statusBarItem.text = `uni-vscode: ` + status.state;
        this.statusBarItem.show();
    }

    clear()
    {
        this.statuses.clear();
        this.statusBarItem.hide();
    }

    dispose()
    {
        this.statusBarItem.dispose();
    }
}

let client: vscodelc.LanguageClient;

export function activate(context: vscode.ExtensionContext)
{
    var fn = "dotnet";
    
    var isWin = process.platform === "win32";
    var ag = __dirname + '/../Server/net7.0/Server.dll';
    if (! isWin) {
        ag = __dirname + '/../server/net7.0/Server.dll';
    }

    const server: vscodelc.Executable =
    {
        command: fn,
        args: [ag],
        options: { shell: false, detached: false }
    };

    const serverOptions: vscodelc.ServerOptions = server;

    let clientOptions: vscodelc.LanguageClientOptions =
    {
        // Register the server for plain text documents
        documentSelector: [
            {scheme: 'file', language: 'any'},
        ]
    };

    client = new vscodelc.LanguageClient('Universal Language Server', serverOptions, clientOptions);
    
    client.registerProposedFeatures();
    
    console.log('Universal Language Server is now active!');
    client.start();
}

export function deactivate(): Thenable<void> | undefined
{
    if (!client)
    {
        return undefined;
    }
    return client.stop();
}
