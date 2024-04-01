using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static courseWork_project.ImageManager;

namespace courseWork_project
{
    /// <summary>
    /// Логіка взаємодії з TestTaking_Window.xaml. Клас TestTaking_Window наслідує інтерфейс IListInGUIPuttable<List<Test.Question>>
    /// </summary>
    /// <remarks> Вікно TestTaking_Window.xaml використовується для проходження тесту</remarks>
    public partial class TestTaking_Window : Window
    {
        /// <summary>
        /// Індекс поточного запитання тесту
        /// </summary>
        /// <remarks>Набуває значень від 1 до 10</remarks>
        private int currentQuestionIndex = 1;
        /// <summary>
        /// Загальна кількість запитань тесту
        /// </summary>
        /// <remarks>Набуває значень від 1 до 10</remarks>
        private int totalQuestionsCount = 1;
        /// <summary>
        /// Кількість правильних відповідей при проходженні тесту
        /// </summary>
        /// <remarks>Набуває значень від 0 до 10</remarks>
        private int correctAnswersCount = 0;
        /// <summary>
        /// Змінна, що містить ім'я користувача
        /// </summary>
        private string userName;
        /// <summary>
        /// Змінна для визначення, чи було обрано який-небудь варіант
        /// </summary>
        private bool buttonClicked = false;
        /// <summary>
        /// Список з TestStructs.Question для оперування даними запитань тесту
        /// </summary>
        private List<TestStructs.Question> questionsList;
        /// <summary>
        /// Структура з інформацією про тест
        /// </summary>
        private TestStructs.TestInfo testInfo;
        /// <summary>
        /// Змінна для визначення існування ілюстрацій
        /// </summary>
        private string transliteratedTestTitle;
        /// <summary>
        /// Словник для пар Button-bool для визначення правильності варіантів
        /// </summary>
        /// <remarks>Модифікується при відкритті проходження тесту, використовується при перевірці правильності варіантів</remarks>
        private Dictionary<Button, bool> variantsDict = new Dictionary<Button, bool>();
        /// <summary>
        /// Цілочисельна змінна для керування таймером
        /// </summary>
        private int timeLimitInSeconds = 0;
        /// <summary>
        /// Ініціалізація таймера
        /// </summary>
        private Timer timer = new Timer();
        /// <summary>
        /// Змінна, на основі якої буде з'являтись вікно підтвердження закриття вікна
        /// </summary>
        bool askForClosingComfirmation = true;

        /// <summary>
        /// Конструктор TestTaking_Window, приймає 3 аргументи
        /// </summary>
        /// <param name="questionsList">Список з TestStructs.Question для оперування даними запитань тесту</param>
        /// <param name="currTestInfo">Структура з інформацією про тест</param>
        /// <param name="userName">Ім'я користувача, що проходить тест</param>
        public TestTaking_Window(List<TestStructs.Question> questionsList, TestStructs.TestInfo currTestInfo, string userName)
        {
            this.questionsList = questionsList;
            testInfo = currTestInfo;
            transliteratedTestTitle = DataDecoder.TransliterateAString(testInfo.testTitle);
            totalQuestionsCount = questionsList.Count;
            this.userName = userName;

            InitializeComponent();

            // Перед початком проходження відображуємо загальну інформацію про тест
            string generalInfoForMessageBox = $"Тест \"{currTestInfo.testTitle}\"" +
                $"\nКількість запитань: {questionsList.Count}\n";
            timeLimitInSeconds = currTestInfo.timerValue * 60;
            bool noTimeLimits = timeLimitInSeconds == 0;
            generalInfoForMessageBox =  noTimeLimits ?
                string.Concat(generalInfoForMessageBox, "Час проходження необмежений")
                : string.Concat(generalInfoForMessageBox, $"Часу на проходження: {currTestInfo.timerValue} хв");
            generalInfoForMessageBox = string.Concat(generalInfoForMessageBox, 
                "\nДеякі запитання можуть мати декілька правильних варіантів відповідей");
            generalInfoForMessageBox = string.Concat(generalInfoForMessageBox, 
                "\nВи розпочнете спробу проходження тесту, коли закриєте це вікно");
            MessageBox.Show(generalInfoForMessageBox, "Інформація про тест");

            ShowQuestionAtIndex(0);
            // Якщо обмежень в часі немає, то текст таймера приховується і таймер не працює
            if (noTimeLimits)
            {
                Timer_TextBlock.Text = string.Empty;
                Timer_TextBlock.Visibility = Visibility.Collapsed;
                return;
            }
            // Початкове значення таймера (сам ліміт)
            Timer_TextBlock.Text = $"{timeLimitInSeconds / 60}:{timeLimitInSeconds % 60}";
            // Налаштування та старт таймера
            timer.Interval = 1000;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }
        /// <summary>
        /// Викликається таймером щосекунди
        /// </summary>
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            timeLimitInSeconds--;
            // Якщо час вичерпано, то таймер зупиняється, виводяться результати та завершується тест
            if(timeLimitInSeconds == 0)
            {
                timer.Stop();
                string resultsOfTest = FormResultsOfTest();
                MessageBox.Show($"Час вичерпано!\nРезультати тестування:\n{resultsOfTest}", "Результати проходження тесту");
                Application.Current.Dispatcher.Invoke(() =>
                {
                    EndTestAndSaveResults(resultsOfTest);
                });
            }
            Timer_TextBlock.Dispatcher.Invoke(() =>
            {
                Timer_TextBlock.Text = $"{timeLimitInSeconds / 60}:{timeLimitInSeconds % 60}";
            }
            );
        }
        /// <summary>
        /// Обробка події, коли натиснуто GUI кнопку BackToMain_Button
        /// </summary>
        /// <remarks>Викликає GoToMainWithConfimation для підтвердження переходу до головної сторінки</remarks>
        private void BackToMain_Button_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            GoToMainWithConfimation();
        }
        /// <summary>
        /// Обробка події, коли натиснуто GUI кнопку NextQuestion_Button
        /// </summary>
        /// <remarks>Викликає GoToNextQuestion для відображення наступного запитання</remarks>
        private void NextQuestion_Button_Click(object sender, RoutedEventArgs e)
        {
            GoToNextQuestion();
        }
        /// <summary>
        /// Обробка події, коли натиснуто GUI кнопку EndTest_Button
        /// </summary>
        /// <remarks>Викликає TryToFinishTheTest</remarks
        private void EndTest_Button_Click(object sender, RoutedEventArgs e)
        {
            TryToFinishTheTest();
        }
        /// <summary>
        /// Формує результати проходження тесту для відображення та запису
        /// </summary>
        /// <returns>Рядок з результатами</returns>
        private string FormResultsOfTest()
        {
            string resultsToReturn = $"{userName}: {correctAnswersCount}/{totalQuestionsCount}";
            return resultsToReturn;
        }
        /// <summary>
        /// Оновлює список з результатами проходження поточного тесту та відкриває MainWindow
        /// </summary>
        /// <param name="currentResult">Рядок з результатами проходження поточного тесту</param>
        private void EndTestAndSaveResults(string currentResult)
        {
            string transliteratedTestTitle = DataDecoder.TransliterateAString(testInfo.testTitle);
            // Отримання актуального списку даних про проходження
            string pathOfResultsDirectory = ConfigurationManager.AppSettings["testResultsDirPath"];
            FileReader fileReader = new FileReader(pathOfResultsDirectory, $"{transliteratedTestTitle}.txt");
            List<string> allResultsList = fileReader.ReadAndReturnLines();
            // Додавання нових даних до цього списку
            allResultsList.Add(currentResult);
            // Запис оновленого списку даних про проходження
            FileWriter fileWriter = new FileWriter(fileReader.DirectoryPath, $"{transliteratedTestTitle}.txt");
            fileWriter.WriteListInFileByLines(allResultsList);
            // Відкриття головного меню
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            askForClosingComfirmation = false;
            Close();
        }
        /// <summary>
        /// Оновлює видимість всіх змінних GUI елементів
        /// </summary>
        /// <remarks>Робить це на основі поточного запитання</remarks>
        private void UpdateGUI()
        {
            // Відображення індексу поточного запитання тесту та їх кількості
            CurrentQuestion_Text.Text = $"{currentQuestionIndex}/{totalQuestionsCount}";
            // Оновлення видимості кнопки переходу на наступне запитання тесту
            if (currentQuestionIndex == totalQuestionsCount)
            {
                NextQuestion_Button.Visibility = Visibility.Collapsed;
            }
            else NextQuestion_Button.Visibility = Visibility.Visible;
            // Оновлення видимості кнопки завершення тесту
            if (currentQuestionIndex == totalQuestionsCount)
            {
                EndTest_Button.Visibility = Visibility.Visible;
            }
        }
        /// <summary>
        /// Оновлює картинку для відображення у поточному запитанні
        /// </summary>
        private void UpdateImageAppearance()
        {
            try
            {
                // Отримуємо всі наявні картинки
                string[] allImagesPaths = Directory.GetFiles(ImagesDirectory);
                // Якщо в папці images немає картинок, сповістити про це
                if (allImagesPaths.Length == 0) throw new ArgumentNullException();
                foreach (string currentImagePath in allImagesPaths)
                {
                    // Знаходження бажаної картинки серед всіх наявних в папці images
                    if (questionsList[currentQuestionIndex-1].hasLinkedImage
                        && currentImagePath.Contains($"{transliteratedTestTitle}-{currentQuestionIndex}"))
                    {
                        // Відображення знайденої картинки
                        string absoluteImagePath = Path.GetFullPath(currentImagePath);
                        BitmapImage foundImageBitmap = new BitmapImage(new Uri(absoluteImagePath, UriKind.Absolute));
                        IllustrationImage.Source = foundImageBitmap;
                        foundImageBitmap.StreamSource = null;

                        QuestionText.HorizontalAlignment = HorizontalAlignment.Left;
                        ViewboxWithImage.Visibility = Visibility.Visible;
                        IllustrationImage.Visibility = Visibility.Visible;
                        return;
                    }
                }
            }
            catch(ArgumentNullException)
            {
                MessageBox.Show("На жаль, картинку не знайдено");
            }
        }
        /// <summary>
        /// Стирає дані, що містились у QuestionText; списку WrapPanel; словнику пар Button-bool
        /// </summary>
        private void ClearElementsData()
        {
            QuestionText.Text = string.Empty;
            wrapPanelOfVariants.Children.Clear();
            variantsDict.Clear();
        }
        /// <summary>
        /// Відображає запитання тесту зі списку під заданим індексом
        /// </summary>
        /// <param name="indexOfElementToReturnTo">Індекс запитання у списку (від 0 до 9)</param>
        private void ShowQuestionAtIndex(int indexOfElementToReturnTo)
        {
            currentQuestionIndex = ++indexOfElementToReturnTo;
            ClearElementsData();
            UpdateGUI();
            GetListAndPutItInGUI(questionsList);
        }
        /// <summary>
        /// Додає новий варіант відповіді на запитання тесту із заданим значенням, приймає 2 аргументи
        /// </summary>
        /// <param name="variantText">Значення (текст) цього варіанту відповіді</param>
        /// <param name="isCorrect">Цей варіант правильний?</param>
        private void AddNewVariant(string variantText, bool isCorrect)
        {
            // Якщо досягнуто ліміт варіантів, нічого не робимо
            if (wrapPanelOfVariants.Children.Count >= 8) return;

            // Створення кнопки з текстом варіанту відповіді
            Button button = new Button
            {
                Content = variantText,
                Foreground = new SolidColorBrush(Colors.Black),
                Background = (Brush)new BrushConverter().ConvertFrom("#fff0f0"),
                FontSize = 18,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                ToolTip = "Клацніть, щоб обрати цей варіант",
                Margin = new Thickness(3),
                MinWidth = 100,
                MaxWidth = 260
            };
            button.Click += VariantButton_Click;

            // Додавання Button та bool правильності поточного варіанту до словника
            variantsDict.Add(button, isCorrect);

            // Додавання створеної Button до WrapPanel
            wrapPanelOfVariants.Children.Add(button);
        }
        /// <summary>
        /// Перевірка правильності вибору користувача; колірне позначення клікнутої кнопки та правильних відповідей
        /// </summary>
        private void VariantButton_Click(object sender, RoutedEventArgs e)
        {
            // Якщо вже було клікнуто на кнопку варіанту, то нічого не робимо
            if (buttonClicked) return;

            buttonClicked = true;
            // Отримання даних натиснутої кнопки
            Button clickedButton = (Button)sender;
            string clickedButtonContent = clickedButton.Content.ToString();
            // Зміна фокусу на інші кнопки відповідно до індексу запитання (потрібно для переходу з допогомою клавіші Enter)
            if(currentQuestionIndex != questionsList.Count)
            {
                NextQuestion_Button.Focus();
            }
            else
            {
                EndTest_Button.Focus();
            }

            foreach(var buttonBoolPair in variantsDict)
            {
                string currButtonContent = buttonBoolPair.Key.Content.ToString();
                // Знайдено клікнуту кнопку (за її вмістом)
                if (string.Compare(currButtonContent, clickedButtonContent) == 0)
                {
                    /*
                     * Зміна кольору клікнутої кнопки на колір неправильного варіанту,
                     * якщо варіант правильний, то колір зміниться на колір правильного завдяки ShowCorrectAnswers
                    */
                    buttonBoolPair.Key.Background = new SolidColorBrush(Colors.DarkRed);
                    buttonBoolPair.Key.Foreground = new SolidColorBrush(Colors.White);

                    // Якщо обрано правильну відповідь, то інкрементується кількість правильних відповідей
                    if (buttonBoolPair.Value)
                    {
                        correctAnswersCount++;
                    }
                    break;
                }
            }
            ShowCorrectAnswers();
        }
        /// <summary>
        /// Змінює кольори кнопок варіантів залежно від правильності обраного варіанту
        /// </summary>
        private void ShowCorrectAnswers()
        {
            foreach (var currVariant in variantsDict)
            {
                if (currVariant.Value)
                {
                    currVariant.Key.Background = new SolidColorBrush(Colors.DarkGreen);
                    currVariant.Key.Foreground = new SolidColorBrush(Colors.White);
                }
            }
        }
        /// <summary>
        /// Обробка події, коли натиснуто клавішу на клавіатурі
        /// </summary>
        /// <remarks>F1 - посібник користувача;
        /// Esc - попереднє вікно;
        /// Enter - наступне запитання/завершення проходження тесту</remarks>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1)
            {
                HelpCenter_Window helpCenter = new HelpCenter_Window();
                helpCenter.Show();
            }
            if (e.Key == Key.Escape)
            {
                GoToMainWithConfimation();
            }
            if(e.Key == Key.Enter)
            {
                // Якщо запитання не останнє, викликає GoToNextQuestion для відображення наступного запитання
                if (currentQuestionIndex != totalQuestionsCount)
                {
                    GoToNextQuestion();
                    return;
                }
                // Інакше пробує закінчити тест
                TryToFinishTheTest();
            }
        }
        /// <summary>
        /// Викликає ConfirmWindow для підтвердженням користувачем переходу на головне вікно
        /// </summary>
        private void GoToMainWithConfimation()
        {
            string confirmationMessage = "Натиснувши \"Ні\", ви будете проходити тест заново. Натиснувши \"Так\", ви перейдете на головну сторінку.";
            List<ImageManager.ImageInfo> emptyImageInfos = new List<ImageManager.ImageInfo>();
            ConfirmWindow confirmWindow = new ConfirmWindow(ConfirmActionsWindowModes.TEST_TAKING_TO_MAIN,
                confirmationMessage, questionsList, emptyImageInfos, testInfo, currentQuestionIndex - 1);
            confirmWindow.Show();
            askForClosingComfirmation = false;
            Close();
        }
        /// <summary>
        /// Виводить результати проходження та викликає функцію їх збереження в базу даних
        /// </summary>
        private void TryToFinishTheTest()
        {
            try
            {
                if (!buttonClicked)
                {
                    throw new ArgumentNullException();
                }
                // Зупинка таймера
                timer.Stop();
                // Формування та вивід результатів
                string resultsOfTest = FormResultsOfTest();
                MessageBox.Show($"Тест \"{testInfo.testTitle}\" пройдено!\nРезультати тестування:\n{resultsOfTest}", "Результати проходження тесту");
                // Завершення тесту та збереження результатів у відповідну до тесту базу даних
                EndTestAndSaveResults(resultsOfTest);
            }
            catch (ArgumentNullException)
            {
                MessageBox.Show("Оберіть варіант відповіді, який вважаєте правильним");
            }
        }
        /// <summary>
        /// Переходить на наступне запитання за його наявності
        /// </summary>
        private void GoToNextQuestion()
        {
            try
            {
                if (!buttonClicked)
                {
                    throw new ArgumentNullException();
                }
                if (questionsList.Count > currentQuestionIndex)
                {
                    buttonClicked = false;
                    currentQuestionIndex++;
                    ShowQuestionAtIndex(currentQuestionIndex - 1);
                }
            }
            catch (ArgumentNullException)
            {
                MessageBox.Show("Оберіть варіант відповіді, який вважаєте правильним");
            }
        }
        /// <summary>
        /// Передає структуру даних поточного запитання в GUI
        /// </summary>
        /// <remarks>Розподіляє дані з структури по списках, формує та оновлює відповідний GUI</remarks>
        /// <param name="questions">Список структур запитань тесту</param>
        public void GetListAndPutItInGUI(List<TestStructs.Question> questions)
        {
            TestStructs.Question currentQuestion = questions[currentQuestionIndex - 1];
            // За наявності відображаємо ілюстрацію
            ViewboxWithImage.Visibility = Visibility.Collapsed;
            IllustrationImage.Visibility = Visibility.Collapsed;
            QuestionText.HorizontalAlignment = HorizontalAlignment.Center;

            if (currentQuestion.hasLinkedImage)
            {
                UpdateImageAppearance();
            }
            QuestionText.Text = currentQuestion.question;
            int tempIndexOfCorrectVariant = 0;
            foreach (string variant in currentQuestion.variants)
            {
                // В списку індексів правильних варіантів міститься/не міститься поточний індекс
                bool variantIsCorrect = currentQuestion.correctVariantsIndexes.Contains(tempIndexOfCorrectVariant);
                AddNewVariant(variant, variantIsCorrect);
                tempIndexOfCorrectVariant++;
            }
            UpdateGUI();
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
