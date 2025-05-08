using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static inprobitas.engine.Files.Utility;
using static inprobitas.engine.Files.Video;
using static inprobitas.engine.Files.Image;
using inprobitas.engine.Graphics;
using System.Diagnostics;
using System.Reflection.PortableExecutable;
using System.IO;
using System.ComponentModel;



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
        SET_FRAME,
        WAIT,
        SHOW_TEXT,
        HIDE_TEXT,
        TEXT1,
        TEXT2,
        TEXT3,
        WAIT_FOR_USER,
        SUB,
        RENDER
    }
    struct SceneFrame
    {
        public int timeStamp;
        public SceneEventType eventType;
        public string[] arguments;
    }
    struct SceneResource
    {
        public bool isVideo;
        public int sizeX;
        public int sizeY;
        public List<uint[]> resource;
    }
    public class Cutscene
    {
        private List<SceneFrame> EventBuffer;
        private Dictionary<string, SceneResource> resources;
        public Cutscene(string SceneName)
        {
            Dictionary<string, string> variables = new Dictionary<string, string>();
            resources = new Dictionary<string, SceneResource>();
            EventBuffer = new List<SceneFrame>();
            List<SceneFrame> ScheduleEventBuffer = new List<SceneFrame>();
            int elapsedTime = 0;

            bool inSchedule = false;
            int ScheduleEventStart = 0;
            int ScheduleEventEnd = 0;
            int ScheduleTimeStart = 0;
            int ScheduleTimeEnd = 0;
            int ScheduleTime = 0;
            


            string FilePath = projectDirectory + "/game/gui/resources/ace attorney/scenes/" + SceneName + ".scene";
            using (StreamReader sr = new StreamReader(FilePath))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] args = line.Split(' ');

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
                                            }
                                            else
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
                                SceneResource res = new SceneResource();
                                res.isVideo = false;
                                res.resource = new List<uint[]>();
                                res.resource.Add(UnpackImage(projectDirectory + "/game/gui/resources/ace attorney/scenes/scene resources/Encoded/" + args[1] + ".bitmap", true));
                                UIdim size = GetImageDimensions(projectDirectory + "/game/gui/resources/ace attorney/scenes/scene resources/Encoded/" + args[1] + ".bitmap");
                                res.sizeX = size.pixelX; res.sizeY = size.pixelY;
                                resources.Add(args[2], res);
                                break;
                            }
                        case "LOAD_VIDEO":
                            {
                                SceneResource res = new SceneResource();
                                res.isVideo = false;
                                res.resource = UnpackFrames(projectDirectory + "/game/gui/resources/ace attorney/scenes/scene resources/Encoded/" + args[1] + ".bitmap", true);
                                UIdim size = GetImageDimensions(projectDirectory + "/game/gui/resources/ace attorney/scenes/scene resources/Encoded/" + args[1] + ".bitmap");
                                res.sizeX = size.pixelX; res.sizeY = size.pixelY;
                                resources.Add(args[2], res);
                                break;
                            }
                        case "SHOW":
                            {
                                if (inSchedule)
                                {
                                    ScheduleEventBuffer.Add(new SceneFrame { timeStamp = ScheduleTime, eventType = SceneEventType.SHOW, arguments = new string[] { args[1] } });
                                    ScheduleTime = 0;
                                    ScheduleEventEnd++;
                                } else
                                {
                                    EventBuffer.Add(new SceneFrame { timeStamp = elapsedTime, eventType = SceneEventType.SHOW, arguments = new string[] { args[1] } });
                                }
                                
                                break;
                            }
                        case "HIDE":
                            {
                                if (inSchedule)
                                {
                                    ScheduleEventBuffer.Add(new SceneFrame { timeStamp = ScheduleTime, eventType = SceneEventType.HIDE, arguments = new string[] { args[1] } });
                                    ScheduleTime = 0;
                                    ScheduleEventEnd++;
                                } else
                                {
                                    EventBuffer.Add(new SceneFrame { timeStamp = elapsedTime, eventType = SceneEventType.HIDE, arguments = new string[] { args[1] } });
                                }
                                
                                break;
                            }
                        case "SET_POS":
                            {
                                

                                if (inSchedule)
                                {
                                    ScheduleEventBuffer.Add(new SceneFrame { timeStamp = ScheduleTime, eventType = SceneEventType.SET_POS, arguments = new string[] { args[1], args[2], args[3] } });
                                    ScheduleTime = 0;
                                    ScheduleEventEnd++;
                                } else
                                {
                                    string arg1 = args[2];
                                    if (arg1[0] == '#') { arg1 = variables[arg1.Substring(1)]; }
                                    string arg2 = args[3];
                                    if (arg2[0] == '#') { arg2 = variables[arg2.Substring(1)]; }
                                    EventBuffer.Add(new SceneFrame { timeStamp = elapsedTime, eventType = SceneEventType.SET_POS, arguments = new string[] { args[1], arg1, arg2 } });
                                }
                                break;
                            }
                        case "SET_SIZE":
                            {
                                break;
                            }
                        case "SET_FRAME":
                            {
                                if (inSchedule)
                                {
                                    ScheduleEventBuffer.Add(new SceneFrame { timeStamp = ScheduleTime, eventType = SceneEventType.SET_FRAME, arguments = new string[] { args[1], args[2] } });
                                    ScheduleTime = 0;
                                    ScheduleEventEnd++;
                                } else
                                {
                                    EventBuffer.Add(new SceneFrame { timeStamp = elapsedTime, eventType = SceneEventType.SET_FRAME, arguments = new string[] { args[1], args[2] } });
                                }
                                break;
                            }
                        case "WAIT":
                            {
                                string arg1 = args[1];
                                if (arg1[0] == '#') { arg1 = variables[arg1.Substring(1)]; }
                                if (inSchedule)
                                {
                                    ScheduleTime += int.Parse(arg1);
                                } else
                                {
                                    elapsedTime += int.Parse(arg1);
                                }
                                
                                break;
                            }
                        case "SHOW_TEXT":
                            {

                                if (inSchedule)
                                {
                                    ScheduleEventBuffer.Add(new SceneFrame { timeStamp = ScheduleTime, eventType = SceneEventType.SHOW_TEXT, arguments = new string[] {  } });
                                    ScheduleTime = 0;
                                    ScheduleEventEnd++;
                                } else
                                {
                                    EventBuffer.Add(new SceneFrame { timeStamp = elapsedTime, eventType = SceneEventType.SHOW_TEXT, arguments = new string[] {  } });
                                }
                                break;
                            }
                        case "HIDE_TEXT":
                            {
                                if (inSchedule)
                                {
                                    ScheduleEventBuffer.Add(new SceneFrame { timeStamp = ScheduleTime, eventType = SceneEventType.HIDE_TEXT, arguments = new string[] { } });
                                    ScheduleTime = 0;
                                    ScheduleEventEnd++;
                                }
                                else
                                {
                                    EventBuffer.Add(new SceneFrame { timeStamp = elapsedTime, eventType = SceneEventType.HIDE_TEXT, arguments = new string[] { } });
                                }
                                break;
                            }
                        case "TEXT1":
                            {
                                string arg1 = args[1];
                                int i = 2;
                                if (arg1[0] == '#') { arg1 = variables[arg1.Substring(1)]; }
                                else if (arg1[0] == '"')
                                {
                                    if (i < args.Length - 1)
                                    {
                                        while (!args[i].EndsWith('"') && i < args.Length - 1)
                                        {
                                            arg1 += args[i];
                                            i++;
                                        }
                                    } else
                                    {
                                        i = 1;
                                    }
                                }
                                string arg2 = "";
                                if (i != 1)
                                {
                                    arg2 = args[i];
                                }

                                string cat = (arg1 + arg2);

                                if (inSchedule)
                                {
                                    ScheduleEventBuffer.Add(new SceneFrame { timeStamp =  ScheduleTime, eventType = SceneEventType.TEXT1, arguments = new string[] { cat.Substring(1,cat.Length-2) } });
                                    ScheduleTime = 0;
                                    ScheduleEventEnd++;
                                } else
                                {
                                    EventBuffer.Add(new SceneFrame { timeStamp = elapsedTime, eventType = SceneEventType.TEXT1, arguments = new string[] { cat.Substring(1, cat.Length - 1) } });
                                }
                                break;
                            }
                        case "TEXT2":
                            {
                                string arg1 = args[1];
                                int i = 2;
                                if (arg1[0] == '#') { arg1 = variables[arg1.Substring(1)]; }
                                else if (arg1[0] == '"')
                                {
                                    if (i < args.Length - 1)
                                    {
                                        while (!args[i].EndsWith('"') && i < args.Length - 1)
                                        {
                                            arg1 += args[i];
                                            i++;
                                        }
                                    }
                                    else
                                    {
                                        i = 1;
                                    }
                                }
                                string arg2 = "";
                                if (i != 1)
                                {
                                    arg2 = args[i];
                                }

                                string cat = (arg1 + arg2);

                                if (inSchedule)
                                {
                                    ScheduleEventBuffer.Add(new SceneFrame { timeStamp = ScheduleTime, eventType = SceneEventType.TEXT2, arguments = new string[] { cat.Substring(1, cat.Length - 2) } });
                                    ScheduleTime = 0;
                                    ScheduleEventEnd++;
                                }
                                else
                                {
                                    EventBuffer.Add(new SceneFrame { timeStamp = elapsedTime, eventType = SceneEventType.TEXT2, arguments = new string[] { cat.Substring(1, cat.Length - 1) } });
                                }
                                break;
                            }
                        case "TEXT3":
                            {
                                string arg1 = args[1];
                                int i = 2;
                                if (arg1[0] == '#') { arg1 = variables[arg1.Substring(1)]; }
                                else if (arg1[0] == '"')
                                {
                                    if (i < args.Length - 1)
                                    {
                                        while (!args[i].EndsWith('"') && i < args.Length - 1)
                                        {
                                            arg1 += args[i];
                                            i++;
                                        }
                                    }
                                    else
                                    {
                                        i = 1;
                                    }
                                }
                                string arg2 = "";
                                if (i != 1)
                                {
                                    arg2 = args[i];
                                }

                                string cat = (arg1 + arg2);

                                if (inSchedule)
                                {
                                    ScheduleEventBuffer.Add(new SceneFrame { timeStamp = ScheduleTime, eventType = SceneEventType.TEXT3, arguments = new string[] { cat.Substring(1, cat.Length - 2) } });
                                    ScheduleTime = 0;
                                    ScheduleEventEnd++;
                                }
                                else
                                {
                                    EventBuffer.Add(new SceneFrame { timeStamp = elapsedTime, eventType = SceneEventType.TEXT3, arguments = new string[] { cat.Substring(1, cat.Length - 1) } });
                                }
                                break;
                            }
                        case "WAIT_FOR_USER":
                            {
                                if (inSchedule)
                                {
                                    ScheduleEventBuffer.Add(new SceneFrame { timeStamp = ScheduleTime, eventType = SceneEventType.WAIT_FOR_USER, arguments = new string[] { } });
                                    ScheduleTime = 0;
                                    ScheduleEventEnd++;
                                }
                                else
                                {
                                    EventBuffer.Add(new SceneFrame { timeStamp = elapsedTime, eventType = SceneEventType.WAIT_FOR_USER, arguments = new string[] { } });
                                }
                                break;
                            }
                        case "SCHEDULE":
                            {
                                ScheduleTimeStart = int.Parse(args[1]);
                                ScheduleTimeEnd = int.Parse(args[2]);




                                ScheduleEventStart = EventBuffer.Count;
                                ScheduleEventEnd = ScheduleEventStart;
                                inSchedule = true;
                                break;
                            }
                        case "END_DEF":
                            {
                                int expendedTime = 0;
                                int id = 0;

                                int i = 0;
                                while (expendedTime < (ScheduleTimeEnd - ScheduleTimeStart))
                                {
                                    SceneFrame frame = ScheduleEventBuffer[i];
                                    Debug.WriteLine($"Schedule: {expendedTime} {frame.eventType} {string.Join(",",frame.arguments)}");
                                    expendedTime += frame.timeStamp;

                                    SceneFrame copy = new SceneFrame();
                                    copy.timeStamp = frame.timeStamp + ScheduleTimeStart + expendedTime + id; id++;
                                    copy.eventType = frame.eventType;
                                    copy.arguments = new string[frame.arguments.Length];
                                    frame.arguments.CopyTo(copy.arguments, 0);

                                    switch (frame.eventType)
                                    {
                                        case SceneEventType.SUB:
                                            {
                                                variables[frame.arguments[0]] = $"{int.Parse(variables[frame.arguments[0]]) - int.Parse(frame.arguments[1])}";
                                                break;
                                            }
                                        case SceneEventType.SET_POS:
                                            {
                                                string arg1 = frame.arguments[1];
                                                if (arg1[0] == '#') { copy.arguments[1] = variables[arg1.Substring(1)]; }
                                                string arg2 = frame.arguments[2];
                                                if (arg2[0] == '#') { copy.arguments[2] = variables[arg2.Substring(1)]; }
                                                break;
                                            }
                                        case SceneEventType.RENDER:
                                            {
                                                break;
                                            }
                                    }



                                    

                                    if (frame.eventType != SceneEventType.SUB)
                                    {
                                        EventBuffer.Add(copy);
                                    }
                                    


                                    i++;
                                    if (i >= ScheduleEventEnd-ScheduleEventStart)
                                    {
                                        i = 0;
                                    }
                                }

                                inSchedule = false; ScheduleTime = 0; ScheduleEventBuffer.Clear();
                                break;
                            }
                        case "SUB":
                            {
                                if (inSchedule)
                                {
                                    ScheduleEventBuffer.Add(new SceneFrame { timeStamp = 0, eventType = SceneEventType.SUB, arguments = new string[] { args[1], args[2] } });
                                    ScheduleEventEnd++;
                                } else
                                {
                                    variables[args[1]] = $"{int.Parse(variables[args[1]]) - int.Parse(args[2])}";
                                }
                                break;
                            }
                        case "RENDER":
                            {
                                if (inSchedule)
                                {
                                    ScheduleEventBuffer.Add(new SceneFrame { timeStamp = ScheduleTime, eventType = SceneEventType.RENDER, arguments = new string[] { } });
                                    ScheduleTime = 0;
                                    ScheduleEventEnd++;
                                } else
                                {
                                    EventBuffer.Add(new SceneFrame { timeStamp = elapsedTime, eventType = SceneEventType.RENDER, arguments = new string[] { } });
                                }
                                break;
                            }
                        case "//":
                            {
                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }
                }
            }
            EventBuffer.Sort((s1, s2) => s1.timeStamp.CompareTo(s2.timeStamp));
            Debug.WriteLine($"variables: {string.Join(", ", variables)}");
        }
        
        public void PlayCutscene(Window w)
        {
            int CurrentZindex = 1;
            GUI cutsceneGui = new GUI(w.Width, w.Height);
            Background backgroundCol = new Background(new Color(0, 0, 0));
            Background transparrentBG = new Background(new Color(0, 0, 0, 200));
            Border TextBoxBoarder = new Border(new Color(254, 255, 255,255));


            Frame bg = new Frame(new UIdim(0, 0, 0f, 0f), new UIdim(w.Width, w.Height, 0f, 0f), new UIdim(0, 0, 0f, 0f),0);
            
            Frame TextBox = new Frame(new UIdim(2, -2, 0f, 1f), new UIdim(w.Width - 4,  -4, 0f, 0.30f), new UIdim(0, 0, 0f, 1f), -999);

            Frame Text1Align = new Frame(new UIdim(0, 0, 0.03f, 0f), new UIdim(0, 0, 0.97f, 0.33f), new UIdim(0, 0, 0f, 0f), 1000);
            Frame Text2Align = new Frame(new UIdim(0, 0, 0.03f, 0.33f), new UIdim(0, 0, 0.97f, 0.33f), new UIdim(0, 0, 0f, 0f), 1000);
            Frame Text3Align = new Frame(new UIdim(0, 0, 0.03f, 0.66f), new UIdim(0, 0, 0.97f, 0.33f), new UIdim(0, 0, 0f, 0f), 1000);


            Text Text1 = new Text("", new Color(255, 255, 255), 15, "Comfortaa");
            Text Text2 = new Text("", new Color(255, 255, 255), 15, "Comfortaa");
            Text Text3 = new Text("", new Color(255, 255, 255), 15, "Comfortaa");

            Text1Align.Append(Text1);
            Text2Align.Append(Text2);
            Text3Align.Append(Text3);

            TextBox.Append(Text1Align);
            TextBox.Append(Text2Align);
            TextBox.Append(Text3Align);

            TextBox.Append(TextBoxBoarder);
            TextBox.Append(transparrentBG);
            bg.Append(backgroundCol);

            cutsceneGui.Append(bg);
            cutsceneGui.Append(TextBox);

            Dictionary<string, Image> SceneRenderables = new Dictionary<string, Image>();

            // initialise renderable objects
            foreach (string resName in resources.Keys)
            {
                SceneResource res = resources[resName];
                UIdim dim = new UIdim(res.sizeX, res.sizeY,0f,0f);

                //SceneRenderables[resName] = new Image(new UIdim(0, 0, 0f, 0f), new UIdim((dim.pixelX*2) / 3, (dim.pixelY*2) / 3, .0f, .0f), new UIdim(0, 0, 0f, 0f), -1, new UIdim((dim.pixelX*2)/3,(dim.pixelY*2)/3,.0f,.0f), res.resource[0], dim);
                SceneRenderables[resName] = new Image(new UIdim(0, 0, 0f, 0f), dim, new UIdim(0, 0, 0f, 0f), -1, dim, res.resource[0], dim);
                cutsceneGui.Append(SceneRenderables[resName]);
            }


            int elapsedTime = 0;
            foreach (SceneFrame f in EventBuffer)
            {
                Debug.WriteLine($"look at this shit: [{f.timeStamp}][{f.timeStamp-elapsedTime}] {f.eventType} {string.Join(", ",f.arguments)}");
                switch(f.eventType)
                {
                    case SceneEventType.SHOW:
                        {
                            SceneRenderables[f.arguments[0]].ZIndex = CurrentZindex;
                            CurrentZindex++;
                            break;
                        }
                    case SceneEventType.HIDE:
                        {
                            SceneRenderables[f.arguments[0]].ZIndex = -1;
                            break;
                        }
                    case SceneEventType.SET_FRAME:
                        {
                            SceneRenderables[f.arguments[0]].ImageData = resources[f.arguments[0]].resource[int.Parse(f.arguments[1])-1];
                            SceneRenderables[f.arguments[0]].GenerateScaledImage();
                            SceneRenderables[f.arguments[0]].GeneratePerFrameImage(SceneRenderables[f.arguments[0]].lastSize);
                            break;
                        }
                    case SceneEventType.SET_POS:
                        {
                            SceneRenderables[f.arguments[0]].position = new UIdim(int.Parse(f.arguments[1]), int.Parse(f.arguments[2]), 0f, 0f);
                            break;
                        }
                    case SceneEventType.SHOW_TEXT:
                        {
                            TextBox.ZIndex = 999;
                            break;
                        }
                    case SceneEventType.HIDE_TEXT:
                        {
                            TextBox.ZIndex = -999;
                            break;
                        }
                    case SceneEventType.TEXT1:
                        {
                            Text1.Content = f.arguments[0];
                            break;
                        }
                    case SceneEventType.TEXT2:
                        {
                            Text2.Content = f.arguments[0];
                            break;
                        }
                    case SceneEventType.TEXT3:
                        {
                            Text3.Content = f.arguments[0];
                            break;
                        }
                    case SceneEventType.WAIT_FOR_USER:
                        {
                            Console.ReadKey(true);
                            break;
                        }
                }
                if (f.eventType == SceneEventType.RENDER)
                {
                    Thread.Sleep((int)f.timeStamp - elapsedTime);
                    elapsedTime += (int)f.timeStamp - elapsedTime;
                    w.ProcessGUI(cutsceneGui);
                    w.Update();
                    w.Render();
                    Debug.WriteLine("RENDERING!");
                }
            }
        }
    }
}


