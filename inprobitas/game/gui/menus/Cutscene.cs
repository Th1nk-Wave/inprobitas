using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static inprobitas.engine.Files.Utility;



namespace inprobitas.game.gui.menus
{
    enum SceneEventType
    {
        DEFINE,
        UNDEFINE,
        LOAD_IMAGE,
        LOAD_VIDEO,
        SHOW,
        HIDE,
        SET_POS,
        SET_SIZE,
        WAIT,
    }
    struct SceneEvent
    {

    }
    public class Cutscene
    {
        private List<SceneEvent> EventBuffer; 
        public Cutscene(string SceneName)
        {
            string FilePath = projectDirectory + "/game/gui/resources/ace attorney/scenes/" + SceneName + ".scene";
            using (StreamReader sr = new StreamReader(FilePath))
            {
                sr.ReadLine();
            }
        }
    }
}


