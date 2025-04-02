namespace TitanStudioWpf.Core.Models;

public class Soundtrack
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ulong ImagePath { get; set; }
}
