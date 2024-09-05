import sys
import urllib.request
import glob
import os
import subprocess

def fetch_unicode_data():
    with open("PropsList.txt", 'r', encoding='utf-8') as file:
        data = file.readlines()
        return data

def parse_unicode_data(lines):
    unicode_dict = {}
    for line in lines:
        # Ignore empty lines and comment lines
        if not line.strip() or line.startswith("#"):
            continue
        
        # Split line at the semicolon
        parts = line.split(';')
        code_range, property_info = parts[0].strip(), parts[1].strip()

        # Extract the comment (property after #)
        if '#' in parts[1]:
            property_info = parts[1].split('#')[1].strip().split(' ')[0].strip()
        
        # Handle single code point or range of code points
        if '..' in code_range:
            start, end = code_range.split('..')
            for code_point in range(int(start, 16), int(end, 16) + 1):
                unicode_dict[format(code_point, '04X')] = property_info
        else:
            unicode_dict[code_range] = property_info
            
    return unicode_dict

def get_unicode_property(unicode_dict, code_point):
    return unicode_dict.get(code_point.upper(), "Code point not found")

class FileLineReader:
    def __init__(self, file_name):
        self.lines = []
        self.num_lines = 0
        try:
            with open(file_name, 'r') as file:
                self.lines = file.readlines()
                self.num_lines = len(self.lines)
        except FileNotFoundError:
            print(f"Error: File '{file_name}' not found.")
    
    def read_line_from_file(self, line_number):
        if 1 <= line_number <= self.num_lines:
            return self.lines[line_number - 1].rstrip('\n')
        else:
            return f"Error: Line number {line_number} is out of range."
    
    def get_number_of_lines(self):
        return self.num_lines
    
def char_to_hex(character):
    # Convert character to its UTF-8 byte representation
    utf8_bytes = character.encode('utf-8')
    # Convert bytes to hexadecimal and join them
    hex_string = ''.join(format(byte, '02x') for byte in utf8_bytes)
    return hex_string

if __name__ == "__main__":
    if len(sys.argv) != 2:
        print("Usage: python find-prop.py <hex>")
        sys.exit(1)
    # Fetch and parse Unicode data
    lines = fetch_unicode_data()
    unicode_dict = parse_unicode_data(lines)
    hex = sys.argv[1]
    property_info = get_unicode_property(unicode_dict, hex)
    print(property_info)
