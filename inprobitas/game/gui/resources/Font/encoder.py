from PIL import Image
import os

def run_length_encode(data):
    """Encodes the pixel data using run-length encoding."""
    encoded_data = bytearray()
    prev_pixel = data[0]
    count = 1
    
    for pixel in data[1:]:
        if pixel == prev_pixel:
            count += 1
        else:
            # Append the count and the pixel value as bytes
            encoded_data.append(count)
            encoded_data.append(prev_pixel)
            prev_pixel = pixel
            count = 1
    # Append the last run
    encoded_data.append(count)
    encoded_data.append(prev_pixel)
    
    return encoded_data

def extract_and_encode_pixels(image_dir,output_dir):
    """Extracts pixels from images, applies RLE, and writes to separate binary files."""
    for image_name in os.listdir(image_dir):
        image_path = os.path.join(image_dir, image_name)
        
        # Open the image and convert it to grayscale
        image = Image.open(image_path).convert("L")  # Convert to grayscale
        pixels = list(image.getdata())  # Get pixels as a list of grayscale values
        
        # Run-length encode the pixel data
        encoded_pixels = run_length_encode(pixels)

        # Create a binary output file for the encoded data
        output_file = os.path.splitext(image_name)[0] + "_encoded.rle"
        with open(output_dir+"/image_name", "wb") as f:
            f.write(encoded_pixels)
            print(f"Processed {image_name} into {output_file}.")

        

# Usage
image_dir = "Comfortaa/Images"  # Directory containing images
output_dir = "Comfortaa/Encoded"   # Output file for encoded pixel data
extract_and_encode_pixels(image_dir, output_dir)