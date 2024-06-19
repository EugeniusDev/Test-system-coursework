using System.Windows;
using System.Windows.Input;

namespace courseWork_project
{
    /// <summary>
    /// Interaction logic for NameEntry_Window.xaml
    /// </summary>
    /// <remarks> NameEntry_Window.xaml is used to prompt user's name and to open TestTaking_Window</remarks>
    public partial class NameEntry_Window : Window
    {
        private readonly Test testToPass;

        public NameEntry_Window(Test testToPass)
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
            return UsernameTextBlock.Text.Equals("Введіть ім'я тут");
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
                UsernameTextBlock.Text = "Введіть ім'я тут";
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
                MessageBox.Show("Введіть ім'я перед тим, як розпочати тест");
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
