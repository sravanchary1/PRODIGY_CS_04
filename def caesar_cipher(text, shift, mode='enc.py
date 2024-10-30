def caesar_cipher(text, shift, mode='encrypt'):
    result = ""
    shift = shift if mode == 'encrypt' else -shift
    
    # Process each character in the text
    for char in text:
        if char.isalpha():
            # Determine the starting ASCII code (A for uppercase, a for lowercase)
            start = ord('A') if char.isupper() else ord('a')
            # Shift character and wrap around within A-Z or a-z
            result += chr((ord(char) - start + shift) % 26 + start)
        else:
            # Non-alphabet characters remain the same
            result += char
            
    return result

def main():
    print("Caesar Cipher Program")
    
    while True:
        choice = input("\nChoose an option (1: Encrypt, 2: Decrypt, 3: Exit): ")
        
        if choice == '1' or choice == '2':
            text = input("Enter the text: ")
            shift = int(input("Enter the shift value: "))
            mode = 'encrypt' if choice == '1' else 'decrypt'
            
            output_text = caesar_cipher(text, shift, mode)
            print(f"Result: {output_text}")
        
        elif choice == '3':
            print("Exiting the program.")
            break
        
        else:
            print("Invalid choice. Please select 1, 2, or 3.")

# Run the program
main()
