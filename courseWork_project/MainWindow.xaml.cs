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
        /// <summary>
        /// List of transliterated test titles
        /// </summary>
        private List<string> existingTestsTitles;
        /// <summary>
        /// List of not transliterated test titles
        /// </summary>
        private List<string> testTitles;
        /// <summary>
        /// Path to a directory that contains a file with list of transliterated test titles
        /// </summary>
        private readonly string directoryPathToTestsList = ConfigurationManager.AppSettings["testTitlesDirPath"];
        /// <summary>
        /// Path to a file with list of transliterated test titles
        /// </summary>
        private readonly string filePathToTestsList = ConfigurationManager.AppSettings["testTitlesFilePath"];
        /// <summary>
        /// Used to determine if window closing confirmation is needed
        /// </summary>
        bool askForClosingComfirmation = true;
        /// <summary>
        /// Parameterless MainWindow constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            // Getting list of existing tests
            FileReader fileReader = new FileReader(directoryPathToTestsList, $"{filePathToTestsList}.txt");
            existingTestsTitles = fileReader.UpdateListOfExistingTestsPaths();

            testItems = new ObservableCollection<TestItem>();
            TestsInfoTextblock.Text = (existingTestsTitles.Count == 0) ? "Немає створених тестів" : "Список тестів:";
            TestsListView.ItemsSource = testItems;
            GetListAndPutItInGUI(existingTestsTitles);
        }
        /// <summary>
        /// Handling pressed keyboard keys
        /// </summary>
        /// <remarks>F1 - user manual</remarks>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.F1)
            {
                HelpCenter_Window helpCenter = new HelpCenter_Window();
                helpCenter.Show();
            }
        }
        /// <summary>
        /// Handling pressed HelpCenter_button
        /// </summary>
        /// <remarks>Opens HelpCenter_Window</remarks>
        private void HelpCenter_button_Click(object sender, RoutedEventArgs e)
        {
            HelpCenter_Window helpCenter_Window = new HelpCenter_Window();
            helpCenter_Window.Show();
        }
        /// <summary>
        /// Handling pressed Create_button
        /// </summary>
        /// <remarks>Opens TestChange_Window in creating mode</remarks>
        private void Create_button_Click(object sender, RoutedEventArgs e)
        {
            TestChange_Window testChange_Window = new TestChange_Window();
            testChange_Window.Show();
            askForClosingComfirmation = false;
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
                List<TestStructs.Question> questionsToTake = DataDecoder.FormQuestionsList(selectedItem.TestTitle);
                TestStructs.TestInfo infoOfTestToTake = DataDecoder.GetTestInfo(selectedItem.TestTitle);
                NameEntry_Window nameEntry_Window = new NameEntry_Window(questionsToTake, infoOfTestToTake);
                nameEntry_Window.Show();
                askForClosingComfirmation = false;
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
                List<TestStructs.Question> questionsToEdit = DataDecoder.FormQuestionsList(selectedItem.TestTitle);
                TestStructs.TestInfo infoOfTestToEdit = DataDecoder.GetTestInfo(selectedItem.TestTitle);

                DataEraser.EraseTestFolder(selectedItem.TestTitle);

                // Updating list of transliterated test titles
                string pathOfTestsDirectory = ConfigurationManager.AppSettings["testTitlesDirPath"];
                string pathOfTestsFile = ConfigurationManager.AppSettings["testTitlesFilePath"];
                FileReader fileReader = new FileReader(pathOfTestsDirectory, $"{pathOfTestsFile}.txt");
                List<string> allTestsList = fileReader.UpdateListOfExistingTestsPaths();
                FileWriter fileWriter = new FileWriter(fileReader.DirectoryPath, fileReader.FilePath);
                fileWriter.WriteListInFileByLines(allTestsList);

                List<ImageManager.ImageInfo> emptyImageInfos = new List<ImageManager.ImageInfo>();
                TestSaving_Window testSaving_Window = new TestSaving_Window(questionsToEdit, emptyImageInfos, infoOfTestToEdit);
                testSaving_Window.Show();
                askForClosingComfirmation = false;
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
                string transliteratedTestTitle = DataDecoder.TransliterateAString(selectedItem.TestTitle);
                // Getting up-to-date list of data about passing selected test
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
                    askForClosingComfirmation = false;
                    Close();
                }
            }
        }
        /// <summary>
        /// Puts list of test titles into GUI
        /// </summary>
        /// <param name="existingTestsTitles">List of transliterated test titles (a set of paths to databases)</param>
        public void GetListAndPutItInGUI(List<string> existingTestsTitles)
        {
            testTitles = new List<string>();
            foreach (string testTitleTransliterated in existingTestsTitles)
            {
                string notTransliteratedTitle = DataDecoder.GetTestInfo(testTitleTransliterated)
                    .testTitle;
                testTitles.Add(notTransliteratedTitle);

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
            if (!askForClosingComfirmation) return;
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
            Sorter.SortTests(TestInfoSortOptions.SelectedIndex, existingTestsTitles);
        }
        /// <summary>
        /// Handling choosing anything from "QuestionSortOptions" ComboBox
        /// </summary>
        private void QuestionSortOptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Sorter.SortQuestions(QuestionSortOptions.SelectedIndex, existingTestsTitles);
        }
        /// <summary>
        /// Handling choosing anything from "TestInfoGroupOptions" ComboBox
        /// </summary>
        private void TestInfoGroupOptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Grouper.GroupTests(TestInfoGroupOptions.SelectedIndex, existingTestsTitles);
        }
        /// <summary>
        /// Handling choosing anything from "QuestionGroupOptions" ComboBox
        /// </summary>
        private void QuestionGroupOptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Grouper.GroupQuestions(QuestionGroupOptions.SelectedIndex, existingTestsTitles);
        }
        /// <summary>
        /// Handling clock in questions search field
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

                foreach(string testTitle in existingTestsTitles)
                {
                    List<TestStructs.Question> currentTestQuestions = DataDecoder.FormQuestionsList(testTitle);
                    TestStructs.Question foundQuestion = currentTestQuestions.Find(a => string.Compare(a.question.ToLower(), questionToSearch.ToLower()) == 0);
                    // If a structure with required title is found
                    if (foundQuestion.question != null)
                    {
                        // Forming output
                        string foundQuestionTestTitle = DataDecoder.GetTestInfo(testTitle).testTitle;
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

                foreach (string testTitle in existingTestsTitles)
                {
                    List<TestStructs.Question> currentTestQuestions = DataDecoder.FormQuestionsList(testTitle);
                    foreach(TestStructs.Question currentQuestion in currentTestQuestions)
                    {
                        foreach(string variant in currentQuestion.variants)
                        {
                            if (string.Compare(variant.ToLower(), variantToSearch.ToLower()) == 0){
                                // If searched variant was found, form the output
                                string foundVariantTestTitle = DataDecoder.GetTestInfo(testTitle).testTitle;
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
