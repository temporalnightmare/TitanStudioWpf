using System.Windows;
using System.Windows.Media;

namespace TitanStudioWpf
{
    public partial class App : Application
    {
        public App()
        {
            // Constructor can be empty since initialization is handled in Program.cs
        }

        public static void UpdateGlobalFont(string fontName)
        {
            if (string.IsNullOrEmpty(fontName)) return;

            Application.Current.Dispatcher.Invoke(() =>
            {
                var newFont = new FontFamily(fontName);
                Application.Current.Resources["GlobalFont"] = newFont;

                // Force the window to re-evaluate its resources
                foreach (Window window in Application.Current.Windows)
                {
                    window.InvalidateVisual();
                }
            });
        }
    }
}