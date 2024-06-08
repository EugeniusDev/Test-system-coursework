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
        Test testToPass;
        /// <summary>
        /// Used to determine if window closing confirmation is needed
        /// </summary>
        bool askForClosingComfirmation = true;

        /// <summary>
        /// NameEntry_Window constructor
        /// </summary>
        /// <remarks>All parameters are later passed into TestTaking_Window</remarks>
        /// <param name="questionsList">Test's question list</param>
        /// <param name="currTestInfo">Structure with test's general info</param>
        public NameEntry_Window(Test testToPass)
        {
            this.testToPass = testToPass;
            InitializeComponent();
        }
        /// <summary>
        /// Handling pressed BackToMain_Button
        /// </summary>
        /// <remarks>Calls GoToMainWindow method</remarks>
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
            if (titleContainsDefaultText || testToPass.TestInfo.testTitle == null)
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
        /// <summary>
        /// Handling pressed BeginTest_Button
        /// </summary>
        /// <remarks>Calls TryToBeginTest method</remarks
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
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            askForClosingComfirmation = false;
            Close();
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
                askForClosingComfirmation = false;
                Close();
            }
            catch (ArgumentNullException)
            {
                MessageBox.Show("Введіть ім'я перед тим, як розпочати тест");
            }
        }
        /// <summary>
        /// Handling window closing event
        /// </summary>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // If closing confirmation is not needed, just close the window
            if (!askForClosingComfirmation) return;
            MessageBoxResult result = MessageBox.Show("Ви справді хочете закрити програму?", "Підтвердження закриття вікна", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result.Equals(MessageBoxResult.No))
            {
                // Cancelling closing process
                e.Cancel = true;
            }
        }
    }
}
