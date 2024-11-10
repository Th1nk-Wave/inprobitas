using static inprobitas.engine.Utility;
using inprobitas.engine.Graphics;

namespace inprobitas.engine.Files
{
    public static class Image
    {
        // decompresses images using runlength compression
        public static UInt32[] UnpackImage(string FilePath, bool alpha)
        {
            
            if (!File.Exists(FilePath))
            {
                throw new FileNotFoundException("could not find file", FilePath);
            }
            UInt32[] buf;
            using (BinaryReader reader = new BinaryReader(File.Open(FilePath, FileMode.Open)))
            {
                Int16 FrameWidth = reader.ReadInt16();
                Int16 FrameHeight = reader.ReadInt16();

                buf = new UInt32[FrameWidth * FrameHeight]; Populate(buf, 0u);

                int pixelIndex = 0;
                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    int repeatedCount = reader.ReadInt32();

                    byte r = reader.ReadByte();
                    byte g = reader.ReadByte();
                    byte b = reader.ReadByte();
                    Graphics.Color packedColor = new Graphics.Color(r, g, b, 255);
                    if (alpha)
                    {
                        byte a = reader.ReadByte();
                        packedColor.a = a;
                    }

                    for (int i = 0; i < repeatedCount; i++)
                    {
                        if (pixelIndex < buf.Length)
                        {
                            buf[pixelIndex] = packedColor.ToUint32();
                            pixelIndex++;
                        }
                    }
                }
            }
            return buf;

        }


    }
}