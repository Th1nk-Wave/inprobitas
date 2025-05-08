using inprobitas.engine.Graphics;

namespace inprobitas.engine.Files
{
    static class Utility
    {
        public static string workingDirectory = Environment.CurrentDirectory;
        public static string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
        public static string fontDirectory = projectDirectory+"/game/gui/resources/Font";
        public static string imageDirectory = projectDirectory + "/game/gui/resources/Image";
        public static string videoDirectory = projectDirectory + "/game/gui/resources/Video";

        public static UIdim GetImageDimensions(string FilePath)
        {
            UInt16 FrameWidth;
            UInt16 FrameHeight;
            using (BinaryReader reader = new BinaryReader(File.Open(FilePath, FileMode.Open)))
            {
                FrameWidth = reader.ReadUInt16();
                FrameHeight = reader.ReadUInt16();
            }
            return new UIdim(FrameWidth, FrameHeight,0f,0f);
        }
    }
}