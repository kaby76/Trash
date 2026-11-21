# CongoCC grammar

This grammar parses the outer structure of [CongoCC](https://github.com/congo-cc/congo-parser-generator) `.ccc` files: settings, includes, token productions, parser productions, and injections. Embedded target-language code is preserved as balanced blocks. It does not resolve includes, evaluate preprocessor conditions, or parse action contents. Known inactive documentation blocks (`#if false` and `#if __YouAreInterested__`) are skipped.

The [self-hosted CongoCC grammar](https://github.com/congo-cc/congo-parser-generator/blob/master/src/grammars/CongoCC.ccc) is the authoritative syntax definition.

The `examples/kotlin`, `examples/lua`, and `examples/rust` directories contain
all eight `.ccc` files from [congo-grammars](https://github.com/congo-cc/congo-grammars).
They are covered by the generated parser's example test. Their MIT license is
included in `examples/LICENSE.congo-grammars`.

The `examples/congo-parser-generator` directory contains all six `.ccc` files
from the [CongoCC source grammars](https://github.com/congo-cc/congo-parser-generator/tree/master/src/grammars),
including the self-hosted grammar. Its MIT license is included alongside them.
