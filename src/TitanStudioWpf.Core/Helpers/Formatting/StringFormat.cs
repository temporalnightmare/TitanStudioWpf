namespace TitanStudioWpf.Core.Helpers.Formatting;

public class StringFormat
{
    // Handle other paths by getting everything after the last matching folder
    static readonly string[] commonFolders = {
        "AI", "AnimSystem", "Animation", "Arena", "Audience", "Audio", "Belts", "Boot", "CAA", "CAC", "CAE", "CAM", "CAS", "CAV",
        "Camera", "Championship_Titles", "Characters", "CommonUI", "CreateShow", "Curves", "Cutscene", "Cutscene_Light", "Director",
        "Effects", "Entrances", "Environment", "Features", "FlowEditor", "GMMode", "Gameplay", "GhostWriter", "Global", "Havok",
        "Hide", "IK", "LightShow", "Logo", "MITB", "MSC", "MatchCreator", "MatchMake", "MoveData", "Movies", "MyFMode", "Network",
        "Orion", "Particle", "Program", "Promo", "Props", "Roster", "Rules", "ScreenSpaceFonts", "Sdb", "SharedGameplay", "Stable",
        "StateMachine", "Subtitles", "UI", "WWEUniverse", "YukesShaders", "gameresource", "sound", "Root" };

    public static string ToLowercase(string input)
    {
        return input.ToLower();
    }

    public static string StripPath(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        foreach (var folder in commonFolders)
        {
            var folderPath = "\\" + folder + "\\";
            if (input.Contains(folderPath))
            {
                return input[(input.LastIndexOf(folderPath) + 1)..];
            }
        }

        return input;
    }

    public static string ConvertSlashes(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        return input.Replace('\\', '/');
    }
}
