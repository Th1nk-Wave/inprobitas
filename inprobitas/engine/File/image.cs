using static inprobitas.engine.Utility;
using inprobitas.engine.Graphics;

namespace inprobitas.engine.Files
{
    public static class Image
    {
        // decompresses images using runlength compression
        public static UInt32[] UnpackImage(string FilePath, int frameWidth, int frameHeight, bool alpha)
        {
            UInt32[] buf = new UInt32[frameWidth*frameHeight]; Populate(buf, 0u);
            if (!File.Exists(FilePath))
            {
                return buf;
            }
            using (BinaryReader reader = new BinaryReader(File.Open(FilePath, FileMode.Open)))
            {
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