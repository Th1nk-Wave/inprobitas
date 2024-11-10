namespace inprobitas.engine.Files
{
    public static class Video
    {
        // decompresses frames using runlength compression
        public static List<UInt32[]> UnpackFrames(string filePath, bool alpha)
        {
            List<UInt32[]> AllFrames = new List<UInt32[]>();

            using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
            {
                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    // Create buffer for current frame
                    UInt16 FrameWidth = reader.ReadUInt16();
                    UInt16 FrameHeight = reader.ReadUInt16();
                    UInt32[] frameBuffer = new UInt32[FrameWidth * FrameHeight];
                    int pixelIndex = 0;

                    while (pixelIndex < frameBuffer.Length)
                    {
                        // Read repeated count (4 bytes)
                        int repeatedCount = reader.ReadInt32();


                        // Read RGB values (3 bytes)
                        byte r = reader.ReadByte();
                        byte g = reader.ReadByte();
                        byte b = reader.ReadByte();

                        Graphics.Color packedColor = new Graphics.Color(r, g, b, 255);
                        if (alpha)
                        {
                            byte a = reader.ReadByte();
                            packedColor.a = a;
                        }

                        

                        // Fill frame buffer with repeated colors
                        for (int i = 0; i < repeatedCount; i++)
                        {
                            if (pixelIndex < frameBuffer.Length)
                            {
                                frameBuffer[pixelIndex] = packedColor.ToUint32();
                                pixelIndex++;
                            }
                        }
                    }

                    // Add unpacked frame list
                    AllFrames.Add(frameBuffer);
                }
            }

            return AllFrames;

        }


    }
}