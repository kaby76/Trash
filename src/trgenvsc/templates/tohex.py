import sys

def char_to_hex(character):
    # Convert character to its UTF-8 byte representation
    utf8_bytes = character.encode('utf-8')
    # Convert bytes to hexadecimal and join them
    hex_string = ''.join(format(byte, '02x') for byte in utf8_bytes)
    return hex_string

if __name__ == "__main__":
    if len(sys.argv) != 2:
        print("Usage: python tohex.py <character>")
        sys.exit(1)
    
    input_char = sys.argv[1]
    hex_output = char_to_hex(input_char)
    if len(hex_output) == 2:
        print("00" + hex_output)
    else:
        print(hex_output)

