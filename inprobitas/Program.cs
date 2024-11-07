using System.Runtime.CompilerServices;
using inprobitas.engine.Graphics;
using static inprobitas.engine.Files.Utility;
using static inprobitas.engine.Files.Video;
using static inprobitas.engine.Files.Image;
using static inprobitas.game.settings.GraphicsSettings;
using inprobitas.game.gui.menus;
using System.ComponentModel.Design;
using System.Text;

namespace inprobitas
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string assetsDir = projectDirectory + "/game/gui/resources/ace attorney/";
            //List<UInt32[]> PhoenixWrightObjection = UnpackFrames(assetsDir + "characters/Phoenix Wright/behind defense bench/Encoded/Phoenix_Objection.bitmap", Width, Height, true);
            UInt32[] courtroomHall = UnpackImage(assetsDir + "rooms/Encoded/courtroomHall.bitmap",Width,Height,true);


            Window w = new Window((ushort)(Width), (ushort)(Height),4,1);



            


            UInt32[] theThinkerImageData = UnpackImage(assetsDir + "scenes/scene resources/Encoded/the-thinker.bitmap",141,204,true);
            List<UInt32[]> blooddrip = UnpackFrames(assetsDir + "scenes/scene resources/Encoded/blooddrop.bitmap", Width, Height, true);
            w.Fill(new Color(0,0,0));
            w.FillWithAt(theThinkerImageData,105,-176,141,204);
            w.FillWithAt(blooddrip[0], 0,0,Width,Height);
            //w.FillWithAt(courtroomHall, 0, 0, Width, Height);
            //w.Update_full();

            int currentFrame = 0;
            int down = 0;
            while (true)
            {
                currentFrame++;
                if (currentFrame > blooddrip.Count - 1) { currentFrame = 0; }
                w.Fill(new Color(0, 0, 0));
                w.FillWithAt(courtroomHall,0,0,Width,Height);
                w.FillWithAt(theThinkerImageData, 105, -176+down, 141, 204);
                w.FillWithAt(blooddrip[currentFrame], 0, down, Width, Height);
                w.Update();
                w.Render();
                if (down < 176) { down+=1; down = Math.Min(down, 176); }
                //w._CompressionFactor = down;
            
            }



            courtroomMenu menu = new courtroomMenu();
            menu.SetRoom("defenseempty");
            menu.SetCharacter("Phoenix Wright", "Objection");
            menu.Render(w);
            //w.Update_optimise2();
            //w.Render_optimise();

            return;

            while (true)
            {
                menu.Render(w);
            }

            return;
            //int currentFrame = 0;
            //while (true)
            //{
            //    currentFrame++;
            //    if (currentFrame > PhoenixWrightObjection.Count - 1) { currentFrame = 0; }
            //    w.FillWithAt(courtroomHall, 0, 0, Width, Height);
            //    w.FillWithAt(PhoenixWrightObjection[currentFrame], 0, 0, Width, Height);
            //    w.Update_optimise2();
            //    w.Render_optimise();
            //    //Thread.Sleep(100);
            //
            //}

            
        }

        static void WaitForEnter()
        {
            while (true)
            {
                ConsoleKey key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.Enter) { break; }
            }
        }
        static void rend(Window w,GUI gui)
        {
            w.ProcessGUI(gui);
        }
    }
}
