from PIL import Image, ImageDraw, ImageFont
import string
import os

def generate_font_files(font_path, output_dir, image_size=(100, 100), font_size=72):
    os.makedirs(output_dir, exist_ok=True)

    font = ImageFont.truetype(font_path, font_size)
    characters = string.ascii_letters + string.digits + string.punctuation + " "
    
    # First pass: Determine baseline by measuring bounding boxes
    max_ascent, max_descent = 0, 0
    for char in characters:
        image = Image.new("L", image_size, "white")
        draw = ImageDraw.Draw(image)
        bbox = draw.textbbox((0, 0), char, font=font)
        
        if bbox:
            max_ascent = max(max_ascent, -bbox[1])  # distance from top to baseline
            max_descent = max(max_descent, bbox[3])  # distance from baseline to bottom

    # Baseline adjustment to center vertically in `100x100`
    vertical_adjustment = (image_size[1] - (max_ascent + max_descent)) // 2

    for char in characters:
        # Create a blank image for each character
        image = Image.new("L", image_size, "white")
        draw = ImageDraw.Draw(image)
        
        # Draw character and crop to bounding box to remove whitespace
        draw.text((0, 0), char, font=font, fill="black")
        bbox = image.getbbox()
        if bbox:
            cropped_image = image.crop(bbox)
        else:
            continue

        # Resize cropped image to fit within 100x100, keeping aspect ratio
        cropped_width, cropped_height = cropped_image.size
        scale = min(image_size[0] / cropped_width, image_size[1] / cropped_height)
        resized_image = cropped_image.resize((int(cropped_width * scale), int(cropped_height * scale)), Image.LANCZOS)

        # Create final 100x100 image and paste the resized character
        final_image = Image.new("L", image_size, "white")
        
        # Calculate position for centered character
        final_x = (image_size[0] - resized_image.width) // 2
        final_y = vertical_adjustment - max_ascent
        final_image.paste(resized_image, (final_x, final_y))
        
        # Save the image as grayscale Run-Length Encoded (RLE) data
        pixel_data = list(final_image.getdata())
        RLE_chunk = bytearray()
        
        prev_value = None
        repeated = 0
        for pixel in pixel_data:
            current_value = pixel
            if current_value == prev_value:
                repeated += 1
            else:
                if prev_value is not None:
                    RLE_chunk.extend(repeated.to_bytes(4, "little"))
                    RLE_chunk.extend((255-prev_value).to_bytes(1, "little"))
                repeated = 1
                prev_value = current_value

        # Write remaining data
        if prev_value is not None:
            RLE_chunk.extend(repeated.to_bytes(4, "little"))
            RLE_chunk.extend((255-prev_value).to_bytes(1, "little"))

        # Save each character's RLE encoded data to its own file
        output_file = os.path.join(output_dir, f"{ord(char)}.char")
        with open(output_file, "wb") as f:
            f.write(RLE_chunk)

        print(f"Saved {output_file}")

# Usage
font_path = "Comfortaa/Comfortaa_Regular.ttf"
output_dir = "Comfortaa/Encoded"
generate_font_files(font_path, output_dir)
