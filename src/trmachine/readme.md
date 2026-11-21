# trmachine

`dotnet trash machine` prints the hardware and operating system details useful when publishing performance measurements. `dotnet trash machine --json` emits the same data as JSON for scripts.

The report includes CPU model, core counts, reported clock speed and its meaning, physical and currently available RAM, OS and runtime versions, disks and their media types, GPU names, and motherboard details when the host exposes them. Unavailable values are shown as `unknown` in text output or `null` in JSON. Disk media type may be `unknown` when the operating system cannot distinguish SSD from HDD. CPU clock speed is a rated maximum on Windows and Linux when available; a current frequency may be used on Linux otherwise.

Example:

```sh
dotnet trash machine > machine.txt
dotnet trash machine --json > machine.json
```

Run this on the same host as the benchmark, and keep its output beside the measurements.
