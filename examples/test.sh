#!/usr/bin/env bash

# Run every documented example as an integration test. Individual examples may
# add stronger assertions in their own test-example.sh script.
set -u

cd "$(dirname "$0")"

failed=()
for example_runner in */run-example.sh; do
    directory=${example_runner%/run-example.sh}
    runner="$directory/test-example.sh"
    if [[ ! -f "$runner" ]]; then
        runner="$directory/run-example.sh"
    fi

    echo "=== Testing example: $directory ==="
    if ! bash "$runner"; then
        failed+=("$directory")
    fi
done

if (( ${#failed[@]} != 0 )); then
    echo "Example tests failed: ${failed[*]}" >&2
    exit 1
fi

echo "All example tests succeeded."
