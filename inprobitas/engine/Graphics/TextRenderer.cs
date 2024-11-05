using System.Drawing;
using static inprobitas.engine.Files.Font;
using static inprobitas.engine.Files.Utility;
namespace inprobitas.engine.Graphics
{
    public enum FontType
    {
        Comfortaa,
        nothing
    }
    static class FontLoader
    {
        static Dictionary<string, List<byte[]>> FontData = new Dictionary<string, List<byte[]>>();
        static Dictionary<string, Dictionary<int, List<byte[]>>> CharacterImages = new Dictionary<string, Dictionary<int, List<byte[]>>>();
        public static List<byte[]> LoadFont(string FontName, int Size)
        {
            if (!FontData.ContainsKey(FontName))
            {
                FontData[FontName] = new List<byte[]>();
                for (int c = 0; c < 126; c++)
                {
                    byte[] charDat = ReadChar(fontDirectory+"/"+FontName+"/Encoded/"+c+".char");
                    FontData[FontName].Insert(c,charDat);
                }
            }

            if (!CharacterImages.ContainsKey(FontName))
            {
                CharacterImages.Add(FontName, new Dictionary<int, List<byte[]>>());
            }

            if (!CharacterImages[FontName].ContainsKey(Size)) {
                CharacterImages[FontName][Size] = downscale(FontData[FontName], Size, Size);
            }

            return CharacterImages[FontName][Size];
        }
        private static byte[] downscale(byte[] originalImage, int targetWidth, int targetHeight)
        {
            byte[] downscaledImage = new byte[targetWidth * targetHeight];
            float scaleX = (float)100 / targetWidth;
            float scaleY = (float)100 / targetHeight;

            for (int y = 0; y < targetHeight; y++)
            {
                for (int x = 0; x < targetWidth; x++)
                {
                    int startX = (int)(x * scaleX);
                    int endX = (int)((x + 1) * scaleX);
                    int startY = (int)(y * scaleY);
                    int endY = (int)((y + 1) * scaleY);

                    int sum = 0;
                    int count = 0;
                    for (int originalY = startY; originalY < endY; originalY++)
                    {
                        for (int originalX = startX; originalX < endX; originalX++)
                        {
                            sum += originalImage[originalY * 100 + originalX];
                            count++;
                        }
                    }

                    downscaledImage[y * targetWidth + x] = (byte)(sum / count);
                }
            }
            return downscaledImage;
        }
        private static List<byte[]> downscale(List<byte[]> originalImages, int targetWidth, int targetHeight)
        {
            List<byte[]> downscaledImages = new List<byte[]>();
            foreach (byte[] image in originalImages)
            {
                byte[] downscaled = downscale(image, targetWidth, targetHeight);
                downscaledImages.Add(downscaled);
            }
            return downscaledImages;
        }
    }











    
    public class TextRenderer
    {
        public string text;
        public int FontSize;
        public string FontName;
        public Color TextColor;
        public TextRenderer(string FontName, int FontSize, Color TextColor, string text = "")
        {
            this.text = text;
            this.FontSize = FontSize;
            this.FontName = FontName;
            this.TextColor = TextColor;

        }

        public void Render(Window w, int topX,int topY, int bottemX, int bottemY)
        {
            List<byte[]> FontData = FontLoader.LoadFont(FontName,FontSize);
            int xpos = 0;
            int Ylevel = 0;
            foreach (char c in text.ToCharArray())
            {
                UInt32[] character = new UInt32[FontSize * FontSize];
                for (int pos = 0; pos < FontSize*FontSize; pos++)
                {
                    float alpha = (float)FontData[(int)c][pos];
                    character[pos] = new Color(TextColor.r,TextColor.g,TextColor.b,(byte)alpha).ToUint32();
                }

                if ((xpos * FontSize / 2 + topX) >= bottemX - FontSize) { xpos = 0; Ylevel++; }

                w.FillWithAt(character, (ushort)(xpos*FontSize/2 + topX), (ushort)(topY + Ylevel*FontSize), (ushort)(FontSize), (ushort)(FontSize));
                //w.SetPixel((ushort)(xpos * FontSize + topX), (ushort)(topY),new Color(255,0,0));
                xpos++;
            }
        }
    } 
}