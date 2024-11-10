using inprobitas.engine.Graphics;
using static inprobitas.engine.Files.Image;
using static inprobitas.engine.Files.Video;
using static inprobitas.engine.Files.Utility;
using static inprobitas.game.settings.GraphicsSettings;

namespace inprobitas.game.gui.menus
{
    internal class courtroomMenu
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

        public int AnimationFrame = 0;

        public courtroomMenu()
        {
            defenseBenchImageData = UnpackImage(projectDirectory + "/game/gui/resources/ace attorney/misc/Encoded/defense bench.bitmap", true);
            DefenseBenchImage = new Image(new UIdim(0, 0, 0f, 1f), new UIdim(198, 44, 0f, 0f), new UIdim(0, 0, 0f, 1f), 2, new UIdim(198, 44, 0f, 0f), defenseBenchImageData, new UIdim(198, 44, 0f, 0f));

            courtroomGUI = new GUI(Width, Height); courtroomGUI.Append(DefenseBenchImage);
            SetRoom("judgestand");
            SetCharacter("Phoenix Wright", "Confident_1");
        }

        public void SetRoom(string Name)
        {
            // Dereference everying beffore loading new image
            RoomImageData = null;
            if (RoomImage != null)
            {
                courtroomGUI.Remove(RoomImage);
                RoomImage.ImageData = null;
                RoomImage.ImageDataScaled = null;
                RoomImage.ImageDataPerFrame = null;
            }
            RoomImage = null;


            RoomImageData = UnpackImage(RoomDir + Name + ".bitmap", true);

            RoomImage = new Image(new UIdim(0,0,0f,0f), new UIdim(Width, Height, 0f, 0f), new UIdim(0, 0, 0f, 0f),0, new UIdim(Width, Height, 0f, 0f),RoomImageData, new UIdim(Width, Height, 0f, 0f));
            courtroomGUI.Append(RoomImage);
        }

        public void SetCharacter(string Name, string action)
        {
            // Dereference everything beffore loading new image
            CharacterAnimData = null;
            if (CharacterImage != null)
            {
                courtroomGUI.Remove(CharacterImage);
                CharacterImage.ImageData = null;
                CharacterImage.ImageDataScaled = null;
                CharacterImage.ImageDataPerFrame = null;
            }
            CharacterImage = null;


            CharacterAnimData = UnpackFrames(CharacterDir + Name + "/behind defense bench/Encoded/" + action + ".bitmap", true);

            CharacterImage = new Image(new UIdim(0, 0, 0f, 0f), new UIdim(Width, Height, 0f, 0f), new UIdim(0, 0, 0f, 0f), 1, new UIdim(Width, Height, 0f, 0f), CharacterAnimData[0], new UIdim(Width, Height, 0f, 0f));
            courtroomGUI.Append(CharacterImage);
        }

        public void Render(Window w)
        {
            //CharacterImage.ImageData = CharacterAnimData[AnimationFrame];
            //CharacterImage.GenerateScaledImage();
            //courtroomGUI.DecendTreeAndPlot(w);
            w.FillWithAt(RoomImageData,0,0,Width,Height);
            w.FillWithAt(CharacterAnimData[AnimationFrame],0,0,Width,Height);
            w.FillWithAt(defenseBenchImageData,0,(ushort)(Height-44),198,44);
            AnimationFrame++;
            if (AnimationFrame > CharacterAnimData.Count-1) { AnimationFrame = 0; }
        }

        public void Deref()
        {

        }
    }
}
