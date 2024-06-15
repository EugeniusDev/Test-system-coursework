using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using static courseWork_project.TestStructs;

namespace courseWork_project
{
    /// <summary>
    /// Interaction logic for NameEntry_Window.xaml
    /// </summary>
    /// <remarks> NameEntry_Window.xaml is used to prompt user's name and to open TestTaking_Window</remarks>
    public partial class NameEntry_Window : Window
    {
        private readonly Test testToPass;
        /// <summary>
        /// Used to determine if window closing confirmation is needed
        /// </summary>
        bool isWindowClosingConfirmationRequired = true;

        public NameEntry_Window(Test testToPass)
        {
            this.testToPass = testToPass;
            InitializeComponent();
        }

        private void BackToMain_Button_Click(object sender, RoutedEventArgs e)
        {
            GoToMainWindow();
        }
        /// <summary>
        /// Handling pressing on the username prompting TextBox
        /// </summary>
        private void UsernameTextBlock_GotFocus(object sender, RoutedEventArgs e)
        {
            bool titleContainsDefaultText = UsernameTextBlock != null
                && string.Compare(UsernameTextBlock.Text, "Введіть ім'я тут") == 0;
            if (titleContainsDefaultText || testToPass.TestMetadata.testTitle == null)
            {
                UsernameTextBlock.Foreground = new SolidColorBrush(Colors.Black);
                UsernameTextBlock.Text = string.Empty;
            }
        }
        /// <summary>
        /// Handling loss of focus on the username prompting TextBox
        /// </summary>
        /// <remarks>If field is empty, refills it with default data</remarks>
        private void UsernameTextBlock_LostFocus(object sender, RoutedEventArgs e)
        {
            bool fieldIsEmpty = UsernameTextBlock != null && string.IsNullOrWhiteSpace(UsernameTextBlock.Text);
            if (fieldIsEmpty)
            {
                UsernameTextBlock.Text = "Введіть ім'я тут";
                UsernameTextBlock.Foreground = new SolidColorBrush(Colors.DarkGray);
            }
        }

        private void BeginTest_Button_Click(object sender, RoutedEventArgs e)
        {
            TryToBeginTest();
        }
        /// <summary>
        /// Handling pressed keyboard keys
        /// </summary>
        /// <remarks>F1 - user manual;
        /// Esc - back to MainWindow;
        /// Enter - call TryToBeginTest method</remarks>
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
        /// <summary>
        /// Opens MainWindow and closes current window
        /// </summary>
        private void GoToMainWindow()
        {
            WindowCaller.ShowMain();
            this.CloseWindowAndDisableConfirmationPrompt(ref isWindowClosingConfirmationRequired);
        }

        /// <summary>
        /// Attempts to open TestTaking_Window
        /// </summary>
        private void TryToBeginTest()
        {
            try
            {
                if (UsernameTextBlock == null) throw new ArgumentNullException();
                string userName = UsernameTextBlock.Text;
                bool fieldIsEmptyOrDefault = string.IsNullOrWhiteSpace(userName)
                    || string.Compare(UsernameTextBlock.Text, "Введіть ім'я тут") == 0;
                if (fieldIsEmptyOrDefault) throw new ArgumentNullException();

                WindowCaller.ShowTestTaking(testToPass, userName);
                this.CloseWindowAndDisableConfirmationPrompt(ref isWindowClosingConfirmationRequired);
            }
            catch (ArgumentNullException)
            {
                MessageBox.Show("Введіть ім'я перед тим, як розпочати тест");
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (isWindowClosingConfirmationRequired)
            {
                e.GetClosingConfirmation();
            }
        }
    }
}
