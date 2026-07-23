#!/usr/bin/env python3
"""
Compare two ATN DOT files for structural equivalence, ignoring state numbers.

Two files are considered equivalent when their graphs are isomorphic and all
edge labels and node types match.  State numbers in node IDs and node labels
are replaced by BFS-order positions before comparison, so purely cosmetic
renumbering does not produce a diff.

Usage:
    python compare-atn.py a.dot b.dot          # print diffs, exit 1 if any
    python compare-atn.py -q a.dot b.dot       # silent, exit code only
    python compare-atn.py -r dir_a dir_b       # compare matching .dot files
"""

import os
import re
import sys
from collections import defaultdict


# ---------------------------------------------------------------------------
# Parsing
# ---------------------------------------------------------------------------

_NODE_RE = re.compile(
    r'^s(\d+)\s*\['
    r'.*?label\s*=\s*"((?:[^"\\]|\\.)*)"'
    r'.*?shape\s*=\s*(\w+)'
    r'(?:.*?peripheries\s*=\s*(\d+))?'
    r'.*?\]'
)

_EDGE_RE = re.compile(
    r'^s(\d+)(?::\w+)?\s*->\s*s(\d+)\s*\['
    r'.*?label\s*=\s*"((?:[^"\\]|\\.)*)"'
    r'.*?\]'
)


def parse_dot(text):
    """Return (nodes, edges).

    nodes: {state_num: semantic_type_str}
    edges: [(src, dst, label)]
    """
    nodes = {}
    edges = []

    for raw in text.splitlines():
        line = raw.strip()

        m = _NODE_RE.match(line)
        if m:
            num = int(m.group(1))
            label = m.group(2)
            shape = m.group(3)
            peripheries = int(m.group(4)) if m.group(4) else (2 if shape == 'doublecircle' else 1)
            nodes[num] = _node_type(label, shape, peripheries, num)
            continue

        m = _EDGE_RE.match(line)
        if m:
            edges.append((int(m.group(1)), int(m.group(2)), m.group(3)))

    return nodes, edges


def _node_type(label, shape, peripheries, num):
    """Semantic description of a node, with the state number stripped out."""
    if shape == 'doublecircle' or peripheries == 2:
        return 'RULE_STOP'

    if shape == 'record':
        # "{N\nd=D|{p0|p1|...}}"  or  "{N\nd=D|{p0|p1|}}"
        # d=N is a global decision index that differs between tools; ignore it.
        ports = len(re.findall(r'\bp\d+\b', label))
        return f'DECISION(alts={ports})'

    # circle — inspect label content
    # Strip the state number so we can classify the residual.
    # Patterns emitted by AtnDotWriter:
    #   plain BASIC:           "N"
    #   block-start (arrow):   "&rarr;\\nN"   (possibly with * or + suffix)
    #   block-end  (arrow):    "&larr;\\nN"
    #   star-loopback:         "N*"
    #   plus-loopback:         "N+"
    #   loop-end:              "N"   (same as basic — treated the same here)
    residual = re.sub(r'\b' + str(num) + r'\b', '', label).strip()

    if '&rarr;' in residual or '→' in residual:
        suffix = '*' if '*' in residual else ('+' if '+' in residual else '')
        return f'BLOCK_START{suffix}'

    if '&larr;' in residual or '←' in residual:
        return 'BLOCK_END'

    if residual == '*':
        return 'STAR_LOOPBACK'

    if residual == '+':
        return 'PLUS_LOOPBACK'

    return 'BASIC'


# ---------------------------------------------------------------------------
# Canonicalisation
# ---------------------------------------------------------------------------

def canonicalize(nodes, edges):
    """BFS from the unique entry state; return (node_seq, edge_seq).

    node_seq: [(canonical_id, semantic_type)]  sorted by canonical_id
    edge_seq: [(canonical_src, canonical_dst, label)]  sorted
    """
    if not nodes:
        return [], []

    adj = defaultdict(list)        # src -> [(dst, label)]
    predecessors = defaultdict(set)

    for src, dst, label in edges:
        adj[src].append((dst, label))
        predecessors[dst].add(src)

    # Entry = no predecessors; break ties by lowest original state number.
    no_pred = [n for n in nodes if n not in predecessors]
    start = min(no_pred) if no_pred else min(nodes)

    # BFS, ordering siblings by edge label for determinism.
    mapping = {}   # original_num -> canonical_num
    queue = [start]
    seen = {start}
    counter = 0

    while queue:
        node = queue.pop(0)
        mapping[node] = counter
        counter += 1
        for dst, _label in sorted(adj[node], key=lambda x: x[1]):
            if dst not in seen:
                seen.add(dst)
                queue.append(dst)

    # Any unreachable nodes (shouldn't happen in well-formed ATN, but just in case)
    for n in sorted(nodes):
        if n not in mapping:
            mapping[n] = counter
            counter += 1

    node_seq = sorted((mapping[n], t) for n, t in nodes.items())
    edge_seq = sorted(
        (mapping[s], mapping[d], lbl)
        for s, d, lbl in edges
        if s in mapping and d in mapping
    )

    return node_seq, edge_seq


# ---------------------------------------------------------------------------
# Comparison
# ---------------------------------------------------------------------------

def compare_files(path_a, path_b):
    """Return list of human-readable difference strings (empty = identical)."""
    with open(path_a, encoding='utf-8') as f:
        text_a = f.read()
    with open(path_b, encoding='utf-8') as f:
        text_b = f.read()

    nodes_a, edges_a = parse_dot(text_a)
    nodes_b, edges_b = parse_dot(text_b)

    canon_nodes_a, canon_edges_a = canonicalize(nodes_a, edges_a)
    canon_nodes_b, canon_edges_b = canonicalize(nodes_b, edges_b)

    diffs = []

    # --- node count ---
    if len(canon_nodes_a) != len(canon_nodes_b):
        diffs.append(
            f'state count: {len(canon_nodes_a)} (a) vs {len(canon_nodes_b)} (b)'
        )
    else:
        for (ci_a, type_a), (ci_b, type_b) in zip(canon_nodes_a, canon_nodes_b):
            if type_a != type_b:
                diffs.append(f'state #{ci_a}: type {type_a!r} (a) vs {type_b!r} (b)')

    # --- edge count ---
    if len(canon_edges_a) != len(canon_edges_b):
        diffs.append(
            f'edge count: {len(canon_edges_a)} (a) vs {len(canon_edges_b)} (b)'
        )
    else:
        for (sa, da, la), (sb, db, lb) in zip(canon_edges_a, canon_edges_b):
            if (sa, da) != (sb, db):
                diffs.append(
                    f'edge connectivity: {sa}→{da} (a) vs {sb}→{db} (b)'
                )
            elif la != lb:
                diffs.append(
                    f'edge {sa}→{da} label: {la!r} (a) vs {lb!r} (b)'
                )

    return diffs


def compare_dirs(dir_a, dir_b, quiet=False):
    """Compare matching .dot files in two directories. Return True if all match."""
    dots_a = {f for f in os.listdir(dir_a) if f.endswith('.dot')}
    dots_b = {f for f in os.listdir(dir_b) if f.endswith('.dot')}

    only_a = sorted(dots_a - dots_b)
    only_b = sorted(dots_b - dots_a)
    common = sorted(dots_a & dots_b)

    all_match = True

    for f in only_a:
        print(f'only in a: {f}')
        all_match = False
    for f in only_b:
        print(f'only in b: {f}')
        all_match = False

    for f in common:
        diffs = compare_files(os.path.join(dir_a, f), os.path.join(dir_b, f))
        if diffs:
            all_match = False
            if not quiet:
                print(f'{f}:')
                for d in diffs:
                    print(f'  {d}')
        else:
            if not quiet:
                pass  # silent on match

    return all_match


# ---------------------------------------------------------------------------
# Entry point
# ---------------------------------------------------------------------------

def main():
    args = sys.argv[1:]
    quiet = False

    if '-q' in args:
        quiet = True
        args = [a for a in args if a != '-q']

    if '-r' in args:
        args = [a for a in args if a != '-r']
        if len(args) != 2:
            print('Usage: compare-atn.py -r dir_a dir_b', file=sys.stderr)
            sys.exit(2)
        ok = compare_dirs(args[0], args[1], quiet=quiet)
        sys.exit(0 if ok else 1)

    if len(args) != 2:
        print('Usage: compare-atn.py [-q] a.dot b.dot', file=sys.stderr)
        print('       compare-atn.py [-q] -r dir_a dir_b', file=sys.stderr)
        sys.exit(2)

    diffs = compare_files(args[0], args[1])
    if diffs:
        if not quiet:
            for d in diffs:
                print(d)
        sys.exit(1)
    else:
        if not quiet:
            print('Files match.')
        sys.exit(0)


if __name__ == '__main__':
    # Ensure UTF-8 output on Windows consoles.
    if hasattr(sys.stdout, 'reconfigure'):
        sys.stdout.reconfigure(encoding='utf-8', errors='replace')
    main()
