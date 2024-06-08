using courseWork_project.DatabaseRelated;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
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
        private ObservableCollection<TestItem> testItems;

        private List<string> testTitles;
        private List<string> transliteratedTestTitles;

        bool isWindowClosingConfirmationRequired = true;

        public MainWindow()
        {
            InitializeComponent();
            // Getting list of existing tests
            FileReader fileReader = new FileReader();
            transliteratedTestTitles = fileReader.UpdateListOfExistingTestsPaths();

            testItems = new ObservableCollection<TestItem>();
            TestsInfoTextblock.Text = (transliteratedTestTitles.Count == 0) ? "Немає створених тестів" : "Список тестів:";
            TestsListView.ItemsSource = testItems;
            DisplayTestsFromTitles(transliteratedTestTitles);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            e.OpenHelpCenterOnF1();
        }
        /// <summary>
        /// Handling pressed HelpCenter_button
        /// </summary>
        /// <remarks>Opens HelpCenter_Window</remarks>
        private void HelpCenter_button_Click(object sender, RoutedEventArgs e)
        {
            WindowCaller.ShowHelpCenter();
        }
        /// <summary>
        /// Handling pressed Create_button
        /// </summary>
        /// <remarks>Opens TestChange_Window in creating mode</remarks>
        private void Create_button_Click(object sender, RoutedEventArgs e)
        {
            WindowCaller.ShowTestChange();
            isWindowClosingConfirmationRequired = false;
            Close();
        }
        /// <summary>
        /// Handling pressed button of type Taking_Button
        /// </summary>
        /// <remarks>Begins passing procedure of selected test</remarks>
        private void Taking_Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            // Find the parent ListViewItem of the clicked button
            ListViewItem itemContainer = GuiHelper.FindAncestor<ListViewItem>(button);
            if (button != null && itemContainer.DataContext is TestItem selectedItem)
            {
                Test testToPass = new Test(DataDecoder.GetQuestionsByTitle(selectedItem.TestTitle),
                    DataDecoder.GetTestInfoByTitle(selectedItem.TestTitle));
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
            Button button = sender as Button;
            // Find the parent ListViewItem of the clicked button
            ListViewItem itemContainer = GuiHelper.FindAncestor<ListViewItem>(button);
            if (button != null && itemContainer.DataContext is TestItem selectedItem)
            {
                List<TestStructs.Question> questionsToEdit = DataDecoder.GetQuestionsByTitle(selectedItem.TestTitle);
                TestStructs.TestInfo infoOfTestToEdit = DataDecoder.GetTestInfoByTitle(selectedItem.TestTitle);

                DataEraser.EraseTestFolder(selectedItem.TestTitle);

                // Updating list of transliterated test titles
                FileReader fileReader = new FileReader();
                List<string> allTestsList = fileReader.UpdateListOfExistingTestsPaths();
                FileWriter fileWriter = new FileWriter();
                fileWriter.WriteListInFileByLines(allTestsList);

                List<ImageManager.ImageInfo> emptyImageInfos = new List<ImageManager.ImageInfo>();
                TestSaving_Window testSaving_Window = new TestSaving_Window(questionsToEdit, emptyImageInfos, infoOfTestToEdit);
                testSaving_Window.Show();
                isWindowClosingConfirmationRequired = false;
                Close();
            }
        }
        /// <summary>
        /// Handling pressed button of type ResultsButton
        /// </summary>
        /// <remarks>Shows results of selected test's passings in MessageBox</remarks>
        private void ResultsButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            // Find the parent ListViewItem of the clicked button
            ListViewItem itemContainer = GuiHelper.FindAncestor<ListViewItem>(button);
            if (button != null && itemContainer.DataContext is TestItem selectedItem)
            {
                string transliteratedTestTitle = DataDecoder.TransliterateToEnglish(selectedItem.TestTitle);
                // Getting up-to-date list of data about passing selected test
                // TODO less abstraction level than needed, make it the problem of reader
                string pathOfResultsDirectory = ConfigurationManager.AppSettings["testResultsDirPath"];
                FileReader fileReader = new FileReader(pathOfResultsDirectory, $"{transliteratedTestTitle}.txt");
                List<string> currTestResultsList = fileReader.ReadAndReturnLines();
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
            Button button = sender as Button;
            // Find the parent ListViewItem of the clicked button
            ListViewItem itemContainer = GuiHelper.FindAncestor<ListViewItem>(button);
            if (button != null && itemContainer.DataContext is TestItem selectedItem)
            {
                string confirmationString = $"Ви видалите тест \"{selectedItem.TestTitle}\". Ви справді хочете це зробити?";
                MessageBoxResult result = MessageBox.Show(confirmationString,
                    "Підтвердження видалення тесту", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result.Equals(MessageBoxResult.Yes))
                {
                    // Deleting selected test's related data
                    DataEraser.EraseTestFolder(selectedItem.TestTitle);
                    DataEraser.ErasePassingData(selectedItem.TestTitle);
                    ImageManager.ImagesCleanup(selectedItem.TestTitle);
                    // Reloading MainWindow to update the list of tests
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    isWindowClosingConfirmationRequired = false;
                    Close();
                }
            }
        }
        /// <summary>
        /// Puts list of test titles into GUI
        /// </summary>
        /// <param name="existingTestsTitles">List of transliterated test titles (a set of paths to databases)</param>
        public void DisplayTestsFromTitles(List<string> existingTestsTitles)
        {
            foreach (string testTitleTransliterated in existingTestsTitles)
            {
                string notTransliteratedTitle = DataDecoder.GetTestInfoByTitle(testTitleTransliterated)
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
            MessageBoxResult result = MessageBox.Show("Ви справді хочете закрити програму?", "Підтвердження закриття вікна", 
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result.Equals(MessageBoxResult.No))
            {
                // Cancelling closing process
                e.Cancel = true;
            }
        }
        /// <summary>
        /// Handling choosing anything from "TestInfoSortOptions" ComboBox
        /// </summary>
        private void TestInfoSortOptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Sorter.SortTests(TestInfoSortOptions.SelectedIndex, transliteratedTestTitles);
        }
        /// <summary>
        /// Handling choosing anything from "QuestionSortOptions" ComboBox
        /// </summary>
        private void QuestionSortOptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Sorter.SortQuestions(QuestionSortOptions.SelectedIndex, transliteratedTestTitles);
        }
        /// <summary>
        /// Handling choosing anything from "TestInfoGroupOptions" ComboBox
        /// </summary>
        private void TestInfoGroupOptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Grouper.GroupTests(TestInfoGroupOptions.SelectedIndex, transliteratedTestTitles);
        }
        /// <summary>
        /// Handling choosing anything from "QuestionGroupOptions" ComboBox
        /// </summary>
        private void QuestionGroupOptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Grouper.GroupQuestions(QuestionGroupOptions.SelectedIndex, transliteratedTestTitles);
        }
        /// <summary>
        /// Handling clock in Questions search field
        /// </summary>
        private void QuestionSearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            QuestionSearchBox.Text = string.Empty;
            QuestionSearchBox.Foreground = new SolidColorBrush(Colors.Black);
        }
        /// <summary>
        /// Handling clock in variants search field
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
                    List<TestStructs.Question> currentTestQuestions = DataDecoder.GetQuestionsByTitle(testTitle);
                    TestStructs.Question foundQuestion = currentTestQuestions.Find(a => string.Compare(a.question.ToLower(), questionToSearch.ToLower()) == 0);
                    // If a structure with required title is found
                    if (foundQuestion.question != null)
                    {
                        // Forming output
                        string foundQuestionTestTitle = DataDecoder.GetTestInfoByTitle(testTitle).testTitle;
                        MessageBox.Show($"Введене запитання знайдено в тесті \"{foundQuestionTestTitle}\":\n"
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
                    List<TestStructs.Question> currentTestQuestions = DataDecoder.GetQuestionsByTitle(testTitle);
                    foreach(TestStructs.Question currentQuestion in currentTestQuestions)
                    {
                        foreach(string variant in currentQuestion.variants)
                        {
                            if (string.Compare(variant.ToLower(), variantToSearch.ToLower()) == 0){
                                // If searched variant was found, form the output
                                string foundVariantTestTitle = DataDecoder.GetTestInfoByTitle(testTitle).testTitle;
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
