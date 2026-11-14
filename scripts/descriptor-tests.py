#!/usr/bin/env python3
"""Emit grammar, start rule, and input pattern for each desc.xml test."""

import sys
import xml.etree.ElementTree as ET


def value(node, name):
    element = node.find(name)
    return (element.text or "").strip() if element is not None else ""


root = ET.parse(sys.argv[1]).getroot()
for test in root.findall("test") or [root]:
    grammar = value(test, "grammar-name") or value(root, "grammar-name")
    start = value(root, "entry-point") or value(test, "entry-point")
    inputs = value(test, "inputs") or value(root, "inputs") or "examples/"
    print("|".join((grammar, start, inputs)))
