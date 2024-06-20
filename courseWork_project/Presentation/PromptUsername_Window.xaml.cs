using courseWork_project.Common.StaticResources;
using System.Windows;
using System.Windows.Input;

namespace courseWork_project
{
    public partial class PromptUsername_Window : Window
    {
        private const string defaultUsernameText = "Введіть ім'я тут";
        private readonly Test testToPass;

        public PromptUsername_Window(Test testToPass)
        {
            this.testToPass = testToPass;
            InitializeComponent();
        }

        private void BackToMain_Button_Click(object sender, RoutedEventArgs e)
        {
            GoToMainWindow();
        }

        private void GoToMainWindow()
        {
            WindowCaller.ShowMain();
            Close();
        }

        private void UsernameTextBlock_GotFocus(object sender, RoutedEventArgs e)
        {
            UsernameTextBlock.Foreground = ColorBrushes.Black;
            if (FieldContainsDefaultText() || IsFieldEmpty())
            {
                UsernameTextBlock.Text = string.Empty;
            }
        }

        private bool FieldContainsDefaultText()
        {
            return UsernameTextBlock.Text.Equals(defaultUsernameText);
        }

        private bool IsFieldEmpty()
        {
            return string.IsNullOrWhiteSpace(UsernameTextBlock.Text);
        }

        private void UsernameTextBlock_LostFocus(object sender, RoutedEventArgs e)
        {
            UsernameTextBlock.Foreground = ColorBrushes.DarkGray;
            if (IsFieldEmpty())
            {
                UsernameTextBlock.Text = defaultUsernameText;
            }
        }

        private void BeginTest_Button_Click(object sender, RoutedEventArgs e)
        {
            TryToBeginTest();
        }

        private void TryToBeginTest()
        {
            if (FieldContainsDefaultText() || IsFieldEmpty())
            {
                MessageBoxes.ShowWarning("Введіть ім'я перед тим, як розпочати тест");
                return;
            }

            string userName = UsernameTextBlock.Text;
            WindowCaller.ShowTestTaking(testToPass, userName);
            Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            e.OpenHelpCenterOnF1();

            if(e.Key == Key.Escape)
            {
                GoToMainWindow();
            }

            if(e.Key == Key.Enter)
            {
                TryToBeginTest();
            }
        }
    }
}
