using System.Runtime.CompilerServices;
using inprobitas.engine.Graphics;
using static inprobitas.engine.Files.Utility;
using static inprobitas.engine.Files.Video;
using static inprobitas.engine.Files.Image;
using static inprobitas.game.settings.GraphicsSettings;
using inprobitas.game.gui.menus;
using System.ComponentModel.Design;
using System.Text;
using System.Timers;

namespace inprobitas
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string assetsDir = projectDirectory + "/game/gui/resources/ace attorney/";
            //List<UInt32[]> PhoenixWrightObjection = UnpackFrames(assetsDir + "characters/Phoenix Wright/behind defense bench/Encoded/Phoenix_Objection.bitmap", Width, Height, true);
            UInt32[] courtroomHall = UnpackImage(assetsDir + "rooms/Encoded/courtroomHall.bitmap",true);


            Window w = new Window((ushort)(Width), (ushort)(Height),4,0);

            GUI test = new GUI(Width,Height);

            Frame background = new Frame(new UIdim(0,0,0f,0f),new UIdim(Width,Height,0f,0f),new UIdim(0,0,0f,0f),0);
            Background background1 = new Background(new Color(255, 0, 0));
            background.Append(background1);

            test.Append(background);

            w.ProcessGUI(test);
            w.Update();
            w.Render();

            Image idk = new Image(new UIdim(0, 0, 0f, 0f), new UIdim(0, 0, 0.5f, 0.5f), new UIdim(0, 0, 0f, 0f), 1, new UIdim(Width,Height,0f,0f) ,UnpackImage(assetsDir + "scenes/scene resources/Encoded/deadgirl.bitmap", true), new UIdim(Width, Height, 0f, 0f));
            background.Append(idk);

            w.ProcessGUI(test);
            w.Update(); w.Render();



            return;

            


            UInt32[] theThinkerImageData = UnpackImage(assetsDir + "scenes/scene resources/Encoded/the-thinker.bitmap",true);
            List<UInt32[]> blooddrip = UnpackFrames(assetsDir + "scenes/scene resources/Encoded/blooddrop.bitmap", true);
            w.Fill(new Color(0,0,0));
            w.FillWithAt(theThinkerImageData,105,-176,141,204);
            w.FillWithAt(blooddrip[0], 0,0,Width,Height);
            //w.FillWithAt(courtroomHall, 0, 0, Width, Height);
            //w.Update_full();

            

            float currentFrame = 0;
            float down = 0f;
            DateTime beffore = DateTime.UtcNow;
            DateTime after = beffore;
            while (true)
            {
                DateTime after2 = DateTime.UtcNow;

                w.Fill(new Color(0, 0, 0));
                //w.FillWithAt(courtroomHall,0,0,Width,Height);
                w.FillWithAt(theThinkerImageData, 105, (int)(-176+down), 141, 204);
                w.FillWithAt(blooddrip[(int)currentFrame], 0, (int)down, Width, Height);
                
                w.Update();
                w.Render();
                after = DateTime.UtcNow;
                
                if (down < 176f) { down = ((float)(after - beffore).TotalMilliseconds / 1000f) * 45; down = Math.Min(down, 176f); }
                currentFrame += (float)((after - after2).TotalMilliseconds) / 1000f * 10;
                if (currentFrame > blooddrip.Count - 1) { currentFrame = 0f; }

            }


            courtroomMenu menu = new courtroomMenu();
            menu.SetRoom("defenseempty");
            menu.SetCharacter("Phoenix Wright", "Objection");
            menu.Render(w);
            w.Update();
            w.Render();


            while (true)
            {
                menu.Render(w);
                w.Update();
                w.Render();
                Thread.Sleep(150);
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
