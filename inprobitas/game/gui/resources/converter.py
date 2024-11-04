import cv2
import time
import os

unique_colors = 255

def downscale_and_extract_rgb(input_path, output_frame_size=(64, 48), repeated_bytes_size=2, output_file="output.txt"):
    # Check if input path is a video or an image file
    is_video = input_path.endswith(('.mp4', '.avi', '.mov'))  # Add any other video file extensions if needed
    is_image = input_path.endswith(('.jpg', '.jpeg', '.png', '.bmp', '.tiff'))  # Add other image file extensions as needed

    if not is_video and not is_image:
        print("Error: Unsupported file type.")
        return

    # Initialize variables for video or image processing
    cap = None
    frame_count = 1 if is_image else None

    if is_video:
        cap = cv2.VideoCapture(input_path)
        # Check if video opened successfully
        if not cap.isOpened():
            print("Error: Could not open video.")
            return
        frame_count = int(cap.get(cv2.CAP_PROP_FRAME_COUNT))
        print(f"Total frames to process: {frame_count}")
    else:
        frame = cv2.imread(input_path)
        if frame is None:
            print("Error: Could not open image.")
            return

    current_frame = 0
    start = time.perf_counter()

    with open(output_file, "wb") as f:
        buffer = bytearray()
        
        while (cap.isOpened() if is_video else current_frame < 1):
            if is_video:
                ret, frame = cap.read()  # Read a frame from video
                if not ret:
                    print("Finished processing all frames.")
                    break
            else:
                # If processing an image, no need to read new frames
                ret = True

            # Resize frame to the specified size
            resized_frame = cv2.resize(frame, output_frame_size, interpolation=cv2.INTER_AREA)

            # Loop through each pixel in the resized frame and extract RGB values
            height, width, _ = resized_frame.shape
            prevRGB = None
            repeated = 0

            for y in range(height):
                for x in range(width):
                    # Get the RGB value of the pixel
                    b, g, r = resized_frame[y, x]  # OpenCV uses BGR by default
                    currentRGB = (r, g, b)

                    # Normalize and handle repetitions
                    normalized = tuple((int(c) * unique_colors // 255) for c in currentRGB)

                    if currentRGB == prevRGB:
                        repeated += 1
                    else:
                        # Write the previous data if a new color is found
                        if prevRGB is not None:
                            buffer.extend(repeated.to_bytes(4, "little"))
                            buffer.extend(prevRGB[0].to_bytes(1, "little"))
                            buffer.extend(prevRGB[1].to_bytes(1, "little"))
                            buffer.extend(prevRGB[2].to_bytes(1, "little"))

                        # Reset repeated count and update previous RGB
                        repeated = 1
                        prevRGB = normalized

            # Write any remaining repeated data
            if prevRGB is not None:
                buffer.extend(repeated.to_bytes(4, "little"))
                buffer.extend(prevRGB[0].to_bytes(1, "little"))
                buffer.extend(prevRGB[1].to_bytes(1, "little"))
                buffer.extend(prevRGB[2].to_bytes(1, "little"))

            f.write(buffer)
            buffer.clear()

            end = time.perf_counter()
            if (end - start) > 1:
                print(f"Processed frame {current_frame + 1}/{frame_count}")
                start = time.perf_counter()

            current_frame += 1
            if not is_video:
                break  # Only process once for an image

    # Release the video capture object if a video was processed
    if is_video:
        cap.release()

    print("Processing complete.")

# Example usage
input_file = 'titlescreenBG.png'  # or 'test.jpg' for an image file
output_file = "titleScreenBG.bitmap"
res = 15
output_size = (8 * res, 6 * res)
downscale_and_extract_rgb(input_file, output_size, output_file=output_file)
