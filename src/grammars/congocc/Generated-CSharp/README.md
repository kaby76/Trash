# CongoCC grammar

This grammar parses the outer structure of [CongoCC](https://github.com/congo-cc/congo-parser-generator) `.ccc` files: settings, includes, token productions, parser productions, and injections. Embedded target-language code is preserved as balanced blocks. It does not preprocess `#if` directives, resolve includes, or parse action contents.

The [self-hosted CongoCC grammar](https://github.com/congo-cc/congo-parser-generator/blob/master/src/grammars/CongoCC.ccc) is the authoritative syntax definition.
