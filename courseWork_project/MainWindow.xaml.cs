using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace courseWork_project
{
    /// <summary>
    /// Клас для маніпулювання об'єктами ObservableCollection для GridView.
    /// </summary>
    /// <remarks>
    /// Містить Getter i Setter для string TestTitle.
    /// </remarks>
    public class TestItem
    {
        public string TestTitle { get; set; }
    }
    /// <summary>
    /// Логіка взаємодії з MainWindow.xaml. Клас MainWindow наслідує інтерфейс IListInGUIPuttable<List<string>>
    /// </summary>
    /// <remarks>Вікно MainWindow.xaml використовується як меню</remarks>
    public partial class MainWindow : Window, IListInGUIPuttable<List<string>>
    {
        /// <summary>
        /// ObservableCollection для DataGrid
        /// </summary>
        private ObservableCollection<TestItem> testItems;
        /// <summary>
        /// Список (List) транслітерованих назв тестів
        /// </summary>
        private List<string> existingTestsTitles;
        /// <summary>
        /// Список (List) не транслітерованих назв тестів
        /// </summary>
        private List<string> testTitles;
        /// <summary>
        /// Назва директорії, в якій зберігається файл зі списком транслітерованих назв тестів
        /// </summary>
        private string directoryPathToTestsList = ConfigurationManager.AppSettings["testTitlesDirPath"];
        /// <summary>
        /// Назва файлу, в якому зберігається список транслітерованих назв тестів
        /// </summary>
        private string filePathToTestsList = ConfigurationManager.AppSettings["testTitlesFilePath"];
        /// <summary>
        /// Змінна, на основі якої буде з'являтись вікно підтвердження закриття вікна
        /// </summary>
        bool askForClosingComfirmation = true;
        /// <summary>
        /// Конструктор MainWindow, не приймає ніяких аргументів
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            // Ініціювання об'єкту класу FileReader для отримання списку транслітерованих назв тестів
            FileReader fileReader = new FileReader(directoryPathToTestsList, $"{filePathToTestsList}.txt");
            existingTestsTitles = fileReader.RefreshTheListOfTests();
            // Ініціювання об'єкту класу ObservableCollection, що містить TestItemи
            testItems = new ObservableCollection<TestItem>();
            // Присвоєння testItems як джерела об'єктів для DataGrid з ім'ям testsGrid
            testsGrid.ItemsSource = testItems;
            GetListAndPutItInGUI(existingTestsTitles);
        }
        // Деструктор
        ~MainWindow() 
        {
            Debug.WriteLine("Знищено об'єкт MainWindow");
        }
        /// <summary>
        /// Обробка події, коли натиснуто клавішу на клавіатурі
        /// </summary>
        /// <remarks>Якщо натиснуто F1, відкриває посібник користувача</remarks>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.F1)
            {
                HelpCenter_Window helpCenter = new HelpCenter_Window();
                helpCenter.Show();
            }
        }
        /// <summary>
        /// Обробка події, коли натиснуто GUI кнопку HelpCenter_button
        /// </summary>
        /// <remarks>Відкриває вікно HelpCenter_Window</remarks>
        private void HelpCenter_button_Click(object sender, RoutedEventArgs e)
        {
            HelpCenter_Window helpCenter_Window = new HelpCenter_Window();
            helpCenter_Window.Show();
        }
        /// <summary>
        /// Обробка події, коли натиснуто GUI кнопку Create_button
        /// </summary>
        /// <remarks>Відкриває вікно TestChange_Window в режимі створення тесту</remarks>
        private void Create_button_Click(object sender, RoutedEventArgs e)
        {
            TestChange_Window testChange_Window = new TestChange_Window();
            testChange_Window.Show();
            askForClosingComfirmation = false;
            Close();
        }
        /// <summary>
        /// Обробка події, коли натиснуто GUI кнопку типу Taking_Button
        /// </summary>
        /// <remarks>Отримує дані обраного тесту з бази даних та відкриває вікно його проходження</remarks>
        private void Taking_Button_Click(object sender, RoutedEventArgs e)
        {
            // Отримання обраного користувачем елементу як класу TestItem
            TestItem selectedItem = testsGrid.SelectedItem as TestItem;

            if (selectedItem != null)
            {
                // Отримання всіх потрібних даних обраного тесту для ініціації TestTaking_Window
                List<Test.Question> questionsToTake = DataDecoder.FormQuestionsList(selectedItem.TestTitle);
                Test.TestInfo infoOfTestToTake = DataDecoder.GetTestInfo(selectedItem.TestTitle);
                // Ініціація об'єкту TestTaking_Window, відкриття цього вікна
                NameEntry_Window nameEntry_Window = new NameEntry_Window(questionsToTake, infoOfTestToTake);
                nameEntry_Window.Show();
                askForClosingComfirmation = false;
                Close();
            }
        }
        /// <summary>
        /// Обробка події, коли натиснуто GUI кнопку типу EditButton
        /// </summary>
        /// <remarks>Отримує дані обраного тесту з бази даних та відкриває вікно його редагування</remarks>
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            // Отримання обраного користувачем елементу як класу TestItem
            TestItem selectedItem = testsGrid.SelectedItem as TestItem;

            if (selectedItem != null)
            {
                // Отримання всіх потрібних даних обраного тесту для ініціації TestSaving_Window в режимі редагування
                List<Test.Question> questionsToEdit = DataDecoder.FormQuestionsList(selectedItem.TestTitle);
                Test.TestInfo infoOfTestToEdit = DataDecoder.GetTestInfo(selectedItem.TestTitle);
                // Видалення директорії з базою даних обраного тесту
                DataDecoder.EraseFolder(selectedItem.TestTitle);

                // Отримання актуального списку транслітерованих назв тестів
                string pathOfTestsDirectory = ConfigurationManager.AppSettings["testTitlesDirPath"];
                string pathOfTestsFile = ConfigurationManager.AppSettings["testTitlesFilePath"];
                FileReader fileReader = new FileReader(pathOfTestsDirectory, $"{pathOfTestsFile}.txt");
                List<string> allTestsList = fileReader.RefreshTheListOfTests();
                // Запис оновленого списку транслітерованих назв існуючих тестів у їхню базу даних
                FileWriter fileWriter = new FileWriter(fileReader.DirectoryPath, fileReader.FilePath);
                fileWriter.WriteListInFileByLines(allTestsList);

                // Ініціація об'єкту TestSaving_Window, відкриття цього вікна в режимі редагування
                List<ImageManager.ImageInfo> emptyImageInfos = new List<ImageManager.ImageInfo>();
                TestSaving_Window testSaving_Window = new TestSaving_Window(questionsToEdit, emptyImageInfos, infoOfTestToEdit);
                testSaving_Window.Show();
                askForClosingComfirmation = false;
                Close();
            }
        }
        /// <summary>
        /// Обробка події, коли натиснуто GUI кнопку типу ResultsButton
        /// </summary>
        /// <remarks>Отримує результати проходжень обраного тесту та виводить їх у MessageBox</remarks>
        private void ResultsButton_Click(object sender, RoutedEventArgs e)
        {
            // Отримання обраного користувачем елементу як класу TestItem
            TestItem selectedItem = testsGrid.SelectedItem as TestItem;
            if (selectedItem != null)
            {
                string transliteratedTestTitle = DataDecoder.TransliterateAString(selectedItem.TestTitle);
                // Отримання актуального списку даних про проходження
                string pathOfResultsDirectory = ConfigurationManager.AppSettings["testResultsDirPath"];
                FileReader fileReader = new FileReader(pathOfResultsDirectory, $"{transliteratedTestTitle}.txt");
                List<string> currTestResultsList = fileReader.ReadAndReturnLines();
                // Якщо список результатів порожній, то значить тест ніхто ще не пройшов
                if(currTestResultsList.Count == 0)
                {
                    MessageBox.Show("Обраний тест ще ніким не було пройдено", "Історія проходжень тесту порожня");
                    return;
                }
                // Формування тексту для виводу
                string resultsToShow = $"Результати проходжень тесту:\n";
                foreach(string result in currTestResultsList)
                {
                    resultsToShow = string.Concat(resultsToShow, $"{result}\n");
                }
                MessageBox.Show(resultsToShow, "Історія проходжень тесту");
            }
        }
        /// <summary>
        /// Обробка події, коли натиснуто GUI кнопку типу DeleteButton
        /// </summary>
        /// <remarks>Відкриває вікно ConfirmWindow для підтвердження видалення обраного тесту</remarks>
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // Отримання обраного користувачем елементу як класу TestItem
            TestItem selectedItem = testsGrid.SelectedItem as TestItem;
            if (selectedItem != null)
            {
                string textOfConfirmation = $"Ви видалите тест \"{selectedItem.TestTitle}\".";
                // Ініціація об'єкту ConfirmWindow, відкриття цього вікна в режимі підтвердження видалення тесту
                ConfirmWindow confirmWindow = new ConfirmWindow(ConfirmActionsWindowModes.MAIN_DELETE_TEST, textOfConfirmation, selectedItem.TestTitle);
                confirmWindow.Show();
                askForClosingComfirmation = false;
                Close();
            }
        }
        /// <summary>
        /// Використовує список назв тестів для отримання їхніх даних. Передає ці дані в GUI
        /// </summary>
        /// <remarks>Кожна назва тесту - новий елемент GUI</remarks>
        /// <param name="existingTestsTitles">Список транслітерованих назв тестів, слугує набором шляхів баз даних.</param>
        public void GetListAndPutItInGUI(List<string> existingTestsTitles)
        {
            testTitles = new List<string>();
            // Отримання нетранслітерованої назви тесту для її відображення у DataGrid
            foreach (string testTitleTransliterated in existingTestsTitles)
            {
                string notTransliteratedTitle = DataDecoder.GetTestInfo(testTitleTransliterated)
                    .testTitle;
                testTitles.Add(notTransliteratedTitle);
                // Створення нового рядка у DataGrid, що містить нетраслітеровану назву тесту
                AddNewDataGridRow(notTransliteratedTitle);
            }
        }
        /// <summary>
        /// Додає новий рядок до DataGrid з заданою назвою тесту
        /// </summary>
        /// <remarks>Кожен рядок містить назву тесту та кнопки для його проходження, редагування та видалення</remarks>
        /// <param name="testTitle">Нетранслітерована назва тесту</param>
        private void AddNewDataGridRow(string testTitle)
        {
            // Створення нового об'єкту класу TestItem з заданим TestTitle
            TestItem newItem = new TestItem
            {
                TestTitle = testTitle
            };
            // Додавання цього об'єкту до ObservableCollection для відображення у DataGrid
            testItems.Add(newItem);
        }
        /// <summary>
        /// Обробка події, коли вікно закривається
        /// </summary>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Якщо підтвердження закриття не потрібне, то нічого не робимо
            if (!askForClosingComfirmation) return;
            MessageBoxResult result = MessageBox.Show("Ви справді хочете закрити програму?", "Підтвердження закриття вікна", 
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No)
            {
                // Скасує процес закриття вікна
                e.Cancel = true;
            }
        }
        /// <summary>
        /// Обробка події, коли обрано якесь значення з ComboBox під назвою TestInfoSortOptions
        /// </summary>
        /// <remarks>Викликає функцію сортування даних тестів за обраним ключем</remarks>
        private void TestInfoSortOptions_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Sorter.SortTests(TestInfoSortOptions.SelectedIndex, existingTestsTitles);
        }
        /// <summary>
        /// Обробка події, коли обрано якесь значення з ComboBox під назвою QuestionSortOptions
        /// </summary>
        /// <remarks>Викликає функцію сортування запитань тестів за обраним ключем</remarks>
        private void QuestionSortOptions_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Sorter.SortQuestions(QuestionSortOptions.SelectedIndex, existingTestsTitles);
        }
        /// <summary>
        /// Обробка події, коли обрано якесь значення з ComboBox під назвою TestInfoGroupOptions
        /// </summary>
        /// <remarks>Викликає функцію групування даних тестів за обраним ключем</remarks>
        private void TestInfoGroupOptions_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Grouper.GroupTests(TestInfoGroupOptions.SelectedIndex, existingTestsTitles);
        }
        /// <summary>
        /// Обробка події, коли обрано якесь значення з ComboBox під назвою QuestionGroupOptions
        /// </summary>
        /// <remarks>Викликає функцію групування запитань тестів за обраним ключем</remarks>
        private void QuestionGroupOptions_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Grouper.GroupQuestions(QuestionGroupOptions.SelectedIndex, existingTestsTitles);
        }
        /// <summary>
        /// Обробка події, коли користувач клацнув на поле пошуку запитань
        /// </summary>
        private void QuestionSearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            QuestionSearchBox.Text = string.Empty;
            QuestionSearchBox.Foreground = new SolidColorBrush(Colors.Black);
        }
        /// <summary>
        /// Обробка події, коли користувач клацнув на поле пошуку відповідей на запитання тесту
        /// </summary>
        private void VariantSearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            VariantSearchBox.Text = string.Empty;
            VariantSearchBox.Foreground = new SolidColorBrush(Colors.Black);
        }
        /// <summary>
        /// Обробка події, коли користувач клацнув на GUI кнопку SearchQuestion_button
        /// </summary>
        private void SearchQuestion_button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string questionToSearch = QuestionSearchBox.Text;
                // Якщо поле порожнє або містить значення по замовчуванню
                if(questionToSearch == string.Empty || string.Compare(questionToSearch, "Пошук запитання тесту") == 0)
                {
                    throw new ArgumentNullException();
                }
                // В кожному існуючому тесті шукаємо структуру із вказаним запитанням
                foreach(string testTitle in existingTestsTitles)
                {
                    List<Test.Question> currentTestQuestions = DataDecoder.FormQuestionsList(testTitle);
                    Test.Question foundQuestion = currentTestQuestions.Find(a => string.Compare(a.question.ToLower(), questionToSearch.ToLower()) == 0);
                    // Якщо структуру із вказаним запитанням знайдено
                    if (foundQuestion.question != null)
                    {
                        // Формування виводу інформації про шукане запитання
                        string foundQuestionTestTitle = DataDecoder.GetTestInfo(testTitle).testTitle;
                        MessageBox.Show($"Введене запитання знайдено в тесті \"{foundQuestionTestTitle}\":\n"
                        + $"Запитання: {foundQuestion.question}; "
                        + $"Всього варіантів: {foundQuestion.variants.Count}; "
                        + $"Правильних варіантів: {foundQuestion.correctVariantsIndexes.Count}\n",
                        "Результат пошуку запитання тесту");
                        return;
                    }
                }
                // Вивід інформації в разі невдачі при пошуку запитання
                MessageBox.Show($"Запитання \"{questionToSearch}\" не знайдено. Перевірте правильність написання та спробуйте ще раз",
                    "Результат пошуку запитання тесту");
            }
            catch(ArgumentNullException)
            {
                MessageBox.Show("Будь ласка, заповніть поле пошуку запитання тесту");
            }
        }
        /// <summary>
        /// Обробка події, коли користувач клацнув на GUI кнопку SearchVariant_button
        /// </summary>
        private void SearchVariant_button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string variantToSearch = VariantSearchBox.Text;
                // Якщо поле порожнє або містить значення по замовчуванню
                if (variantToSearch == string.Empty || string.Compare(variantToSearch, "Пошук запитання тесту") == 0)
                {
                    throw new ArgumentNullException();
                }
                // В кожному існуючому тесті шукаємо запитання із вказаною відповіддю
                foreach (string testTitle in existingTestsTitles)
                {
                    List<Test.Question> currentTestQuestions = DataDecoder.FormQuestionsList(testTitle);
                    foreach(Test.Question currentQuestion in currentTestQuestions)
                    {
                        foreach(string variant in currentQuestion.variants)
                        {
                            // Якщо шуканий варіант знайдено
                            if(string.Compare(variant.ToLower(), variantToSearch.ToLower()) == 0){
                                // Формування виводу інформації про шукане запитання
                                string foundVariantTestTitle = DataDecoder.GetTestInfo(testTitle).testTitle;
                                MessageBox.Show($"Введений варіант знайдено в тесті \"{foundVariantTestTitle}\",\n"
                                + $"в запитанні \"{currentQuestion.question}\"",
                                "Результат пошуку запитання тесту");
                                return;
                            }
                        }
                    }
                }
                // Вивід інформації в разі невдачі при пошуку варіанту
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
