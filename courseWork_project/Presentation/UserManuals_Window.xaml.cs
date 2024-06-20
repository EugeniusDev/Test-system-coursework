using System.Windows;
using System.Windows.Input;
using static courseWork_project.Common.UserManualDictionaries;
namespace courseWork_project
{
    public partial class UserManuals_Window : Window
    {
        public UserManuals_Window()
        {
            InitializeComponent();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
            {
                Close();
            }
        }

        private void MainWindowCombobox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            UpdateInstructions(mainWindowManualMessages[(MainWindowManuals)
                MainWindowCombobox.SelectedIndex]);
        }
        private void TestPassingCombobox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            UpdateInstructions(testPassingManualMessages[(TestPassingManuals)
                TestPassingCombobox.SelectedIndex]);
        }
        private void CreationEditingCombobox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            UpdateInstructions(testChangeManualMessages[(TestChangeManuals)
                CreationEditingCombobox.SelectedIndex]);
        }
        private void TestSavingCombobox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            UpdateInstructions(testSavingManualMessages[(TestSavingManuals)
                TestSavingCombobox.SelectedIndex]);
        }

        private void UpdateInstructions(string text)
        {
            InfoText.Text = text;
        }
    }
}
