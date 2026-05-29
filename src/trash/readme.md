1.0.0 Unified dispatcher for the Trash toolkit.

# trash

`trash` is the unified dispatcher for the [Trash toolkit](https://github.com/kaby76/Trash) —
a collection of parse-tree manipulation tools built on ANTLR4.

## Install

    dotnet tool install -g trash

## Usage

    trash <command> [options]

Both the full command name and a short alias (without the `tr` prefix) are accepted:

    trash trgen  --help
    trash gen    --help
    trash trparse --help
    trash parse   --help

## Example pipeline

    trash trparse -g antlr4 MyGrammar.g4 | trash trtree

## Commands

| Full name      | Alias       |
|----------------|-------------|
| tranalyze      | analyze     |
| trcaret        | caret       |
| trclonereplace | clonereplace|
| trcombine      | combine     |
| trconvert      | convert     |
| trcover        | cover       |
| trdistill      | distill     |
| trdot          | dot         |
| trenum         | enum        |
| trextract      | extract     |
| trff           | ff          |
| trfold         | fold        |
| trfoldlit      | foldlit     |
| trformat       | format      |
| trgen          | gen         |
| trgenvsc       | genvsc      |
| trglob         | glob        |
| trgroup        | group       |
| triconv        | iconv       |
| tritext        | itext       |
| trjson         | json        |
| trkleene       | kleene      |
| trnullable     | nullable    |
| trparse        | parse       |
| trperf         | perf        |
| trpiggy        | piggy       |
| trquery        | query       |
| trrename       | rename      |
| trrr           | rr          |
| trrup          | rup         |
| trsem          | sem         |
| trsort         | sort        |
| trsplit        | split       |
| trsponge       | sponge      |
| trtext         | text        |
| trthompson     | thompson    |
| trtokens       | tokens      |
| trtree         | tree        |
| trull          | ull         |
| trunfold       | unfold      |
| trunfoldlit    | unfoldlit   |
| trungroup      | ungroup     |
| trwdog         | wdog        |
| trxgrep        | xgrep       |
| trxml          | xml         |
| trxml2         | xml2        |

Run `trash --help` to list all commands. Run `trash <command> --help` for per-command help.
