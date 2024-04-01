using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using static courseWork_project.ImageManager;

namespace courseWork_project
{
    /// <summary>
    /// Клас для маніпулювання об'єктами ObservableCollection для GridView.
    /// </summary>
    /// <remarks>
    /// Містить Getter i Setter для string Question.
    /// </remarks>
    public class QuestionItem
    {
        public string Question { get; set; }
    }
    /// <summary>
    /// Логіка взаємодії з TestSaving_Window.xaml. Клас TestSaving_Window наслідує інтерфейс IListInGUIPuttable<List<Test.Question>>>
    /// </summary>
    /// <remarks>Вікно TestSaving_Window використовується для збереження створеного/відредагованого тесту в базу даних</remarks>
    public partial class TestSaving_Window : Window
    {
        /// <summary>
        /// Список (List) структур запитань
        /// </summary>
        private List<TestStructs.Question> questionsToSave;
        /// <summary>
        /// Масив структур для збереження даних про ілюстрації
        /// </summary>
        private List<ImageManager.ImageInfo> imagesList;
        /// <summary>
        /// Структура, що містить інформацію про поточний тест
        /// </summary>
        private TestStructs.TestInfo testInfo;
        /// <summary>
        /// Змінна для використання при оновленні назв картинок
        /// </summary>
        /// <remarks>Містить транслітеровану стару назву тесту</remarks>
        private string transliterOldTestTitle = string.Empty;
        /// <summary>
        /// ObservableCollection для DataGrid
        /// </summary>
        private ObservableCollection<QuestionItem> questionItems;
        /// <summary>
        /// Булева змінна для визначення режиму вікна
        /// </summary>
        /// <remarks>Якщо true - вікно в режимі створення, false - в режимі редагування</remarks>
        private bool creatingMode;
        /// <summary>
        /// Змінна, на основі якої буде з'являтись вікно підтвердження закриття вікна
        /// </summary>
        bool askForClosingComfirmation = true;
        /// <summary>
        /// Конструктор TestSaving_Window для режиму створення тесту
        /// </summary>
        /// <param name="questionsToSave">Список (List) структур запитань</param>
        /// <param name="imagesToSave">Список (List) структур інформації про картинки</param>
        public TestSaving_Window(List<TestStructs.Question> questionsToSave, List<ImageManager.ImageInfo> imagesToSave)
        {
            creatingMode = true;
            this.questionsToSave = questionsToSave;
            imagesList = imagesToSave;
            // По замовчуванню обмежень проходження тесту в часі немає
            testInfo.timerValue = 0;
            InitializeComponent();
            TimerInputBox.Text = testInfo.timerValue.ToString();
            // Ініціювання об'єкту класу ObservableCollection, що містить QuestionItemи
            questionItems = new ObservableCollection<QuestionItem>();
            // Присвоєння questionItems як джерела об'єктів для DataGrid з ім'ям dataGrid
            dataGrid.ItemsSource = questionItems;
            GetListAndPutItInGUI(questionsToSave);
        }
        /// <summary>
        /// Конструктор TestSaving_Window для режиму редагування тесту
        /// </summary>
        /// <param name="questionsToSave">Список (List) структур запитань</param>
        /// <param name="currTestInfo">Структура загальної інформації про тест</param>
        public TestSaving_Window(List<TestStructs.Question> questionsToSave, List<ImageManager.ImageInfo> imagesToSave, TestStructs.TestInfo currTestInfo)
        {
            creatingMode = false;
            this.questionsToSave = questionsToSave;
            testInfo = currTestInfo;
            transliterOldTestTitle = DataDecoder.TransliterateAString(testInfo.testTitle);
            imagesList = imagesToSave;
            InitializeComponent();
            TimerInputBox.Text = testInfo.timerValue.ToString();
            // Вивід даної назви у відповідний GUI елемент
            TestTitleBlock.Foreground = new SolidColorBrush(Colors.Black);
            TestTitleBlock.Text = testInfo.testTitle;
            questionItems = new ObservableCollection<QuestionItem>();
            // Присвоєння questionItems як джерела об'єктів для DataGrid з ім'ям dataGrid
            dataGrid.ItemsSource = questionItems;
            GetListAndPutItInGUI(questionsToSave);
        }
        /// <summary>
        /// Обробка події, коли клацнуто на поле вводу назви тесту
        /// </summary>
        private void TestTitleBlock_GotFocus(object sender, RoutedEventArgs e)
        {
            // У полі текст по замовчуванню
            bool titleContainsDefaultText = TestTitleBlock != null
                && string.Compare(TestTitleBlock.Text, "Введіть назву тесту") == 0;
            if (titleContainsDefaultText || testInfo.testTitle == null)
            {
                // Стираємо поле назви тесту та міняємо колір тексту
                TestTitleBlock.Foreground = new SolidColorBrush(Colors.Black);
                TestTitleBlock.Text = string.Empty;
            }
        }
        /// <summary>
        /// Обробка події, коли поле вводу назви тесту втратило фокус
        /// </summary>
        /// <remarks>Тобто коли після фокусу на полі клацнуто на будь-що інше</remarks>
        private void TestTitleBlock_LostFocus(object sender, RoutedEventArgs e)
        {
            // Якщо поле порожнє, то повертаємо значення по замовчуванню
            bool fieldIsEmpty = TestTitleBlock != null && string.IsNullOrWhiteSpace(TestTitleBlock.Text);
            if (fieldIsEmpty)
            {
                // Міняємо значення поля назви тесту та міняємо колір тексту
                TestTitleBlock.Text = "Введіть назву тесту";
                TestTitleBlock.Foreground = new SolidColorBrush(Colors.DarkGray);
            }
        }
        /// <summary>
        /// Обробка події, коли натиснуто GUI кнопку Save_Button
        /// </summary>
        /// <remarks>Викликає SaveDataAndGoToMain</remarks>
        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            SaveDataAndGoToMain();
        }
        /// <summary>
        /// Обробка події, коли натиснуто GUI кнопку типу EditButton
        /// </summary>
        /// <remarks>Знаходить обраний елемент в List<Test.Question> та відкриває його у TestChange_Window в режимі редагування</remarks>
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            // При неправильному вводі нічого не робимо
            if (!InputIsCorrect()) return;

            // Отримання обраного користувачем елементу як класу QuestionItem
            QuestionItem selectedItem = dataGrid.SelectedItem as QuestionItem;
            
            if (selectedItem != null)
            {
                int indexOfElementToEdit = 0;
                // Знаходження обраного користувачем елементу в List<TestStructs.Question>
                for (int i = 0; i < questionsToSave.Count; i++)
                {
                    if (string.Compare(questionsToSave[i].question, selectedItem.Question) == 0)
                    {
                        indexOfElementToEdit = i+1;
                        break;
                    }
                }
                // Збереження введеної назви у структуру інформації про тест
                testInfo.testTitle = TestTitleBlock.Text;
                // Оновлення відповідно до зміни назви тесту
                UpdateTitleIfChanged();
                // Ініціація TestChange_Window в режимі редагування
                TestChange_Window testChange_Window = new TestChange_Window(questionsToSave, imagesList, testInfo, indexOfElementToEdit);
                testChange_Window.Show();
                askForClosingComfirmation = false;
                Close();
            }
        }
        /// <summary>
        /// Обробка події, коли натиснуто GUI кнопку типу DeleteButton
        /// </summary>
        /// <remarks>Видаляє обране запитання з List<Test.Question> та DataGrid</remarks>
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // Отримання обраного користувачем елементу як класу QuestionItem
            QuestionItem selectedItem = dataGrid.SelectedItem as QuestionItem;
            if (selectedItem != null)
            {
                // Якщо передано порожній список
                if (imagesList.Count == 0)
                {
                    // Формування списку з директорії-бази даних
                    ImageListFormer imageListFormer = new ImageListFormer();
                    imagesList = imageListFormer.FormImageList(testInfo.testTitle, questionsToSave);
                }
                // Знаходження обраного користувачем елементу в List<TestStructs.Question>
                for (int i  = 0; i < questionsToSave.Count; i++)
                {
                    if (string.Compare(questionsToSave[i].question, selectedItem.Question) == 0)
                    {
                        // Видалення обраного запитання з List<TestStructs.Question>
                        questionsToSave.RemoveAt(i);
                        // Спроба пошуку картинки під індексом запитання (від 1 до 10)
                        ImageManager.ImageInfo foundImage = imagesList.Find(x => x.questionIndex == i+1);
                        // Якщо повернено не значення по замовчуванню, то запис знайдено
                        if (!foundImage.Equals(default(ImageInfo)))
                        {
                            // Видалення прив'язаної картинки з List<ImageManager.ImageInfo>
                            imagesList.Remove(foundImage);

                        }
                        break;
                    }
                }
                // Видалення обраного запитання з DataGrid
                questionItems.Remove(selectedItem);
            }
        }
        /// <summary>
        /// Обробка події, коли натиснуто клавішу на клавіатурі
        /// </summary>
        /// <remarks>F1 - посібник користувача;
        /// Esc - попереднє вікно;
        /// Enter - збереження тесту</remarks>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1)
            {
                HelpCenter_Window helpCenter = new HelpCenter_Window();
                helpCenter.Show();
            }
            if (e.Key == Key.Escape)
            {
                // При неправильному вводі нічого не робимо
                if (!InputIsCorrect()) return;
                // Оновлення відповідно до зміни назви тесту
                UpdateTitleIfChanged();
                TestChange_Window testChange_Window = new TestChange_Window(questionsToSave, imagesList, testInfo, questionsToSave.Count);
                testChange_Window.Show();
                askForClosingComfirmation = false;
                Close();
            }
            if (e.Key == Key.Enter)
            {
                SaveDataAndGoToMain();
            }
        }
        /// <summary>
        /// Створює базу даних, записує в неї дані тесту та повертається до MainWindow
        /// </summary>
        private void SaveDataAndGoToMain()
        {
            // При неправильному вводі нічого не робимо
            if (!InputIsCorrect()) return;
            // Збереження закодованих в рядки даних у файл з однойменною до назви тесту назвою
            List<string> listToWrite = DataEncoder.EncodeAndReturnLines(testInfo, questionsToSave);
            FileWriter fileWriter = new FileWriter(testInfo.testTitle);
            fileWriter.WriteListInFileByLines(listToWrite);
            // Отримання актуального списку транслітерованих назв тестів
            string pathOfTestsDirectory = ConfigurationManager.AppSettings["testTitlesDirPath"];
            string pathOfTestsFile = ConfigurationManager.AppSettings["testTitlesFilePath"];
            FileReader fileReader = new FileReader(pathOfTestsDirectory, $"{pathOfTestsFile}.txt");
            List<string> allTestsList = fileReader.RefreshTheListOfTests();
            // Додання назви щойно збереженого тесту до списку транслітерованих назв тестів
            string tranliteratedCurrTestTitle = DataDecoder.TransliterateAString(testInfo.testTitle);
            allTestsList.Add(tranliteratedCurrTestTitle);
            // Перезапис списку транслітерованих назв тестів у їхню базу даних
            fileWriter = new FileWriter(fileReader.DirectoryPath, fileReader.FilePath);
            fileWriter.WriteListInFileByLines(allTestsList);
            // Переміщення/перейменування всіх картинок до директорії-бази даних
            foreach(ImageInfo currentImageInfo in imagesList)
            {
                // Якщо при видаленні запитання було зменшено кількість запитань
                if (questionsToSave.Count == currentImageInfo.questionIndex) break;
                // Якщо прив'язку не видалено та режим створення чи картинку не переміщено в директорію-базу даних
                bool imageNeedsMovement = questionsToSave[currentImageInfo.questionIndex-1].hasLinkedImage
                    && (creatingMode || !ImagePathContainsTestTitle(currentImageInfo));
                if (imageNeedsMovement)
                // Переміщення кожної картинки до директорії-бази даних
                {
                    string imageName = $"{tranliteratedCurrTestTitle}-{currentImageInfo.questionIndex}";
                    ImageManager.CopyImageToFolder(currentImageInfo.imagePath, imageName);
                }
            }
            // Оновлення відповідно до зміни назви тесту
            UpdateTitleIfChanged();
            MessageBox.Show("Тест успішно збережено");
            // Повернення до MainWindow
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            askForClosingComfirmation = false;
            Close();
        }
        /// <summary>
        /// Змінює назву тесту в шляхах картинок в папці та списку структур, якщо назву тесту було змінено
        /// </summary>
        public void UpdateTitleIfChanged()
        {
            // Якщо назву було змінено
            bool titleChanged = string.Compare(transliterOldTestTitle,
                    DataDecoder.TransliterateAString(testInfo.testTitle)) != 0
                    && transliterOldTestTitle != string.Empty;
            if (titleChanged)
            {
                // Перейменування всіх картинок в images
                string transliteratedNewTestTitle = DataDecoder.TransliterateAString(testInfo.testTitle);
                ImageManager.RenameAll(transliterOldTestTitle, transliteratedNewTestTitle);
                // Перейменування даних картинок в списку
                foreach (ImageInfo currImageInfo in imagesList)
                {
                    if (currImageInfo.imagePath.Contains(transliterOldTestTitle))
                    {
                        currImageInfo.imagePath.Replace(transliterOldTestTitle, transliteratedNewTestTitle);
                    }
                }
            }
        }
        /// <summary>
        /// Перевірка, чи шлях містить назву тесту для визначення потреб переміщення картинки
        /// </summary>
        /// <returns>true, якщо містить; false, якщо ні</returns>
        private bool ImagePathContainsTestTitle(ImageInfo imageInfo)
        {
            if (imageInfo.imagePath.Contains(DataDecoder.TransliterateAString(testInfo.testTitle))){
                return true;
            }
            return false;
        }
        /// <summary>
        /// Перевіряє, чи ввід в полі назви і таймера правильний та записує ці дані в структуру TestStructs.TestInfo
        /// </summary>
        /// <returns>true, якщо правильний ввід; false, якщо ні</returns>
        private bool InputIsCorrect()
        {
            try
            {
                // Якщо поле назви порожнє або не зазначене (має значення по замовчуванню)
                bool titleBlockIsNotSet = string.IsNullOrWhiteSpace(TestTitleBlock.Text) || string.Compare(TestTitleBlock.Text, "Введіть назву тесту") == 0;
                // Якщо значення таймера не встановлене
                bool timerIsEmpty = string.IsNullOrWhiteSpace(TimerInputBox.Text);
                if (titleBlockIsNotSet || timerIsEmpty) throw new ArgumentNullException();
                // Якщо значення таймера встановлене, але воно некоректне (не типу int або від'ємне)
                bool timerIsSetWrong = !int.TryParse(TimerInputBox.Text, out testInfo.timerValue) || int.Parse(TimerInputBox.Text) < 0;
                if (timerIsSetWrong) throw new FormatException();

                testInfo.lastEditedTime = DateTime.Now;
                testInfo.testTitle = TestTitleBlock.Text;
                return true;
            }
            catch (ArgumentNullException)
            {
                MessageBox.Show("Будь ласка, заповніть всі потрібні поля");
                return false;
            }
            catch (FormatException)
            {
                MessageBox.Show("Некоректний ввід у поле таймера");
                return false;
            }
        }
        /// <summary>
        /// Передає список (List) структур запитань в GUI
        /// </summary>
        /// <remarks>Створює новий рядок в DataGrid для кожного елемента списку</remarks>
        /// <param name="questionsList">Список структур даних запитань тесту</param>
        public void GetListAndPutItInGUI(List<TestStructs.Question> questionsList)
        {
            foreach (TestStructs.Question questionFromList in questionsList)
            {
                AddNewDataGridRow(questionFromList.question);
            }
        }
        /// <summary>
        /// Створює новий рядок в DataGrid, заповнюючи поле Question заданим значенням
        /// </summary>
        /// <param name="questionText">Текст поточного запитання</param>
        private void AddNewDataGridRow(string questionText)
        {
            // Створення нового елемента (рядка у DataGrid)
            QuestionItem newItem = new QuestionItem
            {
                Question = questionText
            };
            // Додання нового елемента до ObservableCollection
            questionItems.Add(newItem);
        }
        /// <summary>
        /// Обробка події, коли вікно закривається
        /// </summary>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Якщо підтвердження закриття не потрібне, то нічого не робимо
            if (!askForClosingComfirmation) return;
            MessageBoxResult result = MessageBox.Show("Ви справді хочете закрити програму?", "Підтвердження закриття вікна", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No)
            {
                // Скасує процес закриття вікна
                e.Cancel = true;
            }
        }
    }
}
