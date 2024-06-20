using System.Windows;

namespace courseWork_project.Common.StaticResources
{
    internal static class MessageBoxes
    {
        public static void ShowInformation(string message)
        {
            ShowSampleMessageBox(message, MessageBoxImage.Information);
        }

        private static void ShowSampleMessageBox(string message, MessageBoxImage image)
        {
            MessageBox.Show(message, string.Empty,
                MessageBoxButton.OK, image);
        }

        public static void ShowWarning(string message)
        {
            ShowSampleMessageBox(message, MessageBoxImage.Warning);
        }

        public static void ShowError(string message)
        {
            ShowSampleMessageBox(message, MessageBoxImage.Error);
        }

        public static MessageBoxResult ShowConfirmationPrompt(string message, string title)
        {
            return MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
        }
    }
}
