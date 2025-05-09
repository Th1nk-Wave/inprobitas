using inprobitas.engine.Graphics;
using static inprobitas.engine.Files.Image;
using static inprobitas.engine.Files.Video;
using static inprobitas.engine.Files.Utility;
using static inprobitas.game.settings.GraphicsSettings;
using System;
using System.Diagnostics;

namespace inprobitas.game.gui.menus
{
    public class courtroomMenu
    {
        public UInt32[] RoomImageData;
        public Image RoomImage;

        public List<UInt32[]> CharacterAnimData;
        public Image CharacterImage;

        public UInt32[] defenseBenchImageData;
        public Image DefenseBenchImage;


        string RoomDir = projectDirectory + "/game/gui/resources/ace attorney/rooms/Encoded/";
        string CharacterDir = projectDirectory + "/game/gui/resources/ace attorney/characters/";


        public GUI courtroomGUI;

        private Frame CullingFrame;

        private Frame TextBox;
        private Frame SpeakerBox;

        private Frame Text1Align;
        private Frame Text2Align;
        private Frame Text3Align;
        private Frame SpeakerAlign;



        public Text Text1;
        public Text Text2;
        public Text Text3;
        private Text Speaker;

        private Background TransparentBG;
        private Background BlueBG;
        private Background CullBG;
        private Border TextBoxBoarder;

        public int AnimationFrame = 0;

        public courtroomMenu()
        {
            defenseBenchImageData = UnpackImage(projectDirectory + "/game/gui/resources/ace attorney/misc/Encoded/defense bench.bitmap", true);
            DefenseBenchImage = new Image(new UIdim(0, 0, 0f, 1f), new UIdim(198, 44, 0f, 0f), new UIdim(0, 0, 0f, 1f), -999, new UIdim(198, 44, 0f, 0f), defenseBenchImageData, new UIdim(198, 44, 0f, 0f));

            courtroomGUI = new GUI(Width, Height); courtroomGUI.Append(DefenseBenchImage);
            CullingFrame = new Frame(new UIdim(0, 0, 0f, 0f), new UIdim(Width, Height, 0f, 0f), new UIdim(0, 0, 0f, 0f), -1);
            CullBG = new Background(new Color(0, 0, 0)); CullingFrame.Append(CullBG);
            courtroomGUI.Append(CullingFrame);
            

            TransparentBG = new Background(new Color(0, 0, 0, 150));
            TextBoxBoarder = new Border(new Color(254, 255, 255, 255));
            BlueBG = new Background(new Color(50, 50, 255, 150));

            TextBox = new Frame(new UIdim(2, -2, 0f, 1f), new UIdim(Width - 4, -4, 0f, 0.30f), new UIdim(0, 0, 0f, 1f), -999);
            SpeakerBox = new Frame(new UIdim(2, 2, 0f, 0.7f), new UIdim(50, 10, 0f, 0f), new UIdim(0, 0, 0f, 1f), -999);

            Text1Align = new Frame(new UIdim(0, 0, 0.03f, 0f), new UIdim(0, 0, 0.97f, 0.33f), new UIdim(0, 0, 0f, 0f), 1000);
            Text2Align = new Frame(new UIdim(0, 0, 0.03f, 0.33f), new UIdim(0, 0, 0.97f, 0.33f), new UIdim(0, 0, 0f, 0f), 1000);
            Text3Align = new Frame(new UIdim(0, 0, 0.03f, 0.66f), new UIdim(0, 0, 0.97f, 0.33f), new UIdim(0, 0, 0f, 0f), 1000);
            SpeakerAlign = new Frame(new UIdim(0, 0, 0.03f, 0f), new UIdim(0, 0, 0.97f, 1f), new UIdim(0, 0, 0f, 0f), 1000);

            Text1 = new Text("", new Color(255, 255, 255), 15, "Ace-Attourney");
            Text2 = new Text("", new Color(255, 255, 255), 15, "Ace-Attourney");
            Text3 = new Text("", new Color(255, 255, 255), 15, "Ace-Attourney");
            Speaker = new Text("", new Color(255, 255, 255), 10, "Ace-Attourney");

            Text1Align.Append(Text1);
            Text2Align.Append(Text2);
            Text3Align.Append(Text3);
            SpeakerAlign.Append(Speaker);

            TextBox.Append(Text1Align);
            TextBox.Append(Text2Align);
            TextBox.Append(Text3Align);
            SpeakerBox.Append(SpeakerAlign);

            TextBox.Append(TextBoxBoarder);
            TextBox.Append(TransparentBG);

            SpeakerBox.Append(BlueBG);
            SpeakerBox.Append(TextBoxBoarder);

            courtroomGUI.Append(TextBox);
            courtroomGUI.Append(SpeakerBox);

            CharacterImage = new Image(new UIdim(0, 0, 0f, 0f), new UIdim(0, 0, 0f, 0f), new UIdim(0, 0, 0f, 0f), 2, new UIdim(0, 0, 0f, 0f), new uint[] { }, new UIdim(0, 0, 0f, 0f));
            RoomImage = new Image(new UIdim(0, 0, 0f, 0f), new UIdim(0, 0, 0f, 0f), new UIdim(0, 0, 0f, 0f), 1, new UIdim(0, 0, 0f, 0f), new uint[] { }, new UIdim(0, 0, 0f, 0f));
            
            CharacterAnimData = new List<uint[]> { new uint[] { 0 } };

            courtroomGUI.Append(RoomImage);
            courtroomGUI.Append(CharacterImage);
            

        }

        public void SetRoom(string Name)
        {
            if (Name == "none")
            {
                RoomImage.ImageData = new uint[] { };
                RoomImage.ImageSize = new UIdim(0, 0, 0f, 0f);
                RoomImage.ImageRealSize = new UIdim(0, 0, 0f, 0f);
                RoomImage.ImageDataScaled = new uint[Width * Height];
                RoomImage.size = new UIdim(0, 0, 0f, 0f);
                RoomImage.GenerateScaledImage();
                return;
            }


            RoomImageData = UnpackImage(RoomDir + Name + ".bitmap", true);
            RoomImage.ImageData = RoomImageData;
            RoomImage.ImageSize = new UIdim(Width, Height, 0f, 0f);
            RoomImage.ImageRealSize = new UIdim(Width, Height, 0f, 0f);
            RoomImage.ImageDataScaled = new uint[Width * Height];
            RoomImage.size = new UIdim(Width, Height, 0f, 0f);
            RoomImage.GenerateScaledImage();
        }

        public void SetCharacter(string Name, string action)
        {
            if (Name == "none")
            {
                CharacterImage.ImageData = new uint[] { };
                CharacterImage.ImageSize = new UIdim(0, 0, 0f, 0f);
                CharacterImage.ImageRealSize = new UIdim(0, 0, 0f, 0f);
                CharacterImage.ImageDataScaled = new uint[Width * Height];
                CharacterImage.size = new UIdim(0, 0, 0f, 0f);
                CharacterImage.GenerateScaledImage();
                return;
            }
            if (action == "none")
            {
                return;
            }

            CharacterAnimData = UnpackFrames(CharacterDir + Name + "/Encoded/" + action + ".bitmap", true);
            AnimationFrame = 0;
            CharacterImage.ImageData = CharacterAnimData[0];
            CharacterImage.ImageSize = new UIdim(Width, Height, 0f, 0f);
            CharacterImage.ImageRealSize = new UIdim(Width, Height, 0f, 0f);
            CharacterImage.ImageDataScaled = new uint[Width * Height];
            CharacterImage.size = new UIdim(Width, Height, 0f, 0f);
            CharacterImage.GenerateScaledImage();
        }
        public void SetSpeaker(string Name)
        {
            Speaker.Content = Name.Substring(0, Math.Min(Name.Length, 10));
        }
        public void SetText1(string text)
        {
            Text1.Content = text;
        }
        public void SetText2(string text)
        {
            Text2.Content = text;
        }
        public void SetText3(string text)
        {
            Text3.Content = text;
        }
        public void SetTextColor(Color col)
        {
            Text1.TextColor = col;
            Text2.TextColor = col;
            Text3.TextColor = col;
        }
        public void ShowBench()
        {
            DefenseBenchImage.ZIndex = 3;
        }
        public void HideBench()
        {
            DefenseBenchImage.ZIndex = -999;
        }
        public void ShowText()
        {
            TextBox.ZIndex = 100;
            SpeakerBox.ZIndex = 101;
        }
        public void HideText()
        {
            TextBox.ZIndex = -999;
            SpeakerBox.ZIndex = -999;
        }

        public void Render(Window w)
        {
            CharacterImage.ImageData = CharacterAnimData[AnimationFrame];
            CharacterImage.GenerateScaledImage();
            courtroomGUI.DecendTreeAndPlot(w);
            //w.FillWithAt(RoomImageData,0,0,Width,Height);
            //w.FillWithAt(CharacterAnimData[AnimationFrame],0,0,Width,Height);
            //w.FillWithAt(defenseBenchImageData,0,(ushort)(Height-44),198,44);
            AnimationFrame++;
            if (AnimationFrame > CharacterAnimData.Count-1) { AnimationFrame = 0; }
        }

        public void Deref()
        {

        }
    }
}
