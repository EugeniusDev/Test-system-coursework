using courseWork_project.DatabaseRelated;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        /// <summary>
        /// ObservableCollection for ListView
        /// </summary>
        private readonly ObservableCollection<TestItem> testItems;

        private readonly List<string> transliteratedTestTitles;

        bool isWindowClosingConfirmationRequired = true;

        public MainWindow()
        {
            InitializeComponent();
            // Getting list of existing tests
            FileReader fileReader = new FileReader();
            transliteratedTestTitles = fileReader.GetExistingTestTitles();

            testItems = new ObservableCollection<TestItem>();
            TestsInfoTextblock.Text = (transliteratedTestTitles.Count == 0) ? "Немає створених тестів" : "Список тестів:";
            TestsListView.ItemsSource = testItems;
            DisplayTestsFromTitles(transliteratedTestTitles);
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
            isWindowClosingConfirmationRequired = false;
            Close();
        }

        private void Taking_Button_Click(object sender, RoutedEventArgs e)
        {
            TestItem selectedItem = new TestItem();
            if (GuiObjectsFinder.TryGetTestItemFromValidAncestor(sender, ref selectedItem))
            {
                Test testToPass = new Test(DataDecoder.GetQuestionMetadatasByTitle(selectedItem.TestTitle),
                    DataDecoder.GetTestMetadataByTitle(selectedItem.TestTitle));
                WindowCaller.ShowNameEntry(testToPass);
                isWindowClosingConfirmationRequired = false;
                Close();
            }
        }
        /// <summary>
        /// Handling pressed button of type EditButton
        /// </summary>
        /// <remarks>Opens TestSaving_Window in editing mode</remarks>
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            TestItem selectedItem = new TestItem();
            if (GuiObjectsFinder.TryGetTestItemFromValidAncestor(sender, ref selectedItem))
            {
                List<TestStructs.QuestionMetadata> questionsToEdit = DataDecoder.GetQuestionMetadatasByTitle(selectedItem.TestTitle);
                TestStructs.TestMetadata infoOfTestToEdit = DataDecoder.GetTestMetadataByTitle(selectedItem.TestTitle);

                DataEraser.EraseTestFolderByTitle(selectedItem.TestTitle);

                // Updating list of transliterated test titles
                UpdateListOfExistingTests();

                Test testToEdit = new Test(questionsToEdit, infoOfTestToEdit);
                List<ImageManager.ImageMetadata> emptyImages = new List<ImageManager.ImageMetadata>();
                WindowCaller.ShowTestSavingEditingMode(testToEdit, emptyImages);
                isWindowClosingConfirmationRequired = false;
                Close();
            }
        }

        private void UpdateListOfExistingTests()
        {
            FileReader fileReader = new FileReader();
            List<string> existingTestsTitles = fileReader.GetExistingTestTitles();

            FileWriter fileWriter = new FileWriter();
            fileWriter.WriteListLineByLine(existingTestsTitles);
        }

        /// <summary>
        /// Handling pressed button of type ResultsButton
        /// </summary>
        /// <remarks>Shows results of selected test's passings in MessageBox</remarks>
        private void ResultsButton_Click(object sender, RoutedEventArgs e)
        {
            TestItem selectedItem = new TestItem();
            if (GuiObjectsFinder.TryGetTestItemFromValidAncestor(sender, ref selectedItem))
            {
                string transliteratedTestTitle = DataDecoder.TransliterateToEnglish(selectedItem.TestTitle);
                // Getting up-to-date list of data about passing selected test
                // TODO less abstraction level than needed, make it the problem of reader
                string pathOfResultsDirectory = Properties.Settings.Default.testResultsDirectory;
                FileReader fileReader = new FileReader(pathOfResultsDirectory, $"{transliteratedTestTitle}.txt");
                List<string> currTestResultsList = fileReader.GetFileContentInLines();
                // Empty list means no one tried to pass the test yet
                if(currTestResultsList.Count == 0)
                {
                    MessageBox.Show("Обраний тест ще ніким не було пройдено", "Історія проходжень тесту порожня");
                    return;
                }
                // Forming the output
                string resultsToShow = $"Результати проходжень тесту:\n";
                foreach(string result in currTestResultsList)
                {
                    resultsToShow = string.Concat(resultsToShow, $"{result}\n");
                }
                MessageBox.Show(resultsToShow, "Історія проходжень тесту");
            }
        }
        /// <summary>
        /// Handling pressed button of type DeleteButton
        /// </summary>
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            TestItem selectedItem = new TestItem();
            if (GuiObjectsFinder.TryGetTestItemFromValidAncestor(sender, ref selectedItem))
            {
                string confirmationString = $"Ви видалите тест \"{selectedItem.TestTitle}\". Ви справді хочете це зробити?";
                MessageBoxResult result = MessageBox.Show(confirmationString,
                    "Підтвердження видалення тесту", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result.Equals(MessageBoxResult.Yes))
                {
                    // Deleting selected test's related data
                    DataEraser.EraseTestFolderByTitle(selectedItem.TestTitle);
                    DataEraser.EraseTestPassingDataByTitle(selectedItem.TestTitle);
                    ImageManager.ImagesCleanupByTitle(selectedItem.TestTitle);
                    // Reloading MainWindow to update the list of tests
                    WindowCaller.ShowMain();
                    isWindowClosingConfirmationRequired = false;
                    Close();
                }
            }
        }

        public void DisplayTestsFromTitles(List<string> existingTestsTitles)
        {
            foreach (string testTitleTransliterated in existingTestsTitles)
            {
                string notTransliteratedTitle = DataDecoder.GetTestMetadataByTitle(testTitleTransliterated)
                    .testTitle;
                AddNewListViewRow(notTransliteratedTitle);
            }
        }
        /// <summary>
        /// Adds new ListView row with given test title
        /// </summary>
        /// <param name="testTitle">Not transliterated test title</param>
        private void AddNewListViewRow(string testTitle)
        {
            TestItem newItem = new TestItem
            {
                TestTitle = testTitle
            };

            testItems.Add(newItem);
        }
        /// <summary>
        /// Handling window closing event
        /// </summary>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // If closing confirmation is not needed, just close the window
            if (!isWindowClosingConfirmationRequired) return;

            e.GetClosingConfirmation();
        }

        private void TestMetadataSortOptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Sorter.SortTests(TestMetadataSortOptions.SelectedIndex, transliteratedTestTitles);
        }
        private void QuestionMetadataSortOptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Sorter.SortQuestions(QuestionMetadataSortOptions.SelectedIndex, transliteratedTestTitles);
        }

        private void TestMetadataGroupOptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Grouper.GroupTests(TestMetadataGroupOptions.SelectedIndex, transliteratedTestTitles);
        }
        private void QuestionMetadataGroupOptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Grouper.GroupQuestions(QuestionMetadataGroupOptions.SelectedIndex, transliteratedTestTitles);
        }
        /// <summary>
        /// Handling click in QuestionMetadatas search field
        /// </summary>
        private void QuestionSearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            QuestionSearchBox.Text = string.Empty;
            QuestionSearchBox.Foreground = new SolidColorBrush(Colors.Black);
        }
        /// <summary>
        /// Handling click in variants search field
        /// </summary>
        private void VariantSearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            VariantSearchBox.Text = string.Empty;
            VariantSearchBox.Foreground = new SolidColorBrush(Colors.Black);
        }
        /// <summary>
        /// Handling click on SearchQuestion_button
        /// </summary>
        private void SearchQuestion_button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string questionToSearch = QuestionSearchBox.Text;
                bool fieldIsEmptyOrDefault = questionToSearch == string.Empty || string.Compare(questionToSearch, "Пошук запитання тесту") == 0;
                if (fieldIsEmptyOrDefault)
                {
                    throw new ArgumentNullException();
                }

                foreach(string testTitle in transliteratedTestTitles)
                {
                    List<TestStructs.QuestionMetadata> currentQuestionMetadata = DataDecoder.GetQuestionMetadatasByTitle(testTitle);
                    TestStructs.QuestionMetadata foundQuestion = currentQuestionMetadata.Find(a => string.Compare(a.question.ToLower(), questionToSearch.ToLower()) == 0);
                    // If a structure with required title is found
                    if (foundQuestion.question != null)
                    {
                        // Forming output
                        string foundTestTitle = DataDecoder.GetTestMetadataByTitle(testTitle).testTitle;
                        MessageBox.Show($"Введене запитання знайдено в тесті \"{foundTestTitle}\":\n"
                        + $"Запитання: {foundQuestion.question}; "
                        + $"Всього варіантів: {foundQuestion.variants.Count}; "
                        + $"Правильних варіантів: {foundQuestion.correctVariantsIndeces.Count}\n",
                        "Результат пошуку запитання тесту");
                        return;
                    }
                }

                MessageBox.Show($"Запитання \"{questionToSearch}\" не знайдено. Перевірте правильність написання та спробуйте ще раз",
                    "Результат пошуку запитання тесту");
            }
            catch(ArgumentNullException)
            {
                MessageBox.Show("Будь ласка, заповніть поле пошуку запитання тесту");
            }
        }
        /// <summary>
        /// Handling click on SearchVariant_button
        /// </summary>
        private void SearchVariant_button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string variantToSearch = VariantSearchBox.Text;
                bool fieldIsEmptyOrDefault = variantToSearch == string.Empty || string.Compare(variantToSearch, "Пошук запитання тесту") == 0;
                if (fieldIsEmptyOrDefault)
                {
                    throw new ArgumentNullException();
                }

                foreach (string testTitle in transliteratedTestTitles)
                {
                    List<TestStructs.QuestionMetadata> currentTestQuestions = DataDecoder.GetQuestionMetadatasByTitle(testTitle);
                    foreach(TestStructs.QuestionMetadata currentQuestion in currentTestQuestions)
                    {
                        foreach(string variant in currentQuestion.variants)
                        {
                            if (string.Compare(variant.ToLower(), variantToSearch.ToLower()) == 0){
                                // If searched variant was found, form the output
                                string foundVariantTestTitle = DataDecoder.GetTestMetadataByTitle(testTitle).testTitle;
                                MessageBox.Show($"Введений варіант знайдено в тесті \"{foundVariantTestTitle}\",\n"
                                + $"в запитанні \"{currentQuestion.question}\"",
                                "Результат пошуку запитання тесту");
                                return;
                            }
                        }
                    }
                }
                // If searched variant was not found
                MessageBox.Show($"Запитання \"{variantToSearch}\" не знайдено. Перевірте правильність написання та спробуйте ще раз",
                    "Результат пошуку запитання тесту");
            }
            catch (ArgumentNullException)
            {
                MessageBox.Show("Будь ласка, заповніть поле пошуку запитання тесту");
            }
        }
    }
}
