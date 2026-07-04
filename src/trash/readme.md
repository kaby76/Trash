1.1.2 Unified dispatcher for the Trash toolkit. Fix broken Cpp target on Github. Add tokens per second perf measurement. Added more perf measurements to templates.

# trash

`trash` is the unified dispatcher for the [Trash toolkit](https://github.com/kaby76/Trash) —
a collection of parse-tree manipulation tools built on ANTLR4.

## Install Globally

    dotnet tool install -g trash

## Uninstall

    dotnet tool uninstall -g trash

## Install Locally

    dotnet new tool-manifest
    dotnet tool install trash

## Usage

    trash <command> [options]

Both the full command name and a short alias (without the `tr` prefix) are accepted:

    dotnet trash gen --help
    dotnet trash parse --help

## Example pipeline

    dotnet trash parse -g antlr4 MyGrammar.g4 | dotnet trash tree

## Commands

| Full name      | Alias       |
|----------------|-------------|
| tranalyze      | analyze     |
| trcaret        | caret       |
| trclonereplace | clonereplace|
| trcombine      | combine     |
| trconvert      | convert     |
| trcover        | cover       |
| trdot          | dot         |
| trextract      | extract     |
| trff           | ff          |
| trfoldlit      | foldlit     |
| trgen          | gen         |
| trgenvsc       | genvsc      |
| trglob         | glob        |
| triconv        | iconv       |
| tritext        | itext       |
| trjson         | json        |
| trnullable     | nullable    |
| trparse        | parse       |
| trperf         | perf        |
| trquery        | query       |
| trrename       | rename      |
| trsort         | sort        |
| trsplit        | split       |
| trsponge       | sponge      |
| trtext         | text        |
| trtokens       | tokens      |
| trtree         | tree        |
| trunfold       | unfold      |
| trunfoldlit    | unfoldlit   |
| trungroup      | ungroup     |
| trwdog         | wdog        |
| trxgrep        | xgrep       |
| trxml          | xml         |
| trxml2         | xml2        |

Run `trash --help` to list all commands. Run `trash <command> --help` for per-command help.
