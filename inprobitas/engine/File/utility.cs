namespace inprobitas.engine.Files
{
    static class Utility
    {
        public static string workingDirectory = Environment.CurrentDirectory;
        public static string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
        public static string fontDirectory = projectDirectory+"/game/gui/resources/Font";
        public static string imageDirectory = projectDirectory + "/game/gui/resources/Image";
    }
}