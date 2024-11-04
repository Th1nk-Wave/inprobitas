using inprobitas.engine.Graphics;
using inprobitas.game.settings;
using static inprobitas.game.settings.GraphicsSettings;

namespace inprobitas.game.gui.menus
{
    public static class Master
    {
        static public Background blackBG = new Background(new Color(0, 0, 0));
        static public Frame BGFrame = new Frame(new UIdim(0,0f,0,0f), new UIdim(0, 1f, 0, 1f), new UIdim(0, 0f, 0, 0f),0);
        static public GUI mainGUI = new GUI(Width,Height);
    }
}
