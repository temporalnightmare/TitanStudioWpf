namespace TitanStudioWpf.Core.Models;

public class CharacterMapping
{
    public uint Id { get; set; }
    public byte AttireNumber { get; set; }
    public string CharacterFolderPath { get; set; } = string.Empty;
    public string CharacterAttirePath { get; set; } = string.Empty;
    public string CharacterBaseModelFile { get; set; } = string.Empty;
    public string CharacterBaseMaterialsFile { get; set; } = string.Empty;
    public string CASBaseModelFile { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public byte Unknown2 { get; set; }
}
