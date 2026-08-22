# XQuery4 performance benchmarks

Run the node-set scaling benchmark in Release configuration:

```sh
dotnet run --project src/xquery4/benchmarks/XQuery.Benchmarks -c Release
```

The benchmark evaluates an overlapping union over increasingly large documents.
It verifies duplicate elimination and reports total elapsed time and nanoseconds
per result node. Stable nanoseconds per node as the input doubles indicates the
expected near-linear node-set behavior.
