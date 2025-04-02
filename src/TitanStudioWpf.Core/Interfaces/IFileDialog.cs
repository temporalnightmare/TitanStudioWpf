namespace TitanStudioWpf.Core.Interfaces;

public interface IFileDialog
{
    string OpenFile(string filter, string title);
    string SaveFile(string filter, string title);
}
