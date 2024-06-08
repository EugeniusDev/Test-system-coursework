using System.Windows.Input;

namespace courseWork_project
{
    public static class WindowExtensionMethods
    {
        public static void OpenHelpCenterOnF1(this KeyEventArgs keyEventArgs)
        {
            if (keyEventArgs.Key == Key.F1)
            {
                WindowCaller.ShowHelpCenter();
            }
        }
    }
}
