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
            //string assetsDir = projectDirectory + "/game/gui/resources/ace attorney/";
            //List<UInt32[]> PhoenixWrightObjection = UnpackFrames(assetsDir + "characters/Phoenix Wright/behind defense bench/Encoded/Phoenix_Objection.bitmap", Width, Height, true);
            //UInt32[] courtroomHall = UnpackImage(assetsDir + "rooms/Encoded/courtroomHall.bitmap",Width,Height,true);


            Window w = new Window(Width, Height,4);

            courtroomMenu menu = new courtroomMenu();
            menu.SetRoom("defenseempty");
            menu.SetCharacter("Phoenix Wright", "Objection");
            menu.Render(w);
            w.Update_optimise2();
            w.Render_optimise();

            while (true)
            {
                menu.Render(w);
                w.Update_optimise2();
                w.Render_optimise();
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
            w.Update_optimise2();
            w.Render_optimise();
        }
    }
}
