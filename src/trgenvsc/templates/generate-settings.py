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

# print("Finding lexer rules with single character string literals.")
files = glob.glob('parser/Generated-*/*.g4', recursive=False)
command = ["dotnet", "trparse", "--", "-t", "ANTLRv4"] + files
p1 = subprocess.Popen(command, stdout=subprocess.PIPE)
p2 = subprocess.Popen(["dotnet", "trxgrep", "--", "-e", '''
        //lexerRuleSpec
	/lexerRuleBlock
 	/lexerAltList[not(OR)]
        /lexerAlt[not(lexerCommands)]
	/lexerElements[count(*)=1]
	/lexerElement[not(ebnfSuffix)]
	/lexerAtom
	/terminalDef[not(elementOptions)]
	/STRING_LITERAL[string-length(.) < 4]
	/text()'''], stdin=p1.stdout, stdout=subprocess.PIPE)
string_literals = p2.stdout.read().decode().splitlines()
string_literals = [item.strip("'") for item in string_literals]

# print("Finding lexer rule names with single character string literals.")
p1 = subprocess.Popen(command, stdout=subprocess.PIPE)
p2 = subprocess.Popen(["dotnet", "trxgrep", "--", "-e", '''
        //lexerRuleSpec
	[
	lexerRuleBlock
	/lexerAltList[not(OR)]
        /lexerAlt[not(lexerCommands)]
	/lexerElements[count(*)=1]
	/lexerElement[not(ebnfSuffix)]
	/lexerAtom
	/terminalDef[not(elementOptions)]
	/STRING_LITERAL[string-length(.) < 4]]
	/TOKEN_REF
	/text()'''], stdin=p1.stdout, stdout=subprocess.PIPE)
names = p2.stdout.read().decode().splitlines()
names = [item.strip("'") for item in names]

if len(string_literals) != len(names):
    print("string_literals and names are not the same length.")
    exit()

# print("Finding lexer rule names with string literals of length greater than one character.")
# print("These will be considered keywords.")
p1 = subprocess.Popen(command, stdout=subprocess.PIPE)
p2 = subprocess.Popen(["dotnet", "trxgrep", "--", "-e", '''
        //lexerRuleSpec
	[
	lexerRuleBlock
	/lexerAltList[not(OR)]
        /lexerAlt[not(lexerCommands)]
	/lexerElements[count(*)=1]
	/lexerElement[not(ebnfSuffix)]
	/lexerAtom
	/terminalDef[not(elementOptions)]
	/STRING_LITERAL[string-length(.) > 4]]
	/TOKEN_REF
	/text()'''], stdin=p1.stdout, stdout=subprocess.PIPE)
keywords = p2.stdout.read().decode().splitlines()
keywords = [item.strip("'") for item in keywords]

# Fetch and parse Unicode data
lines = fetch_unicode_data()
unicode_dict = parse_unicode_data(lines)

props = []
counter = 0
for line_number in range(0, len(string_literals)):
    input_char = string_literals[line_number]
    hex_output = char_to_hex(input_char)
    if len(hex_output) == 2:
        hex_output = "00" + hex_output
    property_info = get_unicode_property(unicode_dict, hex_output)
    props.append(property_info)

if len(props) != len(string_literals):
    print("props and string_literals are not the same length.")
    print("len(props) = ", end="")
    print(len(props))
    print("len(string_literals) = ", end="")
    print(len(string_literals))
    exit()

combined = list(zip(props, names))

# Sort the combined list by the first element of each pair (props)
combined_sorted = sorted(combined)

# Unzip the sorted pairs back into two lists
props_sorted, names_sorted = zip(*combined_sorted)

# Convert the tuples back into lists
props_sorted = list(props_sorted)
names_sorted = list(names_sorted)

# Output settings.rc
print('''
[{
 "Suffix":".g4",
 "LanguageId":"any",
 "ParserLocation":"parser/Generated-CSharp/bin/Debug/net10.0/Test.dll",
 "ClassesAndClassifiers":[
''')

classes = [
'type',
'class',
'enum',
'interface',
'struct',
'typeParameter',
'parameter',
'variable',
'property',
'enumMember',
'event',
'function',
'method',
'macro',
'modifier',
'comment',
'string',
'number',
'regexp',
'operator',
'decorator'];
        
previous=""
class_index = 0
for i in range(0, len(props_sorted)):
    p = props_sorted[i]
    n = names_sorted[i]
    if previous != p:
        if previous != "":
            print(''')"},''')
            class_index = class_index + 1
        print('''    {"Item1":"''', end = "")
        print(classes[class_index], end = "")
        print('''","Item2":"//(''', end = "")
        print(n, end = "")
        previous = p
    else:
        print(" | " + n, end = "")

if len(keywords) > 0:
    print(''')"},
    {"Item1":"keyword","Item2":"//(''', end = "")
    for i in range(0, len(keywords) - 1):
        if i > 0:
            print(" | ", end = "")
        print(keywords[i], end = "")

print(''')"}
  ]
}]
''')
