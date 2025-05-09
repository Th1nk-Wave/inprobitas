using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using inprobitas.engine.Graphics;
using inprobitas.game.gui.menus;
using System.Media;
using static inprobitas.engine.Files.Utility;
using NAudio.Wave;
using System.Net.Http.Headers;

namespace inprobitas.game.story
{
    public enum ScriptEventType
    {
        TEXT,
        Unknown,
        background,
        fade,
        wait,
        hidetextbox,
        fp,
        speed,
        bip,
        color,
        p,
        b,
        event_,
        name,
        character,
        animation,
        music,
        sound,
        flash,
        shake
        // Add more as needed
    }



    public struct ScriptEvent
    {
        public ScriptEventType Type;
        public string[] Arguments;
        public ScriptEvent(ScriptEventType type, string[] args)
        {
            Type = type;
            Arguments = args;
        }
        public override string ToString() => $"<{Type}:{string.Join(",", Arguments)}>";
    }
    public static class ScriptParser
    {
        public static ScriptEventType ParseType(string name)
        {
            // Allow "event" as alias for "event_"
            if (name == "event")
                name = "event_";

            return Enum.TryParse<ScriptEventType>(name, ignoreCase: true, out var result)
                ? result
                : ScriptEventType.Unknown;
        }

        public static void Parse(Window w, courtroomMenu menu, string story)
        {
            string filePath = story; // Use your actual file path
            var entries = new List<ScriptEvent>();
            var commandRegex = new Regex(@"<([^:>]+)(?::([^>]+))?>");

            foreach (var line in File.ReadLines(filePath))
            {
                int currentIndex = 0;

                foreach (Match match in commandRegex.Matches(line))
                {
                    if (match.Index > currentIndex)
                    {
                        string textBefore = line.Substring(currentIndex, match.Index - currentIndex);
                        if (!string.IsNullOrWhiteSpace(textBefore))
                            entries.Add(new ScriptEvent(ScriptEventType.TEXT,new string[] { textBefore.Trim() }));
                    }

                    string commandName = match.Groups[1].Value.Trim();
                    string argString = match.Groups[2].Success ? match.Groups[2].Value.Trim() : "";
                    ScriptEventType type = ParseType(commandName);
                    string[] argsArray = string.IsNullOrEmpty(argString)
                        ? Array.Empty<string>()
                        : argString.Split(',');

                    entries.Add(new ScriptEvent(type, argsArray));
                    currentIndex = match.Index + match.Length;
                }

                if (currentIndex < line.Length)
                {
                    string remainingText = line[currentIndex..].Trim();
                    if (!string.IsNullOrWhiteSpace(remainingText))
                        entries.Add(new ScriptEvent(ScriptEventType.TEXT, new string[] { remainingText }));
                }
            }
            string assetsDir = projectDirectory + "/game/gui/resources/ace attorney/";
            var bipOut = new WaveOutEvent();
            var bipReader = new WaveFileReader(assetsDir + "sounds/sfx/sfx-typewriter.wav");
            bipOut.Init(bipReader);

            var SfxOut = new WaveOutEvent();
            var SfxReader = new WaveFileReader(assetsDir + "sounds/sfx/sfx-lightbulb.wav");

            var MusicOut = new WaveOutEvent();
            var MusicReader = new WaveFileReader(assetsDir + "sounds/music/lobby.wav");

            string Speaker = "NO-ONE";
            string Character = "none";
            string Animation = "none";
            bool TextBoxVisible = false;
            int TextRow = 0;
            int TextSpeed = 0;
            float bipVolume = 1;
            // Example: Print events
            foreach (ScriptEvent ev in entries)
            {
                switch (ev.Type)
                {
                    case ScriptEventType.TEXT:
                        {
                            menu.ShowText();

                            string t1Prev = menu.Text1.Content;
                            string t2Prev = menu.Text2.Content;
                            string t3Prev = menu.Text3.Content;
                            for (int i = 0; i <= ev.Arguments[0].Length; i++)
                            {
                                string sub = ev.Arguments[0].Substring(0,i);
                                switch (TextRow)
                                {
                                    case 0:
                                        {
                                            menu.SetText1(t1Prev+sub);
                                            break;
                                        }
                                    case 1:
                                        {
                                            menu.SetText2(t2Prev + sub);
                                            break;
                                        }
                                    case 2:
                                        {
                                            menu.SetText3(t3Prev + sub);
                                            break;
                                        }
                                }
                                Thread.Sleep(TextSpeed * 10);
                                try
                                {
                                    bipReader.CurrentTime = new TimeSpan(0);
                                } catch (Exception ex)
                                {
                                    Debug.WriteLine("Bip sound failed to set reader time");
                                }
                                
                                if (bipVolume  > 0)
                                {
                                    bipOut = new WaveOutEvent();
                                    bipOut.Init(bipReader);
                                    if (bipOut.PlaybackState == PlaybackState.Stopped)
                                    {
                                        bipOut.Play();
                                    }
                                }
                                


                                menu.Render(w);
                                w.Update();
                                w.Render();
                            }
                            
                            break;
                        }
                    case ScriptEventType.bip:
                        {
                            if (bipOut.PlaybackState == PlaybackState.Playing) { bipOut.Stop(); }
                            bipVolume = 1;
                            switch (ev.Arguments[0])
                            {
                                case "BIP_HIGH":
                                    {
                                        bipReader.Close();
                                        bipReader = new WaveFileReader(assetsDir + "sounds/sfx/sfx-blipfemale.wav");
                                        bipOut.Init(bipReader);
                                        break;
                                    }
                                case "BIP_NORMAL":
                                    {
                                        bipReader.Close();
                                        bipReader = new WaveFileReader(assetsDir + "sounds/sfx/sfx-blipmale.wav");
                                        bipOut.Init(bipReader);
                                        break;
                                    }
                                case "BIP_TYPEWRITER":
                                    {
                                        bipReader.Close();
                                        bipReader = new WaveFileReader(assetsDir + "sounds/sfx/sfx-typewriter.wav");
                                        bipOut.Init(bipReader);
                                        break;
                                    }
                                case "BIP_NONE":
                                    {
                                        bipVolume = 0;
                                        break;
                                    }
                                default:
                                    {
                                        Debug.WriteLine($"ERROR: UNKNOWN BIP: {ev.Arguments[0]}");
                                        break;
                                    }
                            }
                            break;
                        }
                    case ScriptEventType.speed:
                        {
                            TextSpeed = int.Parse(ev.Arguments[0]);
                            break;
                        }
                    case ScriptEventType.b:
                        {
                            TextRow++;
                            break;
                        }
                    case ScriptEventType.p:
                        {
                            TextRow = 0;
                            menu.SetText1("");
                            menu.SetText2("");
                            menu.SetText3("");
                            Console.ReadKey(true);
                            break;
                        }
                    case ScriptEventType.name:
                        {
                            switch (ev.Arguments[0])
                            {
                                case "NAM_PHOENIX":
                                    {
                                        Speaker = "Phoenix";
                                        break;
                                    }
                                case "NAM_MIA":
                                    {
                                        Speaker = "Mia";
                                        break;
                                    }
                                case "NAM_BUTZ":
                                    {
                                        Speaker = "Butz";
                                        break;
                                    }
                                case "NAM_UNKNOW":
                                    {
                                        Speaker = "???";
                                        break;
                                    }
                                case "NAM_JUDGE":
                                    {
                                        Speaker = "Judge";
                                        break;
                                    }
                                case "NAM_PAYNE":
                                    {
                                        Speaker = "Payne";
                                        break;
                                    }
                                case "NAM_NONE":
                                    {
                                        Speaker = "";
                                        break;
                                    }
                                default:
                                    {
                                        Debug.WriteLine($"ERROR: UNKNOWN NAME: {ev.Arguments[0]}");
                                        break;
                                    }
                            }
                            menu.SetSpeaker(Speaker);
                            break;
                        }
                    case ScriptEventType.character:
                        {
                            switch (ev.Arguments[0])
                            {
                                case "CHR_MIA":
                                    {
                                        Character = "Mia";
                                        break;
                                    }
                                case "CHR_LARRY":
                                    {
                                        Character = "Larry";
                                        break;
                                    }
                                case "CHR_JUDGE":
                                    {
                                        Character = "Judge";
                                        break;
                                    }
                                case "CHR_PHOENIX":
                                    {
                                        Character = "Phoenix";
                                        break;
                                    }
                                case "CHR_PAYNE":
                                    {
                                        Character = "Payne";
                                        break;
                                    }
                                case "CHR_NONE":
                                    {
                                        Character = "none";
                                        break;
                                    }
                                default:
                                    {
                                        Debug.WriteLine($"ERROR: UNKNOWN CHARACTER: {ev.Arguments[0]}");
                                        break;
                                    }
                            }
                            Animation = "none";
                            menu.SetCharacter(Character, Animation);
                            break;
                        }
                    case ScriptEventType.animation:
                        {
                            switch (ev.Arguments[0])
                            {
                                case "ANI_NONE":
                                    {
                                        Animation = "none";
                                        break;
                                    }
                                case "ANI_MIA_QUESTION_STAND":
                                    {
                                        Animation = "mia-ohmy(a)";
                                        Character = "Mia";
                                        break;
                                    }
                                case "ANI_MIA_QUESTION_TALK":
                                    {
                                        Animation = "mia-ohmy(b)";
                                        Character = "Mia";
                                        break;
                                    }
                                case "ANI_MIA_STAND":
                                    {
                                        Animation = "mia-normal(a)";
                                        Character = "Mia";
                                        break;
                                    }
                                case "ANI_MIA_TALK":
                                    {
                                        Animation = "mia-normal(b)";
                                        Character = "Mia";
                                        break;
                                    }
                                case "ANI_MIA_CONFIDENT_STAND":
                                    {
                                        Animation = "mia-grinning(a)";
                                        Character = "Mia";
                                        break;
                                    }
                                case "ANI_MIA_CONFIDENT_TALK":
                                    {
                                        Animation = "mia-grinning(b)";
                                        Character = "Mia";
                                        break;
                                    }
                                case "ANI_MIA_SAD_STAND":
                                    {
                                        Animation = "mia-bench-sad(a)";
                                        Character = "Mia";
                                        break;
                                    }
                                case "ANI_MIA_SAD_TALK":
                                    {
                                        Animation = "mia-bench-sad(b)";
                                        Character = "Mia";
                                        break;
                                    }
                                case "ANI_MIA_SHOCK_STAND":
                                    {
                                        Animation = "mia-bench-wut(a)";
                                        Character = "Mia";
                                        break;
                                    }
                                case "ANI_MIA_SHOCK_TALK":
                                    {
                                        Animation = "mia-bench-wut(b)";
                                        Character = "Mia";
                                        break;
                                    }
                                case "ANI_LARRY_CRY_STAND":
                                    {
                                        Animation = "larry-nervous(a)";
                                        Character = "Larry";
                                        break;
                                    }
                                case "ANI_LARRY_CRY_TALK":
                                    {
                                        Animation = "larry-nervous(b)";
                                        Character = "Larry";
                                        break;
                                    }
                                case "ANI_LARRY_TUMB_UP_STAND":
                                    {
                                        Animation = "larry-thumbs up(a)";
                                        Character = "Larry";
                                        break;
                                    }
                                case "ANI_LARRY_THUMB_UP_STAND":
                                    {
                                        Animation = "larry-thumbs up(a)";
                                        Character = "Larry";
                                        break;
                                    }
                                case "ANI_LARRY_TUMB_UP_TALK":
                                    {
                                        Animation = "larry-thumps up(b)";
                                        Character = "Larry";
                                        break;
                                    }
                                case "ANI_LARRY_THUMB_UP_TALK":
                                    {
                                        Animation = "larry-thumps up(b)";
                                        Character = "Larry";
                                        break;
                                    }
                                case "ANI_JUDGE_STAND":
                                    {
                                        Animation = "judge-normal(a)";
                                        Character = "Judge";
                                        break;
                                    }
                                case "ANI_JUDGE_TALK":
                                    {
                                        Animation = "judge-normal(b)";
                                        Character = "Judge";
                                        break;
                                    }
                                case "ANI_JUDGE_EYES_CLOSED_STAND":
                                    {
                                        Animation = "_judge-thinking";
                                        Character = "Judge";
                                        break;
                                    }
                                case "ANI_JUDGE_EYES_CLOSED_TALK":
                                    {
                                        Animation = "judge-thinking";
                                        Character = "Judge";
                                        break;
                                    }
                                case "ANI_JUDGE_ANGRY_STAND":
                                    {
                                        Animation = "judge-warning(a)";
                                        Character = "Judge";
                                        break;
                                    }
                                case "ANI_JUDGE_ANGRY_TALK":
                                    {
                                        Animation = "judge-warning(b)";
                                        Character = "Judge";
                                        break;
                                    }
                                case "ANI_PHOENIX_STAND":
                                    {
                                        Animation = "normal(a)";
                                        Character = "Phoenix";
                                        break;
                                    }
                                case "ANI_PHOENIX_TALK":
                                    {
                                        Animation = "normal(b)";
                                        Character = "Phoenix";
                                        break;
                                    }
                                case "ANI_PHOENIX_THINK_STAND":
                                    {
                                        Animation = "Thinking_1";
                                        Character = "Phoenix";
                                        break;
                                    }
                                case "ANI_PHOENIX_THINK_TALK":
                                    {
                                        Animation = "Thinking_2";
                                        Character = "Phoenix";
                                        break;
                                    }
                                case "ANI_PHOENIX_SWEAT_STAND":
                                    {
                                        Animation = "Sweating_1";
                                        Character = "Phoenix";
                                        break;
                                    }
                                case "ANI_PHOENIX_SWEAT_TALK":
                                    {
                                        Animation = "Sweating_2";
                                        Character = "Phoenix";
                                        break;
                                    }
                                case "ANI_PAYNE_STAND":
                                    {
                                        Animation = "_payne-normal";
                                        Character = "Payne";
                                        break;
                                    }
                                case "ANI_PAYNE_TALK":
                                    {
                                        Animation = "payne-normal";
                                        Character = "Payne";
                                        break;
                                    }
                                default:
                                    {
                                        Debug.WriteLine($"ERROR: UNKNOWN ANIMATION: {ev.Arguments[0]}");
                                        break;
                                    }
                            }
                            menu.SetCharacter(Character,Animation);
                            break;
                        }
                    case ScriptEventType.wait:
                        {
                            Thread.Sleep(int.Parse(ev.Arguments[0])*10);
                            break;
                        }
                    case ScriptEventType.hidetextbox:
                        {
                            TextBoxVisible = false;
                            menu.HideText();
                            break;
                        }
                    case ScriptEventType.background:
                        {
                            switch (ev.Arguments[0])
                            {
                                case "BKG_NONE":
                                    {
                                        menu.SetRoom("none");
                                        break;
                                    }
                                case "BKG_COURTROOM_LOBBY":
                                    {
                                        menu.SetRoom("courtroomHall");
                                        break;
                                    }
                                case "BKG_CINDY_MURDER":
                                    {
                                        menu.SetRoom("deadgirl");
                                        break;
                                    }
                                case "BKG_COURTROOM_TALK":
                                    {
                                        menu.PlayScene("gallery-chatter",w);
                                        break;
                                    }
                                case "BKG_COURTROOM_JUDGE":
                                    {
                                        menu.SetRoom("judgestand");
                                        break;
                                    }
                                case "BKG_GAVEL_SLAM_0":
                                    {
                                        menu.PlayScene("gavel-slam",w);
                                        break;
                                    }
                                case "BGK_COURTROOM_PROSECUTION":
                                    {
                                        menu.SetRoom("prosecutorempty");
                                        break;
                                    }
                                case "BKG_COURTROOM_DEFENSE":
                                    {
                                        menu.SetRoom("defenseempty");
                                        menu.ShowBench();
                                        break;
                                    }
                                case "BKG_COURTROOM_PROSECUTION":
                                    {
                                        menu.SetRoom("prosecutorempty");
                                        break;
                                    }
                                case "BKG_COURTROOM_DEFENSE_SIDE":
                                    {
                                        menu.SetRoom("helperstand");
                                        break;
                                    }
                                default:
                                    {
                                        Debug.WriteLine($"ERROR: UNKNOWN BACKGROUND: {ev.Arguments[0]}");
                                        break;
                                    }
                            }
                            if (!(ev.Arguments[0] == "BKG_COURTROOM_DEFENSE"))
                            {
                                menu.HideBench();
                            }
                            break;
                        }
                    case ScriptEventType.sound:
                        {
                            switch (ev.Arguments[0])
                            {
                                case "SFX_SUPERSHOCK":
                                    {
                                        try
                                        {
                                            SfxReader = new WaveFileReader(assetsDir + "sounds/sfx/sfx-supershock.wav");
                                            SfxOut = new WaveOutEvent();
                                            SfxOut.Init(SfxReader);
                                            SfxOut.Play();
                                        } catch (Exception ex)
                                        {
                                            Debug.WriteLine("ERROR: FAILED TO PLAY SFX SOUND");
                                        }
                                        
                                        break;
                                    }
                                case "SFX_LIGHTBULB":
                                    {
                                        try
                                        {
                                            SfxReader = new WaveFileReader(assetsDir + "sounds/sfx/sfx-lightbulb.wav");
                                            SfxOut = new WaveOutEvent();
                                            SfxOut.Init(SfxReader);
                                            SfxOut.Play();
                                        }
                                        catch (Exception ex)
                                        {
                                            Debug.WriteLine("ERROR: FAILED TO PLAY SFX SOUND");
                                        }

                                        break;
                                    }
                                case "SFX_REALIZATION":
                                    {
                                        try
                                        {
                                            SfxReader = new WaveFileReader(assetsDir + "sounds/sfx/sfx-realization.wav");
                                            SfxOut = new WaveOutEvent();
                                            SfxOut.Init(SfxReader);
                                            SfxOut.Play();
                                        }
                                        catch (Exception ex)
                                        {
                                            Debug.WriteLine("ERROR: FAILED TO PLAY SFX SOUND");
                                        }

                                        break;
                                    }
                                default:
                                    {
                                        Debug.WriteLine($"ERROR: UNKNOWN SFX: {ev.Arguments[0]}");
                                        break;
                                    }
                            }
                            break;
                        }
                    case ScriptEventType.music:
                        {
                            switch(ev.Arguments[0]) {
                                case "MUS_COURTROOM_LOUNGE":
                                    {
                                        try
                                        {
                                            MusicReader = new WaveFileReader(assetsDir + "sounds/music/lobby.wav");
                                            MusicOut = new WaveOutEvent();
                                            MusicOut.Init(MusicReader);
                                            MusicOut.Play();
                                        }
                                        catch (Exception ex)
                                        {
                                            Debug.WriteLine("ERROR: FAILED TO PLAY MUSIC SOUND");
                                        }
                                        break;
                                    }
                                case "MUS_TRIAL":
                                    {
                                        try
                                        {
                                            MusicReader = new WaveFileReader(assetsDir + "sounds/music/court_in_session.wav");
                                            MusicOut = new WaveOutEvent();
                                            MusicOut.Init(MusicReader);
                                            MusicOut.Play();
                                        }
                                        catch (Exception ex)
                                        {
                                            Debug.WriteLine("ERROR: FAILED TO PLAY MUSIC SOUND");
                                        }
                                        break;
                                    }
                                case "MUS_NONE":
                                    {
                                        try
                                        {
                                            MusicOut.Pause();
                                        }
                                        catch (Exception ex)
                                        {
                                            Debug.WriteLine("ERROR: FAILED TO PLAY MUSIC SOUND");
                                        }
                                        break;
                                    }
                                default:
                                    {
                                        Debug.WriteLine($"ERROR: UNKNOWN MUSIC: {ev.Arguments[0]}");
                                        break;
                                    }
                            }
                            break;
                        }
                    default:
                        {
                            Debug.WriteLine($"ERROR: UNKNOWN COMMAND: {ev}");
                            break;
                        }
                }
                menu.Render(w);
                w.Update();
                w.Render();
                //Debug.WriteLine(ev);
            }
        }
    }
}
