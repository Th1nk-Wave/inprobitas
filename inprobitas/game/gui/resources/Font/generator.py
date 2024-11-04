from PIL import Image, ImageDraw, ImageFont
import string
import os
import re

def sanitize_filename(char):
    # Replace or remove invalid characters from the filename
    return re.sub(r'[<>:"/\\|?*]', '_', char)

def generate_font_images(font_path, output_dir, image_size=(100, 100), font_size=72):
    # Ensure the output directory exists
    os.makedirs(output_dir, exist_ok=True)
    
    # Load the font
    font = ImageFont.truetype(font_path, font_size)
    
    # Define character set: uppercase, lowercase, digits, and some symbols
    characters = string.ascii_letters + string.digits + string.punctuation + " "
    
    for char in characters:
        # Create a blank image with a white background
        image = Image.new("RGB", image_size, "white")
        draw = ImageDraw.Draw(image)
        
        # Calculate the position to center the character
        bbox = draw.textbbox((0, 0), char, font=font)
        text_width, text_height = bbox[2] - bbox[0], bbox[3] - bbox[1]
        position = ((image_size[0] - text_width) // 2, (image_size[1] - text_height) // 2)
        

        
        # Draw the character onto the image
        draw.text(position, char, font=font, fill="black")
        
        # Sanitize the character for the filename
        sanitized_char = sanitize_filename(char)
        output_path = os.path.join(output_dir, f"{ord(char):03}_{sanitized_char}.png")
        
        # Save the image
        image.save(output_path)
        print(f"Saved {output_path}")


# Usage
font_path = "Comfortaa/Comfortaa_Regular.ttf"  # Update with your font path
output_dir = "Comfortaa/Images"
generate_font_images(font_path, output_dir)