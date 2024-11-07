using System.Runtime.CompilerServices;
using static inprobitas.engine.API.Console;
using static inprobitas.engine.Utility;

using inprobitas.engine.Graphics;
using System.Text;
using System.Net.NetworkInformation;
using System.Drawing;

namespace inprobitas.engine.Graphics
{
    public struct Color
    {
        public byte r, g, b, a;
        public Color(byte r, byte g, byte b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = 255;
        }
        public Color(byte r, byte g, byte b, byte a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }
        public static Color FromUint32(UInt32 uint32)
        {
            return new Color(
                (byte)(uint32 >> 8),      // Extract red component
                (byte)(uint32 >> 16),     // Extract green component
                (byte)(uint32 >> 24),     // Extract blue component
                (byte)(uint32)            // Extract alpha component
            );
        }

        public Color Blend(Color col2)
        {
            float alpha = (float)col2.a / (float)255;
            return new Color(
                (byte)(((float)col2.r * alpha) + ((float)r * (float)(1 - alpha))),
                (byte)(((float)col2.g * alpha) + ((float)g * (float)(1 - alpha))),
                (byte)(((float)col2.b * alpha) + ((float)b * (float)(1 - alpha)))
            );
        }

        public UInt32 ToUint32()
        {
            return ((UInt32)this.r << 8) | ((UInt32)this.g << 16) | ((UInt32)this.b << 24) | (UInt32)this.a;
        }

        public static UInt32 BlendColors(UInt32 bgColor, UInt32 fgColor)
        {
            // Extract components of the background color
            byte bgR = (byte)(bgColor >> 8);
            byte bgG = (byte)(bgColor >> 16);
            byte bgB = (byte)(bgColor >> 24);
            byte bgA = (byte)(bgColor); // Not really needed if we assume bgA is opaque (255)

            // Extract components of the foreground color
            byte fgR = (byte)(fgColor >> 8);
            byte fgG = (byte)(fgColor >> 16);
            byte fgB = (byte)(fgColor >> 24);
            byte fgA = (byte)(fgColor);

            // Calculate alpha as a fraction
            float alpha = fgA / 255f;

            // Perform alpha blending for each color channel
            byte blendedR = (byte)((fgR * alpha) + (bgR * (1 - alpha)));
            byte blendedG = (byte)((fgG * alpha) + (bgG * (1 - alpha)));
            byte blendedB = (byte)((fgB * alpha) + (bgB * (1 - alpha)));

            // Assemble the blended color into a single UInt32 value with bgA as the final alpha
            return ((UInt32)blendedB << 24) | ((UInt32)blendedG << 16) | ((UInt32)blendedR << 8) | bgA;
        }
    }
    public class Window
    {
        private UInt32[] ColorBuffer;
        private UInt32[] LastFrame;
        private string optiRend;
        public int _CompressionFactor;

        private UInt32[] CharColorBuffer;
        private char[] CharBuffer;
        private Boolean[] LineUpdates;
        private Boolean[] NeedRender;
        private string[] RenderStrings;
        private int UpdateComplexity;
        private Stack<UInt16> UpdateStack;

        private UInt16 _Width = 100;
        private UInt16 _Height = 100;

        private IntPtr Hwindow;
        private IntPtr FrontBuffer;
        private IntPtr BackBuffer;

        public UInt16 Width
        {
            get { return _Width; }
            set { _Width = value; }
        }
        public UInt16 Height
        {
            get { return _Height; }
            set { _Height = value; }
        }

        public Window(UInt16 Width, UInt16 Height, Int16 FontSize, int CompressionFactor)
        {
            _Width = Width;
            _Height = Height;

            ColorBuffer = new UInt32[Width * Height]; Populate(ColorBuffer, 1000000u);
            LastFrame = new UInt32[Width * Height]; Populate(LastFrame, 0u);
            _CompressionFactor = CompressionFactor;
            //PixelUpdates = new bool[Width * Height]; Populate(PixelUpdates,false);

            //PixelEdits = new byte[Width * Height]; Populate(PixelEdits, (byte)0);
            //LastFramePixelEdits = new byte[Width * Height]; Populate(PixelEdits, (byte)0);

            CharColorBuffer = new UInt32[Width * Height * 2]; Populate(CharColorBuffer, 0u);
            CharBuffer = new char[Width * Height * 2]; Populate(CharBuffer, ' ');
            LineUpdates = new Boolean[Height]; Populate(LineUpdates, false);
            NeedRender = new Boolean[Height]; Populate(NeedRender, false);
            RenderStrings = new string[Height]; Populate(RenderStrings, "render string not calculated for this line");
            UpdateComplexity = 0;
            UpdateStack = new Stack<UInt16>(Height);

            //get std handle
            Hwindow = GetStdHandle(STD_OUTPUT_HANDLE);
            //create screen buffers
            FrontBuffer = CreateConsoleScreenBuffer(
                GENERIC_READ | GENERIC_WRITE,
                FILE_SHARE_READ | FILE_SHARE_WRITE,
                0,
                CONSOLE_TEXTMODE_BUFFER,
                0
            );
            BackBuffer = CreateConsoleScreenBuffer(
                GENERIC_READ | GENERIC_WRITE,
                FILE_SHARE_READ | FILE_SHARE_WRITE,
                0,
                CONSOLE_TEXTMODE_BUFFER,
                0
            );

            //set virtual
            SetVirtual(Hwindow);

            //set font to tiny (for per-pixel console writing)
            SetCurrentFont(Hwindow, "Consolas", FontSize);
            SetWindowSize(Hwindow, Width, Height);
            


        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetPixel(int x, int y, Color col)
        {
            // check if pixel is out of bounds
            if (x < 0 || y < 0) { return; }
            if (x >= _Width || y >= _Height) { return; }

            // prepack color
            UInt32 packedColor = col.ToUint32();

            // check if color is overwriting a duplicate
            if (packedColor == ColorBuffer[x + y * _Width]) {  return; }

            // color is (in bounds, is writing a different value)

            // avoid expensive blending if a color is not transparrent
            if (col.a < 255)
            {
                if (col.a > 4)
                {
                    ColorBuffer[x + y * _Width] = Color.BlendColors(ColorBuffer[x + y * _Width], packedColor);
                }
            } else
            {
                ColorBuffer[x + y * _Width] = col.ToUint32();
            }
            
            // finally, signal that this pixel has changed and thus needs to be rendered
            LineUpdates[y] = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetPixelInBounds(int x, int y, Color col) // same as SetPixel but with less checks
        {
            // prepack color
            UInt32 packedColor = col.ToUint32();

            // check if color is overwriting a duplicate
            if (packedColor == ColorBuffer[x + y * _Width]) { return; }

            // color is (in bounds, is writing a different value)

            // avoid expensive blending if a color is not transparrent
            if (col.a < 255)
            {
                if (col.a > 4)
                {
                    ColorBuffer[x + y * _Width] = Color.BlendColors(ColorBuffer[x + y * _Width], packedColor);
                }
            }
            else
            {
                ColorBuffer[x + y * _Width] = col.ToUint32();
            }

            // finally, signal that this pixel has changed and thus needs to be rendered
            LineUpdates[y] = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetPixel(int x, int y, UInt32 col)
        {
            // check if pixel is out of bounds
            if (x < 0 || y < 0) { return; }
            if (x >= _Width || y >= _Height) { return; }

            // check if color is overwriting a duplicate
            if (col == ColorBuffer[x + y * _Width]) { return; }

            // color is (in bounds, is writing a different value)

            // avoid expensive blending if a color is not transparrent
            if ((byte)col < 255)
            {
                if ((byte)col > 4)
                {
                    ColorBuffer[x + y * _Width] = Color.BlendColors(ColorBuffer[x + y * _Width], col);
                }
            }
            else
            {
                ColorBuffer[x + y * _Width] = col;
            }

            // finally, signal that this pixel has changed and thus needs to be rendered
            LineUpdates[y] = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetPixelInBounds(int x, int y, UInt32 col) // same as SetPixel but with less checks
        {
            // check if color is overwriting a duplicate
            //if (col == ColorBuffer[x + y * _Width]) { return; }
            //if (col == LastFrame[x + y * _Width]){}

            // color is (in bounds, is writing a different value)

            // avoid expensive blending if a color is not transparrent
            if ((byte)col < 255)
            {
                if ((byte)col > 4)
                {
                    ColorBuffer[x + y * _Width] = Color.BlendColors(ColorBuffer[x + y * _Width], col);
                } else
                {
                    return;
                }
            }
            else
            {
                ColorBuffer[x + y * _Width] = col;
            }

            // finally, signal that this pixel has changed and thus needs to be rendered
            LineUpdates[y] = true;
        }
        public Color GetPixel(UInt16 x, UInt16 y)
        {
            return Color.FromUint32(ColorBuffer[x + y*_Width]);
        }

        public void Fill(Color col)
        {
            Populate(ColorBuffer,col.ToUint32());
            Populate(LineUpdates,true);
        }

        public void FillWith(UInt32[] frame)
        {
            frame.CopyTo(ColorBuffer,0);
            Populate(LineUpdates, true);
        }

        public void FillWithAt(UInt32[] frame, int X, int Y, UInt16 Width, UInt16 Height)
        {
            for (int y = 0; y < Height; y++)
            {
                if (y+Y >= _Height || y+Y < 0) { continue; }
                for (int x = 0; x < Width; x++)
                {
                    if (x+X >= _Width || x+X < 0) { continue; }

                    //SetPixelInBounds(x+X,y+Y, (UInt32)frame[x + y*Width]);

                    UInt32 col = frame[x + y * Width];
                    if ((byte)col < 255)
                    {
                        if ((byte)col > 4)
                        {
                            ColorBuffer[(x+X) + (y+Y) * _Width] = Color.BlendColors(ColorBuffer[(x + X) + (y + Y) * _Width], col);
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        ColorBuffer[(x + X) + (y + Y) * _Width] = col;
                    }

                    // finally, signal that this pixel has changed and thus needs to be rendered
                    LineUpdates[y+Y] = true;
                }
            }
        }

        public void Box(UInt16 X1, UInt16 Y1, UInt16 X2, UInt16 Y2, Color col)
        {
            short StepX; if (X2 > X1) { StepX = 1; } else { StepX = -1; }
            short StepY; if (Y2 > Y1) { StepY = 1; } else { StepY = -1; }
            UInt32 colUInt32 = col.ToUint32();
            for (short Y = (short)Y1; Y != Y2; Y+=StepY)
            {
                if (Y >= _Height || Y < 0) { continue; }
                for (short X = (short)X1; X != X2; X+=StepX)
                {
                    if (X >= _Width || X < 0) { continue; }
                    if (col.a < 255)
                    {
                        if (col.a > 4)
                        {
                            ColorBuffer[X + Y * _Width] = Color.BlendColors(ColorBuffer[X + Y * _Width], col.ToUint32());
                        }
                    }
                    else
                    {
                        ColorBuffer[X + Y * _Width] = col.ToUint32();
                    }
                }
                LineUpdates[Y] = true;
            }
        }

        public void VerticalLine(UInt16 X, UInt16 Y, UInt16 Length, Color col)
        {
            if (X > _Width || X < 0) { return; }
            UInt32 colUInt32 = col.ToUint32();
            for (UInt16 ypos = Y; ypos < Y+Length; ypos++)
            {
                if (ypos > _Height || ypos < 0) { continue;}
                ColorBuffer[X + ypos * _Width] = colUInt32;
                LineUpdates[ypos] = true;
            }
        }

        public void HorizontalLine(UInt16 X, UInt16 Y, UInt16 Length, Color col)
        {
            if (Y > _Height || Y < 0) { return; }
            UInt32 colUInt32 = col.ToUint32();
            LineUpdates[Y] = true;
            for (UInt16 xpos = X; xpos < X + Length; xpos++)
            {
                if (xpos > _Width || xpos < 0) { continue; }
                ColorBuffer[xpos + Y * _Width] = colUInt32;
            }
        }



        public void ProcessGUI(GUI gui)
        {
            gui.DecendTreeAndPlot(this);
        }

        public void Update_old()
        {
            
            for (UInt16 ypos = 0; ypos < _Height; ypos++)
            {
                UInt32 oldrgb = 0u;
                UInt32 oldTextrgb = 0u;
                if (!LineUpdates[ypos]) { continue; }

                string lineString = "";
                for (UInt16 xpos = 0; xpos < _Width; xpos++)
                {
                    string charString = "";
                    UInt32 currentTextrgb = 0u;

                    UInt32 textrgb1 = CharColorBuffer[xpos*2 + ypos * _Width*2];
                    char char1 = CharBuffer[xpos*2 + ypos * _Width*2];
                    if ((textrgb1!=0u) && (textrgb1 != oldTextrgb))
                    {
                        charString += $"\x1b[38;2;{(byte)(textrgb1 >> 8)};{(byte)(textrgb1 >> 16)};{(byte)(textrgb1 >> 24)}m";
                        currentTextrgb = textrgb1;
                    } else
                    {
                        currentTextrgb = oldTextrgb;
                    }
                    charString += char1;

                    UInt32 textrgb2 = CharColorBuffer[(xpos * 2) + 1 + ypos * _Width*2];
                    char char2 = CharBuffer[(xpos * 2) + 1 + ypos * _Width*2];
                    if ((textrgb2 != 0u) && (textrgb1 != textrgb2))
                    {
                        charString += $"\x1b[38;2;{(byte)(textrgb2 >> 8)};{(byte)(textrgb2 >> 16)};{(byte)(textrgb2 >> 24)}m";
                        currentTextrgb = textrgb2;
                    }
                    charString += char2;

                    UInt32 rgb = ColorBuffer[xpos + ypos * _Width];
                    if (rgb == oldrgb)
                    {
                        lineString += charString;
                    } else
                    {
                        lineString += $"\x1b[48;2;{(byte)(rgb >> 8)};{(byte)(rgb >> 16)};{(byte)(rgb >> 24)}m{charString}";
                    }
                    oldrgb = rgb;
                    oldTextrgb = currentTextrgb;
                }
                RenderStrings[ypos] = lineString;
                LineUpdates[ypos] = false;
                NeedRender[ypos] = true;


            }
        }

        private int renderer_PerLineEscapeSequenceOffset = "\x1b[;0H".Length + (int)Math.Floor(Math.Log10(1 + (int)UInt16.MaxValue));

        public void Update_full()
        {
            for (UInt16 ypos = 0; ypos < _Height; ypos++)
            {
                UInt32 oldrgb = UInt32.MaxValue;
                if (!LineUpdates[ypos]) { continue; }

                StringBuilder lineSTR = new StringBuilder(_Width * 2 + 500);
                UInt16 blankCount = 0;

                for (UInt16 xpos = 0; xpos < _Width; xpos++)
                {
                    UInt32 rgb = ColorBuffer[xpos + ypos * _Width];
                    if (rgb == oldrgb)
                    {
                        blankCount += 1;
                    }
                    else
                    {
                        if (blankCount > 0)
                        {
                            //lineSTR += new string(' ', blankCount * 2);
                            lineSTR.Append( new StringBuilder("  ".Length * blankCount).Insert(0, "  ", blankCount));
                            blankCount = 0;
                        }

                        lineSTR.Append($"\x1b[48;2;{(byte)(rgb >> 8)};{(byte)(rgb >> 16)};{(byte)(rgb >> 24)}m  ");
                    }
                    oldrgb = rgb;
                }
                if (blankCount > 0)
                {
                    //lineSTR += new string(' ', blankCount * 2);
                    lineSTR.Append(new StringBuilder("  ".Length * blankCount).Insert(0, "  ", blankCount));
                    blankCount = 0;
                }

                if (NeedRender[ypos])
                {
                    UpdateComplexity -= RenderStrings[ypos].Length;
                    UpdateComplexity += lineSTR.Length;
                }
                else
                {
                    UpdateComplexity += lineSTR.Length + renderer_PerLineEscapeSequenceOffset;
                    UpdateStack.Push(ypos);
                }

                RenderStrings[ypos] = lineSTR.ToString();
                LineUpdates[ypos] = false;
                NeedRender[ypos] = true;
            }
        }

        /*
        public void Update_optimise3()
        {
            optiRend = "";
            UInt32 OldCol = UInt32.MaxValue;
            int Blanks = 0;
            StringBuilder RenderSTR = new StringBuilder();
            bool jumping = false;
            for (UInt16 ypos = 0; ypos < _Height; ypos++)
            {
                for (UInt16 xpos = 0; xpos < _Width; xpos++)
                {
                    // check if this pixel has been changed recently
                    bool NeedsUpdate = PixelUpdates[xpos + ypos * _Width];
                    if (NeedsUpdate)
                    {
                        UInt32 col = ColorBuffer[xpos + ypos * _Width];

                        if (jumping)
                        {
                            if (Blanks > 0) // deal with any ghost pixels
                            {
                                RenderSTR.Append(' ', Blanks * 2);
                                Blanks = 0;
                            }
                            RenderSTR.Append($"\x1b[{ypos + 1};{xpos * 2 + 1}H"); // handle jumping
                            jumping = false;
                        }

                        // if the pixel is the same color as the one beffore it
                        if (col == OldCol)
                        {
                            Blanks++;
                        } else // this is a new color now so we have to add color change command
                        {
                            if (Blanks > 0) // deal with any blanks first
                            {
                                RenderSTR.Append(' ', Blanks * 2);
                                Blanks = 0;
                            }
                            // now you can start a new color chunk
                            RenderSTR.Append($"\x1b[48;2;{(byte)(col >> 8)};{(byte)(col >> 16)};{(byte)(col >> 24)}m  ");
                        }
                        OldCol = col; // pass on current color to next pixel old col check

                        PixelUpdates[xpos + ypos * _Width] = false; // this pixel has been successfully updated


                    } else // if we ever jump over a pixel, (we might still have some blanks stored up that havn't been written yet)
                    {
                        UInt32 col = ColorBuffer[xpos + ypos * _Width];
                        if (col == OldCol)
                        {
                            // ghost pixel logic
                            Blanks++;
                            OldCol = col; // carry on oldCol to next ghost pixel
                            jumping = false;
                        }
                        else
                        {
                            // handle blanks beffore current pixel
                            if (Blanks > 0)
                            {
                                RenderSTR.Append(' ', Blanks * 2);
                                Blanks = 0; // we have successfully delt with the blanks
                            }
                            jumping = true; // we just jumped over a pixel so this flag is enabled so we know that we need to add a cursor setpos command later
                        }
                    }
                }
            }

            if (Blanks > 0) // deal with any blanks still left
            {
                RenderSTR.Append(' ', Blanks * 2);
                Blanks = 0;
            }

            RenderSTR.Append($"\x1b[{_Height + 1};0H"); // finally, tell console to return the cursor to the bottem
            optiRend = RenderSTR.ToString();
        }
        */
        public bool RGBisclose(UInt32 col1, UInt32 col2)
        {
            // Extract components of the background color
            byte c1R = (byte)(col1 >> 8);
            byte c1G = (byte)(col1 >> 16);
            byte c1B = (byte)(col1 >> 24);

            // Extract components of the foreground color
            byte c2R = (byte)(col2 >> 8);
            byte c2G = (byte)(col2 >> 16);
            byte c2B = (byte)(col2 >> 24);

            bool Rclose = Math.Abs((int)c1R - (int)c2R) <= _CompressionFactor;
            bool Gclose = Math.Abs((int)c1G - (int)c2G) <= _CompressionFactor;
            bool Bclose = Math.Abs((int)c1B - (int)c2B) <= _CompressionFactor;
            return (Rclose && Gclose && Bclose);
        }
        public void Update()
        {
            optiRend = "";
            UInt32 OldCol = UInt32.MaxValue;
            int Blanks = 0;
            StringBuilder RenderSTR = new StringBuilder();
            bool jumping = true;

            for (UInt16 ypos = 0; ypos < _Height; ypos++)
            {
                RenderSTR.Append($"\x1b[{ypos + 1};0H");
                for (UInt16 xpos = 0; xpos < _Width; xpos++)
                {
                    UInt32 col = ColorBuffer[xpos + ypos * _Width];
                    UInt32 LastFcol = LastFrame[xpos + ypos * _Width];
                    bool NeedsUpdate = col != LastFcol;


                    if (NeedsUpdate)
                    {
                        if (jumping)
                        {
                            RenderSTR.Append($"\x1b[{ypos + 1};{xpos * 2 + 1}H");
                            jumping = false;
                        }

                        if ((col == OldCol || RGBisclose(col, OldCol)) && !jumping)
                        {
                            RenderSTR.Append(' ',2);
                        } else
                        {
                            RenderSTR.Append($"\x1b[48;2;{(byte)(col >> 8)};{(byte)(col >> 16)};{(byte)(col >> 24)}m  ");
                            OldCol = col;
                        }
                        
                    }
                    else
                    {
                        jumping = true;
                    }
                }
            }
            //RenderSTR.Append($"\x1b[{_Height + 1};0H");
            optiRend = RenderSTR.ToString();
            ColorBuffer.CopyTo(LastFrame, 0);
        }


        public void Render()
        {
            uint charsWritten = 0;
            nint reserved = 0;
            WriteConsole(Hwindow, optiRend, (uint)optiRend.Length, out charsWritten, reserved);
        }


        private int renderer_FinalSuffixEscapeSequenceOffset = "\x1b[;0H".Length + (int)Math.Floor(Math.Log10(1 + (int)UInt16.MaxValue));
        public void Render_full()
        {

            StringBuilder renderSTR = new StringBuilder(UpdateComplexity + renderer_FinalSuffixEscapeSequenceOffset);
            UpdateComplexity = 0;
            if (UpdateStack.Count <= 0) { return; }
            do
            {
                UInt16 updateRow = UpdateStack.Pop();
                renderSTR.Append($"\x1b[{updateRow + 1};0H{RenderStrings[updateRow]}");
                NeedRender[updateRow] = false;
            } while (UpdateStack.Count > 0);
            renderSTR.Append($"\x1b[{_Height + 1};0H");
            uint charsWritten = 0;
            nint reserved = 0;
            WriteConsole(Hwindow, renderSTR.ToString(), (uint)renderSTR.Length, out charsWritten, reserved);
        }

        public List<string> BakeFramesFixed(List<UInt32[]> frames, UInt16 frameWidth, UInt16 frameHeight, UInt16 posX, UInt16 posY)
        {
            List<string> bakedFrames = new List<string>();
            foreach (UInt32[] frame in frames)
            {
                UInt32 oldrgb = UInt32.MaxValue;
                UInt16 blankCount = 0;
                StringBuilder renderSTR = new StringBuilder(frameWidth * 2 * frameHeight + 100* frameHeight);
                for (UInt16 ypos = 0; ypos < frameHeight; ypos++)
                {

                    renderSTR.Append($"\x1b[{posY + 1 + ypos};{posX + 1}H");

                    for (UInt16 xpos = 0; xpos < frameWidth; xpos++)
                    {
                        UInt32 rgb = frame[xpos + ypos * frameWidth];
                        if (rgb == oldrgb)
                        {
                            blankCount += 1;
                        }
                        else
                        {
                            if (blankCount > 0)
                            {
                                renderSTR.Append(new StringBuilder("  ".Length * blankCount).Insert(0, "  ", blankCount));
                                blankCount = 0;
                            }

                            renderSTR.Append($"\x1b[48;2;{(byte)(rgb >> 8)};{(byte)(rgb >> 16)};{(byte)(rgb >> 24)}m  ");
                        }
                        oldrgb = rgb;
                    }
                    if (blankCount > 0)
                    { 
                        renderSTR.Append(new StringBuilder("  ".Length * blankCount).Insert(0, "  ", blankCount));
                        blankCount = 0;
                    }
                }
                renderSTR.Append($"\x1b[{_Height + 1};0H");
                bakedFrames.Add(renderSTR.ToString());
            }
            return bakedFrames;
        }

        public static List<string[]> BakeFramesDynamic(List<UInt32[]> frames, UInt16 frameWidth, UInt16 frameHeight)
        {
            List<string[]> bakedFrames = new List<string[]>();
            foreach (UInt32[] frame in frames)
            {
                string[] bakedFrame = new string[frameHeight];
                UInt32 oldrgb = UInt32.MaxValue;
                UInt16 blankCount = 0;
                for (UInt16 ypos = 0; ypos < frameHeight; ypos++)
                {
                    StringBuilder renderSTR = new StringBuilder(frameWidth * 2 * frameHeight + 100 * frameHeight);
                    for (UInt16 xpos = 0; xpos < frameWidth; xpos++)
                    {
                        UInt32 rgb = frame[xpos + ypos * frameWidth];
                        if (rgb == oldrgb)
                        {
                            blankCount += 1;
                        }
                        else
                        {
                            if (blankCount > 0)
                            {
                                renderSTR.Append(new StringBuilder("  ".Length * blankCount).Insert(0, "  ", blankCount));
                                blankCount = 0;
                            }

                            renderSTR.Append($"\x1b[48;2;{(byte)(rgb >> 8)};{(byte)(rgb >> 16)};{(byte)(rgb >> 24)}m  ");
                        }
                        oldrgb = rgb;
                    }
                    if (blankCount > 0)
                    {
                        renderSTR.Append(new StringBuilder("  ".Length * blankCount).Insert(0, "  ", blankCount));
                        blankCount = 0;
                    }
                    bakedFrame[ypos] = renderSTR.ToString();
                }
                bakedFrames.Add(bakedFrame);
            }
            return bakedFrames;
        }

        public static List<string> BakeFrameDynamic(UInt32[] frame, UInt16 frameWidth, UInt16 frameHeight)
        {
            List<string> bakedFrame = new List<string>();
            UInt32 oldrgb = UInt32.MaxValue;
            UInt16 blankCount = 0;
            for (UInt16 ypos = 0; ypos < frameHeight; ypos++)
            {
                StringBuilder renderSTR = new StringBuilder(frameWidth * 2 * frameHeight + 100 * frameHeight);
                for (UInt16 xpos = 0; xpos < frameWidth; xpos++)
                {
                    UInt32 rgb = frame[xpos + ypos * frameWidth];
                    if (rgb == oldrgb)
                    {
                        blankCount += 1;
                    }
                    else
                    {
                        if (blankCount > 0)
                        {
                            renderSTR.Append(new StringBuilder("  ".Length * blankCount).Insert(0, "  ", blankCount));
                            blankCount = 0;
                        }

                        renderSTR.Append($"\x1b[48;2;{(byte)(rgb >> 8)};{(byte)(rgb >> 16)};{(byte)(rgb >> 24)}m  ");
                    }
                    oldrgb = rgb;
                }
                if (blankCount > 0)
                {
                    renderSTR.Append(new StringBuilder("  ".Length * blankCount).Insert(0, "  ", blankCount));
                    blankCount = 0;
                }
                bakedFrame[ypos] = renderSTR.ToString();
            }
            return bakedFrame;
        }

        public void RenderFixedBakedFrame(string baked_frame)
        {
            uint charsWritten = 0;
            nint reserved = 0;
            WriteConsole(Hwindow, baked_frame, (uint)baked_frame.Length, out charsWritten, reserved);
        }
    }
}
