using static inprobitas.engine.Utility;
namespace inprobitas.engine.Files
{
    public static class Font
    {
        public static byte[] ReadChar(string FilePath)
        {
            byte[] buf = new byte[100 * 100]; Populate(buf, (byte)0);
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
                    byte alpha = reader.ReadByte();

                    for (int i = 0; i < repeatedCount; i++)
                    {
                        if (pixelIndex < buf.Length)
                        {
                            buf[pixelIndex] = alpha;
                            pixelIndex++;
                        }
                    }
                }
            }
            return buf;
        }
    }
}