#!/usr/bin/env python3
"""Sort nodes and edges in a Graphviz DOT file numerically by state number.

Usage: python sort-dot.py <file.dot>
       python sort-dot.py <file.dot> <reference.dot>   # diff mode
"""

import re
import sys


def node_key(line):
    m = re.match(r's(\d+)', line)
    return int(m.group(1)) if m else 0


def edge_key(line):
    m = re.match(r's(\d+)(?::(\w+))?\s*->\s*s(\d+)', line)
    if m:
        src = int(m.group(1))
        port = m.group(2) or ''
        dst = int(m.group(3))
        return (src, port, dst)
    return (0, '', 0)


def sort_dot(text):
    lines = text.splitlines()
    header = []
    options = []
    nodes = []
    edges = []
    footer = []

    for line in lines:
        stripped = line.strip()
        if stripped.startswith('digraph') or stripped.startswith('//'):
            header.append(line)
        elif stripped == '}':
            footer.append(line)
        elif re.match(r's\d+\[', stripped):
            nodes.append(line)
        elif re.match(r's\d+', stripped) and '->' in stripped:
            edges.append(line)
        else:
            options.append(line)

    nodes.sort(key=node_key)
    edges.sort(key=edge_key)

    return '\n'.join(header + options + nodes + edges + footer) + '\n'


def main():
    if len(sys.argv) < 2:
        print(f"Usage: {sys.argv[0]} <file.dot> [reference.dot]", file=sys.stderr)
        sys.exit(1)

    with open(sys.argv[1], 'r') as f:
        sorted_text = sort_dot(f.read())

    if len(sys.argv) == 3:
        with open(sys.argv[2], 'r') as f:
            ref_sorted = sort_dot(f.read())
        if sorted_text == ref_sorted:
            print("Files match.")
        else:
            import difflib
            diff = difflib.unified_diff(
                ref_sorted.splitlines(keepends=True),
                sorted_text.splitlines(keepends=True),
                fromfile=sys.argv[2],
                tofile=sys.argv[1],
            )
            sys.stdout.writelines(diff)
            sys.exit(1)
    else:
        sys.stdout.write(sorted_text)


if __name__ == '__main__':
    main()
