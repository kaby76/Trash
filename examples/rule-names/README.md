# Construct XML from parser rule names

Demonstrates using XQuery4 direct element constructors to build XML from an
ANTLR4 grammar parse tree.

The query selects every parser-rule name with:

```xquery
//parserRuleSpec/RULE_REF/text()
```

It creates one `<ruleName>` element for each selected text node and wraps the
result in a `<ruleNames>` element.

## Running

```sh
bash run-example.sh
```

Or run the pipeline directly:

```sh
dotnet trash parse RuleNames.g4 \
  | dotnet trash xquery --query rule-names.xq \
  | dotnet trash tree
```

`trparse` parses `RuleNames.g4`, `trxquery` constructs a new XML element tree,
and `trtree` displays that constructed tree, including its text values.

The output is:

```text
ruleNames
├── ruleName
│   └── "document"
├── ruleName
│   └── "header"
└── ruleName
    └── "section"
```
