using System.Windows.Media;

namespace courseWork_project
{
    internal static class ColorBrushes
    {
        public static readonly SolidColorBrush DarkGreen = new SolidColorBrush(Colors.DarkGreen);
        public static readonly SolidColorBrush DarkRed = new SolidColorBrush(Colors.DarkRed);
        public static readonly SolidColorBrush DarkGray = new SolidColorBrush(Colors.DarkGray);
        public static readonly SolidColorBrush White = new SolidColorBrush(Colors.White);
        public static readonly SolidColorBrush Black = new SolidColorBrush(Colors.Black);
        public static readonly Brush VariantBackground = (Brush)new BrushConverter().ConvertFrom("#fff0f0");
    }
}
