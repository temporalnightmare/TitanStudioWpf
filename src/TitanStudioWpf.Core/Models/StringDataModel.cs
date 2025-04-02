namespace TitanStudioWpf.Core.Models;

public class StringDataModel
{
    public int fileCount { get; set; }

    public DatabaseString[]? DatabaseStrings { get; set; }
}

public class DatabaseString
{
    public int Index { get; set; }
    public uint Id { get; set; }
    public int Offset { get; set; }
    public string StringText { get; set; } = string.Empty;
    public int Length { get; set; }

}
