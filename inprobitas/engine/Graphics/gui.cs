namespace inprobitas.engine.Graphics
{
    public struct UIdim
    {
        public int pixelX;
        public int pixelY;
        public float percentX;
        public float percentY;


        // utility functions
        public int getAbsoluteX(int parentWidth)
        {
            return pixelX + (int)(parentWidth * percentX);
        }
        public int getAbsoluteY(int parentHeight)
        {
            return pixelY + (int)(parentHeight * percentY);
        }
        public UIdim getAbsolute(int parentWidth, int parentHeight)
        {
            return new UIdim(getAbsoluteX(parentWidth), getAbsoluteY(parentHeight),0f,0f);
        }

        // constructors
        public UIdim(int pixelX, int pixelY, float percentX, float percentY)
        {
            this.pixelX = pixelX;
            this.pixelY = pixelY;
            this.percentX = percentX;
            this.percentY = percentY;
        }

        public UIdim(int pixelX, float percentX, int pixelY, float percentY)
        {
            this.pixelX = pixelX;
            this.pixelY = pixelY;
            this.percentX = percentX;
            this.percentY = percentY;
        }





        // arithmatic operators
        public static UIdim operator +(UIdim a, UIdim b) => new UIdim(a.pixelX + b.pixelX, a.pixelY + b.pixelY, a.percentX + b.percentX, a.percentY + b.percentY);
        public static UIdim operator -(UIdim a, UIdim b) => new UIdim(a.pixelX - b.pixelX, a.pixelY - b.pixelY, a.percentX - b.percentX, a.percentY - b.percentY);
        public static UIdim operator *(UIdim a, UIdim b) => new UIdim(a.pixelX * b.pixelX, a.pixelY * b.pixelY, a.percentX * b.percentX, a.percentY * b.percentY);
        public static UIdim operator /(UIdim a, UIdim b) => new UIdim(a.pixelX / b.pixelX, a.pixelY / b.pixelY, a.percentX / b.percentX, a.percentY / b.percentY);
        public static UIdim operator *(float a, UIdim b) => new UIdim((int)(a * b.pixelX), (int)(a * b.pixelY), a * b.percentX, a * b.percentY);
        public static UIdim operator /(float a, UIdim b) => new UIdim((int)(a / b.pixelX), (int)(a / b.pixelY), a / b.percentX, a / b.percentY);
        public static UIdim operator *(UIdim b, float a) => new UIdim((int)(a * b.pixelX), (int)(a * b.pixelY), a * b.percentX, a * b.percentY);
        public static UIdim operator /(UIdim b, float a) => new UIdim((int)(a / b.pixelX), (int)(a / b.pixelY), a / b.percentX, a / b.percentY);
        public static UIdim operator *(int a, UIdim b) => new UIdim(a * b.pixelX, a * b.pixelY, a * b.percentX, a * b.percentY);
        public static UIdim operator /(int a, UIdim b) => new UIdim(a / b.pixelX, a / b.pixelY, a / b.percentX, a / b.percentY);
        public static UIdim operator *(UIdim b, int a) => new UIdim(a * b.pixelX, a * b.pixelY, a * b.percentX, a * b.percentY);
        public static UIdim operator /(UIdim b, int a) => new UIdim(a / b.pixelX, a / b.pixelY, a / b.percentX, a / b.percentY);
        public static UIdim operator -(UIdim a) => new UIdim(-a.pixelX, -a.pixelY, -a.percentX, -a.percentY);


        // comparrison operators
        public static bool operator ==(UIdim a, UIdim b) => a.pixelX == b.pixelX && a.pixelY == b.pixelX && a.percentX == b.percentX && a.percentY == b.percentY;
        public static bool operator !=(UIdim a, UIdim b) => !(a == b);


        // who even uses this
        public object this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return this.pixelX;
                    case 1: return this.pixelX;
                    case 2: return this.pixelX;
                    case 3: return this.pixelX;
                    default: throw new ArgumentOutOfRangeException(nameof(index), index, "who the fuck even gets members of a struct by indexing it like a list\n its slower to use the square brackets you know ;)");
                }
            }
            set
            {
                switch (index)
                {
                    default: throw new Exception("no.. im not implementing that, i dont care if you like setting members of a struct using the square brakets. how about you try implementing it yourself at 2am.");
                }
            }
        }
    }

    public abstract class GuiModification
    {
        public abstract void Beffore(Window w, in GuiElement element, UIdim AbsolutePosition, UIdim AbsoluteSize);
        public abstract void After(Window w, in GuiElement element, UIdim AbsolutePosition, UIdim AbsoluteSize);
    }
    public abstract class GuiElement
    {
        public UIdim position;
        public UIdim size;
        public UIdim anchor;
        public int ZIndex = 0;
        public List<GuiElement> children;
        public List<GuiModification> modifications;

        public void Append(GuiElement element)
        {
            children.Add(element);
        }
        public void Remove(GuiElement element)
        {
            children.Remove(element);
        }
        public void Append(GuiModification Mod)
        {
            modifications.Add(Mod);
        }
        public void Remove(GuiModification Mod)
        {
            modifications.Remove(Mod);
        }
        

        public List<GuiElement> GetChildren(bool sorted = true)
        {
            if (children == null) { return null; }
            if (sorted) { return children.OrderBy(c => c.ZIndex).ToList(); }
            return children;
        }

        public abstract void Draw(Window w, UIdim AbsolutePosition, UIdim AbsoluteSize, UIdim Anchor);
        private protected abstract void ApplyBefforeModifications(Window w, UIdim AbsolutePosition, UIdim AbsoluteSize);
        private protected abstract void ApplyAfterModifications(Window w, UIdim AbsolutePosition, UIdim AbsoluteSize);




        public GuiElement(UIdim position, UIdim size, UIdim anchor, int ZIndex, List<GuiElement> children = null, List<GuiModification> modifications = null)
        {
            this.position = position;
            this.size = size;
            this.anchor = anchor;
            this.ZIndex = ZIndex;

            if (children == null) { this.children = new List<GuiElement>(); } else { this.children = children; }
            if (modifications == null) { this.modifications = new List<GuiModification>(); } else { this.modifications = modifications; }
        }
    }

    public class Frame : GuiElement
    {
        //List<FrameModification> modifications;
        public Frame(UIdim position, UIdim size, UIdim anchor, int ZIndex, List<GuiElement> children = null, List<FrameModification> modifications = null)
            : base(position, size, anchor, ZIndex,children) // implements default constructor behaviour
        {
            // extra constructor behaviour if needed
            //if (modifications == null) { this.modifications = new List<FrameModification>(); } else { this.modifications = modifications; }
        }

        

        private protected override void ApplyBefforeModifications(Window w, UIdim TopLeftCorner, UIdim BottemRightCorner)
        {
            if (modifications == null) { return; }
            foreach (FrameModification modification in modifications)
            {
                modification.Beffore(w, this, TopLeftCorner, BottemRightCorner);
            }
        }
        private protected override void ApplyAfterModifications(Window w, UIdim TopLeftCorner, UIdim BottemRightCorner)
        {
            if (modifications == null) { return; }
            foreach (FrameModification modification in modifications)
            {
                modification.After(w, this, TopLeftCorner, BottemRightCorner);
            }
        }
        public override void Draw(Window w, UIdim AbsolutePosition, UIdim AbsoluteSize, UIdim Anchor)
        {
            UIdim TopLeftCorner = AbsolutePosition - Anchor;
            UIdim BottemRightCorner = TopLeftCorner + AbsoluteSize;
            ApplyBefforeModifications(w, TopLeftCorner, BottemRightCorner);
            ApplyAfterModifications(w,TopLeftCorner, BottemRightCorner);
        }
    }

    public abstract class FrameModification : GuiModification
    {
        public abstract override void Beffore(Window w, in GuiElement element, UIdim TopLeftCorner, UIdim BottemRightCorner);
    }

    public class Background : FrameModification
    {
        public Color BackgroundColor;
        public Background(Color BackgroundColor)
        {
            this.BackgroundColor = BackgroundColor;
        }
        public override void Beffore(Window w, in GuiElement element, UIdim TopLeftCorner, UIdim BottemRightCorner)
        {
            w.Box((ushort)TopLeftCorner.pixelX, (ushort)TopLeftCorner.pixelY, (ushort)BottemRightCorner.pixelX, (ushort)BottemRightCorner.pixelY,BackgroundColor);
        }
        public override void After(Window w, in GuiElement element, UIdim TopLeftCorner, UIdim BottemRightCorner)
        {
            
        }
    }

    public class Border : FrameModification
    {
        public Color BorderColor;
        public Border(Color BorderColor)
        {
            this.BorderColor = BorderColor;
        }
        public override void Beffore(Window w, in GuiElement element, UIdim TopLeftCorner, UIdim BottemRightCorner)
        {
            // Left border
            w.VerticalLine(
                (ushort)(TopLeftCorner.pixelX - 1),               // X position (left of TopLeftCorner)
                (ushort)(TopLeftCorner.pixelY - 1),               // Start Y position
                (ushort)((BottemRightCorner.pixelY - TopLeftCorner.pixelY) + 2), // Height of the border
                BorderColor
            );

            // Right border
            w.VerticalLine(
                (ushort)(BottemRightCorner.pixelX),               // X position (right of BottemRightCorner)
                (ushort)(TopLeftCorner.pixelY - 1),               // Start Y position
                (ushort)((BottemRightCorner.pixelY - TopLeftCorner.pixelY) + 2), // Height of the border
                BorderColor
            );

            // Top border
            w.HorizontalLine(
                (ushort)(TopLeftCorner.pixelX - 1),               // Start X position
                (ushort)(TopLeftCorner.pixelY - 1),               // Y position (above TopLeftCorner)
                (ushort)((BottemRightCorner.pixelX - TopLeftCorner.pixelX) + 2), // Width of the border
                BorderColor
            );

            // Bottom border
            w.HorizontalLine(
                (ushort)(TopLeftCorner.pixelX - 1),               // Start X position
                (ushort)(BottemRightCorner.pixelY),               // Y position (below BottemRightCorner)
                (ushort)((BottemRightCorner.pixelX - TopLeftCorner.pixelX) + 2), // Width of the border
                BorderColor
            );
        }
        public override void After(Window w, in GuiElement element, UIdim TopLeftCorner, UIdim BottemRightCorner)
        {

        }
    }

    public class Text : FrameModification
    {
        public string FontName;
        public TextRenderer selfRenderer;
        public Color TextColor { get => selfRenderer.TextColor; set => selfRenderer.TextColor = value; }
        public string Content { get => selfRenderer.text; set => selfRenderer.text = value; }
        public int FontSize { get => selfRenderer.FontSize; set => selfRenderer.FontSize = value; }
        public Text(string text,Color TextColor,int TextSize,string FontName)
        {
            this.FontName = FontName;
            this.selfRenderer = new TextRenderer(FontName,TextSize,TextColor,text);
        }
        public override void Beffore(Window w, in GuiElement element, UIdim TopLeftCorner, UIdim BottemRightCorner)
        {

        }
        public override void After(Window w, in GuiElement element, UIdim TopLeftCorner, UIdim BottemRightCorner)
        {
            selfRenderer.Render(w,TopLeftCorner.pixelX,TopLeftCorner.pixelY,BottemRightCorner.pixelX,BottemRightCorner.pixelY);
        }
    }

    public class Image : GuiElement
    {
        List<FrameModification> modifications;
        private List<string> BakedFrame;
        public UIdim ImageSize;
        public UIdim ImageRealSize;
        public UInt32[] ImageData;
        public UInt32[] ImageDataScaled;
        public UInt32[] ImageDataPerFrame;

        private protected override void ApplyBefforeModifications(Window w, UIdim TopLeftCorner, UIdim BottemRightCorner)
        {
            if (modifications == null) { return; }
            foreach (FrameModification modification in modifications)
            {
                if (modification is ImageModification) { continue; }
                modification.Beffore(w, this, TopLeftCorner, BottemRightCorner);
            }
        }
        private protected override void ApplyAfterModifications(Window w, UIdim TopLeftCorner, UIdim BottemRightCorner)
        {
            if (modifications == null) { return; }
            foreach (FrameModification modification in modifications)
            {
                if (modification is ImageModification) { continue; }
                modification.After(w, this, TopLeftCorner, BottemRightCorner);
            }
        }

        public Image(UIdim position, UIdim size, UIdim anchor, int ZIndex, UIdim ImageSize, UInt32[] ImageData, UIdim ImageRealSize, List<GuiElement> children = null, List<FrameModification> modifications = null)
            : base(position, size, anchor, ZIndex,children) // implements default constructor behaviour
        {
            this.ImageData = ImageData;
            this.ImageRealSize = ImageRealSize;
            this.ImageSize = ImageSize;

            this.ImageDataScaled = new UInt32[ImageSize.pixelX * ImageSize.pixelY];

            GenerateScaledImage();

            if (modifications == null) { this.modifications = new List<FrameModification>(); } else { this.modifications = modifications; }
        }
        private UIdim lastSize;
        public override void Draw(Window w, UIdim AbsolutePosition, UIdim AbsoluteSize, UIdim Anchor)
        {
            UIdim TopLeftCorner = AbsolutePosition - Anchor;
            UIdim BottemRightCorner = TopLeftCorner + AbsoluteSize;
            UIdim scale = new UIdim(0,0, (float)AbsoluteSize.pixelX / (float)ImageSize.pixelX, (float)AbsoluteSize.pixelY / (float)ImageSize.pixelY);

            float scaleAmount = (scale.percentX + scale.percentY) / 2;

            UIdim scaledImageSize = new UIdim((int)(ImageSize.pixelX * scaleAmount), (int)(ImageSize.pixelY * scaleAmount),0f,0f);
            if (scaledImageSize != lastSize)
            {
                GeneratePerFrameImage(scaledImageSize);
            }
            lastSize = scaledImageSize;
            w.FillWithAt(ImageDataPerFrame, (ushort)TopLeftCorner.pixelX,(ushort)TopLeftCorner.pixelY, (ushort)scaledImageSize.pixelX, (ushort)scaledImageSize.pixelY);


        }

        public void GenerateScaledImage()
        {
            for (ushort X = 0; X < ImageSize.pixelX; X++)
            {
                for (ushort Y = 0; Y < ImageSize.pixelY; Y++)
                {
                    ushort ScaledX = (ushort)((X * ImageRealSize.pixelX) / ImageSize.pixelX);
                    ushort ScaledY = (ushort)((Y * ImageRealSize.pixelY) / ImageSize.pixelY);

                    UInt32 rgb = ImageData[ScaledX + ScaledY * ImageRealSize.pixelX];
                    ImageDataScaled[X + Y * ImageSize.pixelX] = rgb;
                }
            }
        }
        public void GeneratePerFrameImage(UIdim scaledImageSize)
        {
            ImageDataPerFrame = new UInt32[scaledImageSize.pixelX * scaledImageSize.pixelY];
            for (ushort X = 0; X < scaledImageSize.pixelX; X++)
            {
                for (ushort Y = 0; Y < scaledImageSize.pixelY; Y++)
                {
                    ushort ScaledX = (ushort)((X * ImageSize.pixelX) / scaledImageSize.pixelX);
                    ushort ScaledY = (ushort)((Y * ImageSize.pixelY) / scaledImageSize.pixelY);

                    UInt32 rgb = ImageDataScaled[ScaledX + ScaledY * ImageSize.pixelX];
                    ImageDataPerFrame[X + Y * scaledImageSize.pixelX] = rgb;
                }
            }
        }

        public void BakeImage()
        {
            BakedFrame = Window.BakeFrameDynamic(ImageDataScaled, (ushort)ImageSize.pixelX, (ushort)ImageSize.pixelY);
        }
    }

    public abstract class ImageModification : FrameModification
    {
        public abstract override void Beffore(Window w, in GuiElement element, UIdim TopLeftCorner, UIdim BottemRightCorner);
    }


    public class GUI
    {
        public UInt16 ViewportWidth;
        public UInt16 ViewportHeight;
        public List<GuiElement> Children;
        public GUI(UInt16 ViewportWidth, UInt16 ViewportHeight, List<GuiElement> Children = null)
        {
            this.ViewportWidth = ViewportWidth;
            this.ViewportHeight = ViewportHeight;
            if (Children == null) { this.Children = new List<GuiElement>(); } else { this.Children = Children; }
        }

        public void Append(GuiElement element)
        {
            Children.Add(element);
        }

        public void Remove(GuiElement element)
        {
            Children.Remove(element);
        }

        public List<GuiElement> GetChildren(bool sorted = true)
        {
            if (Children == null) { return null; }
            if (sorted) { return Children.OrderBy(c => c.ZIndex).ToList(); }
            return Children;
        }


        public void DecendTreeAndPlot(Window w)
        {
            void inner(GuiElement Parent, UIdim ParentPosition, UIdim ParentSize)
            {
                foreach (GuiElement Child in Parent.GetChildren())
                {
                    UIdim AbsolutePos = (Child.position + ParentPosition).getAbsolute(ParentSize.pixelX, ParentSize.pixelY);
                    UIdim AbsoluteSize = Child.size.getAbsolute(ParentSize.pixelX,ParentSize.pixelY);
                    UIdim Anchor = Child.anchor.getAbsolute(AbsoluteSize.pixelX, AbsoluteSize.pixelY);

                    Child.Draw(w, AbsolutePos, AbsoluteSize, Anchor);
                    inner(Child, AbsolutePos - Anchor, AbsoluteSize);
                }
            }
            foreach (GuiElement Child in GetChildren())
            {
                UIdim AbsolutePos = Child.position.getAbsolute(ViewportWidth, ViewportHeight);
                UIdim AbsoluteSize = Child.size.getAbsolute(ViewportWidth, ViewportHeight);
                UIdim Anchor = Child.anchor.getAbsolute(AbsoluteSize.pixelX, AbsoluteSize.pixelY);

                Child.Draw(w,AbsolutePos, AbsoluteSize, Anchor);
                inner(Child,AbsolutePos - Anchor, AbsoluteSize);
            }
        }
    }
}
