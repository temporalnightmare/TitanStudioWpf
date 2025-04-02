using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitanStudioWpf.Core.Helpers.IO;

public class FileSystem
{

    public static string BrowseFile(string filter, string? title = null)
    {
        var dialog = new OpenFileDialog()
        {
            Filter = filter,
            Title = title,
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer)
        };

        return dialog.ShowDialog() == true ? dialog.FileName : string.Empty;
    }

    public static string SetFile(string filter, string? title = null)
    {
        var dialog = new SaveFileDialog()
        {
            Filter = filter,
            Title = title,
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer)
        };

        return dialog.ShowDialog() == true ? dialog.FileName : string.Empty;
    }

    public static string BrowseFolder()
    {
        var dialog = new OpenFolderDialog()
        {
            Title = "Select a folder",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer)

        };
        return dialog.ShowDialog() == true ? dialog.FolderName : string.Empty;

    }
}
