using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static inprobitas.engine.Files.Utility;
using static inprobitas.engine.Files.Video;
using static inprobitas.engine.Files.Image;



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
        SHOW_TEXT
    }
    struct SceneFrame
    {
        public uint delay;
        public SceneEventType eventType;
        public string[] arguments;
    }
    public class Cutscene
    {
        private List<SceneFrame> EventBuffer;
        private Dictionary<string, Object> resources;
        public Cutscene(string SceneName)
        {
            Dictionary<string,string> variables = new Dictionary<string,string>();
            EventBuffer = new List<SceneFrame>();
            uint waitTimer = 0;


            string FilePath = projectDirectory + "/game/gui/resources/ace attorney/scenes/" + SceneName + ".scene";
            using (StreamReader sr = new StreamReader(FilePath))
            {
                string[] args = sr.ReadLine().Split(' ');

                switch (args[0])
                {
                    case "DEFINE":
                        {
                            switch (args[2][0])
                            {
                                case '"':
                                    {
                                        if (args.Length >= 4)
                                        {
                                            string concat = args[2];
                                            for (int i = 3; i < args.Length; i++)
                                            {
                                                concat += args[i];
                                            }
                                            variables.Add(args[1], args[2]);
                                        } else
                                        {
                                            variables.Add(args[1], args[2]);
                                        }
                                        break;
                                    }
                                default:
                                    {
                                        variables.Add(args[1], args[2]);
                                        break;
                                    }
                            }
                            break;
                        }
                    case "UNEFINE":
                        {
                            variables.Remove(args[1]);
                            break;
                        }
                    case "LOAD_IMAGE":
                        {
                            resources.Add(args[2], UnpackImage(projectDirectory + "/game/resources/scene resources/Encoded/" + args[1] + ".bitmap", true));
                            break;
                        }
                    case "LOAD_VIDEO":
                        {
                            resources.Add(args[2], UnpackFrames(projectDirectory + "/game/resources/scene resources/Encoded/" + args[1] + ".bitmap", true));
                            break;
                        }
                    case "SHOW":
                        {
                            EventBuffer.Add(new SceneFrame { delay = waitTimer, eventType = SceneEventType.SHOW, arguments = new string[] { args[1] } });
                            waitTimer = 0;
                            break;
                        }
                    case "HIDE":
                        {
                            EventBuffer.Add(new SceneFrame { delay = waitTimer, eventType = SceneEventType.HIDE, arguments = new string[] { args[1] } });
                            waitTimer = 0;
                            break;
                        }
                    case "SET_POS":
                        {
                            string arg1 = args[2];
                            if (arg1[0] == '#') { arg1 = variables[arg1.Substring(1)]; }
                            string arg2 = args[3];
                            if (arg2[0] == '#') { arg2 = variables[arg2.Substring(1)]; }
                            EventBuffer.Add(new SceneFrame { delay = waitTimer, eventType = SceneEventType.SET_POS, arguments =  new string[] { args[1], arg1, arg2 } });
                            waitTimer = 0;
                            break;
                        }
                    case "SET_SIZE":
                        {
                            break;
                        }
                    case "WAIT":
                        {
                            string arg1 = args[1];
                            if (arg1[0] == '#') { arg1 = variables[arg1.Substring(1)]; }
                            waitTimer += uint.Parse(arg1);
                            break;
                        }
                    case "SHOW_TEXT":
                        {
                            string arg1 = args[1];
                            int i = 2;
                            if (arg1[0] == '#') { arg1 = variables[arg1.Substring(1)]; }
                            else if (arg1[0] == '"')
                            {
                                
                                while (!args[i].EndsWith('"'))
                                {
                                    arg1 += args[i];
                                }
                            }
                            string arg2 = args[i];

                            EventBuffer.Add(new SceneFrame { delay = waitTimer, eventType = SceneEventType.SHOW_TEXT, arguments = new string[] { arg1, arg2 } });
                            waitTimer = 0;
                            

                            break;
                        }
                    case "//":
                        {
                            break;
                        }
                }

            }
        }
    }
}


