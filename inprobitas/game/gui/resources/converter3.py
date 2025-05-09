import cv2
import time
import os
import numpy as np
from PIL import Image as PILImage, ImageSequence  # Import PIL for GIF support

def downscale_and_extract_rgba(input_path, output_frame_size=(64, 48), output_file="output.txt", resize=True, white_is_transparent=False, transparent_color=(255, 255, 255)):
    # Check if input path is a video, image, or GIF file
    is_video = input_path.endswith(('.mp4', '.avi', '.mov'))
    is_image = input_path.endswith(('.jpg', '.jpeg', '.png', '.bmp', '.tiff'))
    is_gif = input_path.endswith('.gif')

    if not (is_video or is_image or is_gif):
        print("Error: Unsupported file type.")
        return

    cap = None
    frame_count = 1 if is_image or is_gif else None

    if is_video:
        cap = cv2.VideoCapture(input_path)
        if not cap.isOpened():
            print("Error: Could not open video.")
            return
        frame_count = int(cap.get(cv2.CAP_PROP_FRAME_COUNT))
        print(f"Total frames to process: {frame_count}")
    elif is_image:
        frame = cv2.imread(input_path, cv2.IMREAD_UNCHANGED)
        if frame is None:
            print("Error: Could not open image.")
            return
    elif is_gif:
        gif = PILImage.open(input_path)
        frames = [frame.convert("RGBA") for frame in ImageSequence.Iterator(gif)]
        frame_count = len(frames)
        print(f"Total frames to process: {frame_count}")

    current_frame = 0
    start = time.perf_counter()

    with open(output_file, "wb") as f:
        buffer = bytearray()

        while (cap.isOpened() if is_video else current_frame < frame_count):
            if is_video:
                ret, frame = cap.read()
                if not ret:
                    print("Finished processing all frames.")
                    break
                frame = cv2.cvtColor(frame, cv2.COLOR_BGR2BGRA)  # Convert to BGRA to handle transparency
            elif is_image:
                # For single images, this runs only once
                ret = True
            elif is_gif:
                frame = frames[current_frame]
                frame = cv2.cvtColor(np.array(frame), cv2.COLOR_RGBA2BGRA)
                ret = True

            # Resize the frame if needed
            if resize:
                frame = cv2.resize(frame, output_frame_size, interpolation=cv2.INTER_AREA)

            height, width, _ = frame.shape

            # Write width and height to output file as Uint16
            buffer.extend(width.to_bytes(2, "little"))
            buffer.extend(height.to_bytes(2, "little"))

            # Ensure frame has alpha channel
            if frame.shape[2] == 3:  # RGB only
                frame = cv2.cvtColor(frame, cv2.COLOR_BGR2BGRA)

            prev_rgba = None
            repeated = 0

            for y in range(height):
                for x in range(width):
                    b, g, r, a = frame[y, x]
                    current_rgba = (int(r), int(g), int(b), int(a))

                    if white_is_transparent:
                        if (int(r) == transparent_color[0] and int(g) == transparent_color[1] and int(b) == transparent_color[2]):
                            current_rgba = (int(r), int(g), int(b), 0)

                    if current_rgba == prev_rgba:
                        repeated += 1
                    else:
                        if prev_rgba is not None:
                            buffer.extend(repeated.to_bytes(4, "little"))
                            buffer.extend(prev_rgba[0].to_bytes(1, "little"))
                            buffer.extend(prev_rgba[1].to_bytes(1, "little"))
                            buffer.extend(prev_rgba[2].to_bytes(1, "little"))
                            buffer.extend(prev_rgba[3].to_bytes(1, "little"))

                        repeated = 1
                        prev_rgba = current_rgba

            # Write the final run-length data for the last color in frame
            if prev_rgba is not None:
                buffer.extend(repeated.to_bytes(4, "little"))
                buffer.extend(prev_rgba[0].to_bytes(1, "little"))
                buffer.extend(prev_rgba[1].to_bytes(1, "little"))
                buffer.extend(prev_rgba[2].to_bytes(1, "little"))
                buffer.extend(prev_rgba[3].to_bytes(1, "little"))

            f.write(buffer)
            buffer.clear()

            end = time.perf_counter()
            if (end - start) > 1:
                print(f"Processed frame {current_frame + 1}/{frame_count}")
                start = time.perf_counter()

            current_frame += 1
            if not is_video and not is_gif:
                break  # Only process once for single images

    if is_video:
        cap.release()

    print("Processing complete.")

# Example usage
output_size = (256, 192)

def convert_all_files(folder_path, output_folder, resize=True):
    for root, dirs, files in os.walk(folder_path):
        for file in files:
            filepath = os.path.join(root, file)
            print(f"opening {filepath}")
            downscale_and_extract_rgba(filepath, output_size, output_folder + "/" + os.path.splitext(file)[0] + ".bitmap", resize=resize)




"""
output_folder = "ace attorney/scenes/scene resources/Encoded"
folder_path = 'ace attorney/scenes/scene resources'
convert_all_files(folder_path,output_folder,False)
downscale_and_extract_rgba(folder_path + "/the-thinker.png",(141,204),output_folder+"/the-thinker.bitmap",False,True,(0,255,255))

output_folder = "ace attorney/rooms/Encoded"
folder_path = 'ace attorney/rooms'
convert_all_files(folder_path,output_folder,False)

output_folder = "ace attorney/misc/Encoded"
folder_path = 'ace attorney/misc'
convert_all_files(folder_path,output_folder,False)
downscale_and_extract_rgba(folder_path + "/defense bench.png",(141,204),output_folder+"/defense bench.bitmap",False,True,(255,255,255))

output_folder = "ace attorney/characters/Phoenix Wright/behind defense bench/Encoded"
folder_path = 'ace attorney/characters/Phoenix Wright/behind defense bench'
convert_all_files(folder_path,output_folder,False)
"""

output_folder = "ace attorney/characters/Payne/Encoded"
folder_path = 'ace attorney/characters/Payne'
convert_all_files(folder_path,output_folder,False)
