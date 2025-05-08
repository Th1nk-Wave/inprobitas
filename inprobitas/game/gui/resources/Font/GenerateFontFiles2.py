from PIL import Image, ImageDraw, ImageFont
import string
import os

def generate_font_files(font_path, output_dir, image_size=(100, 100), font_size=72):
    os.makedirs(output_dir, exist_ok=True)

    font = ImageFont.truetype(font_path, font_size)
    characters = string.ascii_letters + string.digits + string.punctuation + " "

    ascent, descent = font.getmetrics()
    total_height = ascent + descent

    for char in characters:
        # Measure character size and offset
        (left, top, right, bottom) = font.getbbox(char)
        char_width = right - left
        char_height = bottom - top

        # Create image for the character
        image = Image.new("L", image_size, "white")
        draw = ImageDraw.Draw(image)

        # Calculate x and y to align to baseline and center horizontally
        x = (image_size[0] - char_width) // 2 - left
        y = (image_size[1] - total_height) // 2 - top + ascent - char_height#+ ascent - top 

        draw.text((x, y), char, font=font, fill="black")

        # Crop to non-white area
        bbox = image.getbbox()
        if bbox is None:
            continue
        cropped = image.crop(bbox)

        # Resize proportionally if needed (optional, you can remove this step)
        cropped_width, cropped_height = cropped.size
        scale = min(image_size[0] / cropped_width, image_size[1] / cropped_height)
        resized = cropped.resize((int(cropped_width * scale), int(cropped_height * scale)), Image.LANCZOS)

        # Paste centered again into 100x100
        final_image = Image.new("L", image_size, "white")
        final_x = (image_size[0] - resized.width) // 2
        final_y = (image_size[1] - resized.height) // 2
        final_image.paste(resized, (final_x, final_y))

        # Convert to grayscale RLE
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
                    RLE_chunk.extend((255 - prev_value).to_bytes(1, "little"))
                repeated = 1
                prev_value = current_value
        if prev_value is not None:
            RLE_chunk.extend(repeated.to_bytes(4, "little"))
            RLE_chunk.extend((255 - prev_value).to_bytes(1, "little"))

        # Save to file
        output_file = os.path.join(output_dir, f"{ord(char)}.char")
        with open(output_file, "wb") as f:
            f.write(RLE_chunk)
        print(f"Saved {output_file}")

# Example usage
font_path = "D:/stuff thats in better drive/programming/c# programming/inprobitas/inprobitas/game/gui/resources/Font/Ace-Attourney/Ace-Attorney.ttf"
output_dir = "D:/stuff thats in better drive/programming/c# programming/inprobitas/inprobitas/game/gui/resources/Font/Ace-Attourney/Encoded/"
generate_font_files(font_path, output_dir)