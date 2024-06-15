using courseWork_project.DatabaseRelated;
using courseWork_project.DataManipulation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace courseWork_project
{
    /// <summary>
    /// Class for ObservableCollection objects manipulation
    /// </summary>
    public class TestItem
    {
        public string TestTitle { get; set; }
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// <remarks>MainWindow.xaml is used as main menu</remarks>
    public partial class MainWindow : Window
    {
        private readonly ObservableCollection<TestItem> testItems;
        private readonly List<string> transliteratedTestTitles;
        private bool isWindowClosingConfirmationRequired = true;

        public MainWindow()
        {
            InitializeComponent();

            UpdateListOfExistingTests();
            FileReader fileReader = new FileReader();
            transliteratedTestTitles = fileReader.GetExistingTestTitles();

            DisplayTestsInfo();
            testItems = new ObservableCollection<TestItem>();
            TestsListView.ItemsSource = testItems;
            DisplayTestsFromTitles(transliteratedTestTitles);
        }

        private void DisplayTestsInfo()
        {
            TestsInfoTextblock.Text = (transliteratedTestTitles.Count == 0) ? "Немає створених тестів" 
                : "Список тестів:";
        }

        public void DisplayTestsFromTitles(List<string> transliteratedTestTitles)
        {
            foreach (string transTitle in transliteratedTestTitles)
            {
                TestStructs.TestMetadata testMetadata = DataDecoder.GetTestMetadataByTitle(transTitle);
                AddNewListViewRow(testMetadata.testTitle);
            }
        }

        private void AddNewListViewRow(string testTitle)
        {
            TestItem newItem = new TestItem
            {
                TestTitle = testTitle
            };

            testItems.Add(newItem);
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            e.OpenHelpCenterOnF1();
        }

        private void HelpCenter_button_Click(object sender, RoutedEventArgs e)
        {
            WindowCaller.ShowHelpCenter();
        }

        private void Create_button_Click(object sender, RoutedEventArgs e)
        {
            WindowCaller.ShowTestChangeCreatingMode();
            this.CloseWindowAndDisableConfirmationPrompt(ref isWindowClosingConfirmationRequired);
        }

        private void Passing_Button_Click(object sender, RoutedEventArgs e)
        {
            TestItem selectedItem = new TestItem();
            if (GuiObjectsFinder.TryGetTestItemFromValidAncestor(sender, ref selectedItem))
            {
                Test testToPass = DataDecoder.GetTestByTestItem(selectedItem);
                // Prompt for username before passing test
                WindowCaller.ShowNameEntry(testToPass);
                this.CloseWindowAndDisableConfirmationPrompt(ref isWindowClosingConfirmationRequired);
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            TestItem selectedItem = new TestItem();
            if (GuiObjectsFinder.TryGetTestItemFromValidAncestor(sender, ref selectedItem))
            {
                DataEraser.EraseTestFolderByTitle(selectedItem.TestTitle);

                Test testToEdit = DataDecoder.GetTestByTestItem(selectedItem);
                List<ImageManager.ImageMetadata> emptyImages = new List<ImageManager.ImageMetadata>();
                WindowCaller.ShowTestSavingEditingMode(testToEdit, emptyImages);
                this.CloseWindowAndDisableConfirmationPrompt(ref isWindowClosingConfirmationRequired);
            }
        }

        private static void UpdateListOfExistingTests()
        {
            FileReader fileReader = new FileReader();
            List<string> existingTestsTitles = fileReader.GetExistingTestTitles();

            FileWriter fileWriter = new FileWriter();
            fileWriter.WriteListLineByLine(existingTestsTitles);
        }

        private void ResultsButton_Click(object sender, RoutedEventArgs e)
        {
            TestItem selectedItem = new TestItem();
            if (GuiObjectsFinder.TryGetTestItemFromValidAncestor(sender, ref selectedItem))
            {
                List<string> selectedTestResults = DataDecoder
                    .GetTestResultsByTitle(selectedItem.TestTitle);
                ShowTestResults(selectedTestResults);
            }
        }

        private static void ShowTestResults(List<string> selectedTestResults)
        {
            if (selectedTestResults.Count == 0)
            {
                MessageBox.Show("Обраний тест ще ніким не було пройдено",
                    "Історія проходжень тесту порожня");
                return;
            }

            MessageBox.Show(FormTestResultsOutput(selectedTestResults),
                "Історія проходжень тесту");
        }

        private static string FormTestResultsOutput(List<string> selectedTestResults)
        {
            StringBuilder results = new StringBuilder("Результати проходжень тесту:\n");
            foreach (string result in selectedTestResults)
            {
                results.AppendLine(result);
            }

            return results.ToString();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            TestItem selectedItem = new TestItem();
            bool validTestSelected = GuiObjectsFinder
                .TryGetTestItemFromValidAncestor(sender, ref selectedItem);
            if (validTestSelected && TestDeletionConfirmed(selectedItem))
            {
                DataEraser.EraseTestByTestItem(selectedItem);
                ReloadMainWindow();
            }
        }

        private static bool TestDeletionConfirmed(TestItem selectedItem)
        {
            string confirmationPrompt = $"Ви видалите тест \"{selectedItem.TestTitle}\". " +
                $"Ви справді хочете це зробити?";
            MessageBoxResult result = MessageBox.Show(confirmationPrompt,
                "Підтвердження видалення тесту", 
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            return result.Equals(MessageBoxResult.Yes);
        }

        private void ReloadMainWindow()
        {
            WindowCaller.ShowMain();
            this.CloseWindowAndDisableConfirmationPrompt(ref isWindowClosingConfirmationRequired);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (isWindowClosingConfirmationRequired)
            {
                e.GetClosingConfirmation();
            }
        }

        private void TestGroupOptionsSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Grouper.ShowTestsGroup((TestGroupOption)TestGroupOptionsSelector.SelectedIndex, 
                transliteratedTestTitles);
        }
        private void QuestionGroupOptionsSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Grouper.ShowQuestionsGroup((QuestionGroupOption)QuestionGroupOptionsSelector.SelectedIndex, 
                transliteratedTestTitles);
        }

        private void TestSortOptionsSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Sorter.ShowSortedTests((TestSortOption)TestSortOptionsSelector.SelectedIndex,
                transliteratedTestTitles);
        }
        private void QuestionSortOptionsSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Sorter.ShowSortedQuestions((QuestionSortOption)QuestionSortOptionsSelector.SelectedIndex,
                transliteratedTestTitles);
        }

        private void QuestionSearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            QuestionSearchBox.Text = string.Empty;
            QuestionSearchBox.Foreground = new SolidColorBrush(Colors.Black);
        }

        private void VariantSearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            VariantSearchBox.Text = string.Empty;
            VariantSearchBox.Foreground = new SolidColorBrush(Colors.Black);
        }

        private void SearchQuestion_button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string fieldValue = QuestionSearchBox.Text;
                Searcher.ValidateInputForField(fieldValue, "запитання");
                Searcher.ShowQuestionSearchingResults(fieldValue, transliteratedTestTitles);
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SearchVariant_button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string fieldValue = VariantSearchBox.Text;
                Searcher.ValidateInputForField(fieldValue, "варіанту");
                Searcher.ShowVariantSearchingResults(fieldValue, transliteratedTestTitles);
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
