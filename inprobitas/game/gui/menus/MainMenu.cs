using inprobitas.engine.Graphics;
using static inprobitas.game.settings.GraphicsSettings;

namespace inprobitas.game.gui.menus
{
    static public class MainMenu
    {
        static public Text optionText = new Text("continue",new Color(255,255,255),25,"Comfortaa");
        static public Frame GuiOption1 = new Frame(new UIdim(0, 0.5f, 0, 0.5f), new UIdim(0, 0.25f, 0, 1f), new UIdim(0, 0.5f, 0, 0.5f), 1, modifications: [optionText]);


        static public Background transparrentBG = new Background(new Color(100, 100, 100, 100));
        static public Frame CenterFrame = new Frame(new UIdim(0, 0.5f, 0, 0.5f), new UIdim(0, 0.25f, 0, 1f), new UIdim(0, 0.5f, 0, 0.5f), 1, modifications: [transparrentBG],
            children: [GuiOption1]);

        static public Background blackBG = new Background(new Color(0, 0, 0));
        static public Frame BGFrame = new Frame(new UIdim(0, 0f, 0, 0f), new UIdim(0, 1f, 0, 1f), new UIdim(0, 0f, 0, 0f), 0, modifications: [blackBG], children: [CenterFrame]);




        static public GUI mainGUI = new GUI(Width, Height, [BGFrame]);
    }
}
