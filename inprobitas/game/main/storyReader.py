import re

outputFile = open("proccessed.txt","w")

# Placeholder functions for specific actions
def on_npc_speak(character_name, dialogue):
    """
    Triggered when an NPC speaks.
    :param character_name: Name of the character speaking
    :param dialogue: List of dialogue lines
    """
    print(f"{character_name} says: {dialogue}")
    outputFile.write(f"{character_name} says: {dialogue}\n")

def on_scene_change():
    """
    Triggered when a scene change occurs (e.g., screen fades to black).
    """
    print("Scene Change Detected")
    outputFile.write("Scene Change Detected\n")

def on_option_box(action, option_text):
    """
    Triggered when options are given or when a selection is required.
    :param action: Action associated with the option (e.g., 'Examine signpost')
    :param option_text: Text associated with the option
    """
    print(f"Option Box for '{action}': {option_text}")
    outputFile.write(f"Option Box for '{action}': {option_text}\n")

def on_examine(action, description):
    """
    Triggered when an examination command is found.
    :param action: The object being examined
    :param description: The text that appears during examination
    """
    print(f"Examine '{action}': {description}")
    outputFile.write(f"Examine '{action}': {description}\n")

# Parsing function
def parse_script(file_path):
    with open(file_path, 'r', encoding='utf-8') as file:
        lines = file.readlines()

    character_dialogue = []
    current_character = None
    option_box_active = False
    option_text = []
    examine_active = False
    examine_text = []

    for line in lines:
        line = line.strip()

        # Check for a scene change marker (full line of hyphens)
        if re.match(r'^-+$', line):
            # Trigger the scene change function
            on_scene_change()
            continue

        # Check for option box markers (three-sided box of stars)
        if line.startswith("***") and line.endswith("***"):
            option_box_active = not option_box_active
            if not option_box_active:
                on_option_box("action_placeholder", option_text)
                option_text = []
            continue
        elif option_box_active:
            option_text.append(line)
            continue

        # Check for examination action
        if line.startswith("Examine"):
            if examine_active:
                on_examine(current_character, examine_text)
                examine_text = []
            examine_active = True
            current_character = line.split("Examine ")[1]
            continue
        elif examine_active:
            if line.startswith("*"):
                examine_text.append(line.strip("* "))
            else:
                examine_active = False
                on_examine(current_character, examine_text)
                examine_text = []

        # Match dialogue lines with character name tags
        match = re.match(r'^-(\w+)$', line)
        if match:
            if current_character and character_dialogue:
                # Call the NPC speak function with the previous character's dialogue
                on_npc_speak(current_character, character_dialogue)
                character_dialogue = []

            current_character = match.group(1)
            character_dialogue = []
        elif current_character:
            # Continue collecting dialogue lines
            character_dialogue.append(line)

    # Handle the last character's dialogue if still active
    if current_character and character_dialogue:
        on_npc_speak(current_character, character_dialogue)

# Example call to parse a script file
parse_script('script.txt')
