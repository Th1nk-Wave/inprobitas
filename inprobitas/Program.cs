using System.Runtime.CompilerServices;
using inprobitas.engine.Graphics;
using static inprobitas.engine.Files.Utility;
using static inprobitas.engine.Files.Video;

namespace inprobitas
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ushort res = 60;
            Window w = new Window((ushort)(8 * res), (ushort)(6 * res), 2);

            //w.Box((ushort)(8 * res-6), (ushort)(6 * res-6), (ushort)(8 * res), (ushort)(6 * res), new Color(255, 0, 0));

            Background GreenBG = new Background(new Color(0,25,0));
            Frame ScreenCover = new Frame(new UIdim(0, 0f, 0, 0f), new UIdim(0, 1f, 0, 1f), new UIdim(0, 0f, 0, 0f),0);
            ScreenCover.Append(GreenBG);


            Border greenBD = new Border(new Color(0, 255, 0));
            Background WhiteBG = new Background(new Color(255, 255, 255));
            Frame box1 = new Frame(new UIdim(0, 0f, 0, 0f), new UIdim(5, 0f, 5, 0f), new UIdim(0, 0f, 0, 0f), 101);
            box1.Append(WhiteBG);

            Frame box2 = new Frame(new UIdim(0, 1f, 0, 1f), new UIdim(5, 0f, 5, 0f), new UIdim(0, 1f, 0, 1f), 102);
            box2.Append(WhiteBG);

            Background redBG = new Background(new Color(255, 0, 0));

            Frame box = new Frame(new UIdim(0, 0.5f, 0, 0.5f), new UIdim(5, 0f, 5, 0f), new UIdim(0, 0.5f, 0, 0.5f), 100);
            box.Append(redBG);
            box.Append(greenBD);

            GUI hud = new GUI(w.Width, w.Height);
            hud.Append(box);
            hud.Append(box1);
            hud.Append(box2);
            hud.Append(ScreenCover);

            w.ProcessGUI(hud);
            w.Update_optimise2();
            w.Render_optimise();

            res = 15;
            List<UInt32[]> imageData = UnpackFrames(projectDirectory + "/game/gui/resources/Image/test.txt", (res * 8), (res * 6));
            UInt32[] image = imageData[200];
            Image GuiImage = new Image(new UIdim(0, 0, 0f, 0f), new UIdim(50, 50, 0f, 0f), new UIdim(0, 0, 0f, 0f), 100, new UIdim((res * 8), (res * 6), 0f, 0f), image, new UIdim((res * 8), (res * 6), 0f, 0f));

            hud.Append(GuiImage);

            w.ProcessGUI(hud);
            w.Update_optimise2();
            w.Render_optimise();
            Random rnd = new Random();
            Text randomAssStuff = new Text("eat my ass microsoft" + rnd.Next(0, 10).ToString(), new Color(255, 0, 0), 20, "Comfortaa");
            Text randomAssStuff2 = new Text("fuck your stupid terminal" + rnd.Next(0, 10).ToString(), new Color(255, 0, 0), 30, "Comfortaa");
            Frame randomAssContainer = new Frame(new UIdim(50, 0f, 50, 0f), new UIdim(50, 0f, 50, 0f), new UIdim(0, 0f, 0, 0f), 1000);
            Frame randomAssContainer2 = new Frame(new UIdim(0, 0f, 100, 0f), new UIdim(50, 0f, 50, 0f), new UIdim(0, 0f, 0, 0f), 1000);
            randomAssContainer.Append(randomAssStuff);
            randomAssContainer2.Append(randomAssStuff2);
            hud.Append(randomAssContainer);
            hud.Append(randomAssContainer2);
            w.ProcessGUI(hud);
            w.Update_optimise2();
            w.Render_optimise();
            
            while (true)
            {
                randomAssContainer.Remove(randomAssStuff);
                randomAssStuff = new Text("eat my ass microsoft" + rnd.Next(0, 10).ToString(), new Color(255, 0, 0), 20, "Comfortaa");
                randomAssContainer.Append(randomAssStuff);

                randomAssContainer2.Remove(randomAssStuff2);
                randomAssStuff2 = new Text("fuck your stupid terminal" + rnd.Next(0, 10).ToString(), new Color(255, 0, 0), 30, "Comfortaa");
                randomAssContainer2.Append(randomAssStuff2);

                w.ProcessGUI(hud);
                w.Update_optimise2();
                w.Render_optimise();
            }
        }
    }
}
