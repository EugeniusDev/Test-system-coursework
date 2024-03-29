using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Configuration;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using static courseWork_project.ImageManager;
using System.IO;
using System.Linq;

namespace courseWork_project
{
    /// <summary>
    /// Логіка взаємодії з TestChange_Window.xaml. Клас TestChange_Window наслідує інтерфейс IListInGUIPuttable<List<Test.Question>>
    /// </summary>
    /// <remarks> Вікно TestChange_Window.xaml використовується для створення/редагування запитань тесту</remarks>
    public partial class TestChange_Window : Window, IListInGUIPuttable<List<Test.Question>>
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
        /// Булева змінна для визначення режиму вікна
        /// </summary>
        /// <remarks>Якщо true - вікно в режимі створення, false - в режимі редагування</remarks>
        private bool creatingMode;
        /// <summary>
        /// Список з Test.Question для оперування даними запитань тесту
        /// </summary>
        private List<Test.Question> questionsList;
        /// <summary>
        /// Масив структур для збереження та маніпуляції даними про додану/змінену ілюстрацію
        /// </summary>
        private List<ImageManager.ImageInfo> imagesList;
        /// <summary>
        /// Структура з інформацією про тест
        /// </summary>
        private Test.TestInfo testInfo;
        /// <summary>
        /// Словник для пар TextBox-CheckBox для оперування даними варіантів відповідей
        /// </summary>
        /// <remarks>Модифікується при додаванні та видаленні варіантів відповідей</remarks>
        private Dictionary<TextBox, CheckBox> variantsDict = new Dictionary<TextBox, CheckBox>();
        /// <summary>
        /// Змінна, на основі якої буде з'являтись вікно підтвердження закриття вікна
        /// </summary>
        bool askForClosingComfirmation = true;

        /// <summary>
        /// Конструктор для режиму створення тесту
        /// </summary>
        /// <remarks>0 параметрів</remarks>
        public TestChange_Window()
        {
            InitializeComponent();
            questionsList = new List<Test.Question>();
            imagesList = new List<ImageManager.ImageInfo>();
            creatingMode = true;
            UpdateGUI();
        }
        /// <summary>
        /// Конструктор для режиму редагування тесту
        /// </summary>
        /// <remarks>Приймає 4 параметри</remarks>
        /// <param name="oldQuestionsList">Список Test.Question тесту, який редагується</param>
        /// <param name="imageSources">Список (List) структур інформації про картинки (може бути порожнім: 
        /// у випадку отримання картинок з директорії-бази даних)</param>
        /// <param name="currTestInfo">Інформація про тест, який редагується</param>
        /// <param name="indexOfElementToEdit">Індекс запитання, яке редагується (1-10)</param>
        public TestChange_Window(List<Test.Question> oldQuestionsList, List<ImageManager.ImageInfo> imageSources, Test.TestInfo currTestInfo, int indexOfElementToEdit)
        {
            creatingMode = false;
            questionsList = oldQuestionsList;
            testInfo = currTestInfo;
            // Якщо передано порожній список
            if (imageSources.Count == 0)
            {
                // Формування списку з директорії-бази даних
                ImageListFormer imageListFormer = new ImageListFormer();
                imagesList = imageListFormer.FormImageList(testInfo.testTitle, questionsList);
            }
            else
            {
                imagesList = imageSources;
            }

            InitializeComponent();

            ShowQuestionAtIndex(indexOfElementToEdit - 1);
            UpdateImageAppearance();
        }
        // Деструктор
        ~TestChange_Window()
        {
            Debug.WriteLine("Знищено об'єкт TestChange_Window");
        }
        /// <summary>
        /// Обробка події, коли натиснуто GUI кнопку BackToMain_Button
        /// </summary>
        /// <remarks>Відкриває вікно ConfirmWindow для підтвердження переходу до головної сторінки</remarks>
        private void BackToMain_Button_Click(object sender, RoutedEventArgs e)
        {
            if (!UpdateCurrentQuestionInfo()) return;
            GoToMainWithConfimation();
        }
        /// <summary>
        /// Обробка події, коли натиснуто GUI кнопку NextQuestion_Button
        /// </summary>
        /// <remarks>Викликає GoToNextQuestion</remarks>
        private void NextQuestion_Button_Click(object sender, RoutedEventArgs e)
        {
            GoToNextQuestion();
        }
        /// <summary>
        /// Обробка події, коли натиснуто GUI кнопку PrevQuestion_Button
        /// </summary>
        /// <remarks>Зберігає поточне запитання тесту, відображає попереднє</remarks>
        private void PrevQuestion_Button_Click(object sender, RoutedEventArgs e)
        {
            if (!UpdateCurrentQuestionInfo()) return;
            currentQuestionIndex--;
            ShowQuestionAtIndex(currentQuestionIndex-1);
        }
        /// <summary>
        /// Обробка події, коли натиснуто GUI кнопку SaveTest_Button
        /// </summary>
        /// <remarks>Викликає GoToSavingWindow</remarks>
        private void SaveTest_Button_Click(object sender, RoutedEventArgs e)
        {
            GoToSavingWindow();
        }
        /// <summary>
        /// Обробка події, коли клацнуто на поле варіанту відповіді
        /// </summary>
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            // Якщо поле містить текст по замовчуванню, то стираємо текст поля
            bool fieldContainsDefaultText = textBox != null
                && string.Compare(textBox.Text, "Введіть варіант відповіді") == 0;
            if (fieldContainsDefaultText)
            {
                textBox.Text = string.Empty;
            }
        }
        /// <summary>
        /// Обробка події, коли поле варіанту відповіді втратило фокус
        /// </summary>
        /// <remarks>Тобто коли після фокусу на полі клацнуто на будь-що інше</remarks>
        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            // Якщо поле порожнє, то повертаємо значення по замовчуванню
            bool fieldIsEmpty = textBox != null && string.IsNullOrWhiteSpace(textBox.Text);
            if (fieldIsEmpty)
            {
                textBox.Text = "Введіть варіант відповіді";
            }
        }
        /// <summary>
        /// Обробка події, коли будь-який CheckBox змінив своє значення
        /// </summary>
        /// <remarks>Тобто коли користувач клацнув на один з них</remarks>
        private void CheckBox_Updated(object sender, RoutedEventArgs e)
        {
            UpdateVariantsCheckedConditions();
        }
        /// <summary>
        /// Обробка події, коли натиснуто GUI кнопку AddVariant_Button
        /// </summary>
        /// <remarks>Додає новий варіант відповіді та оновлює відображення GUI</remarks>
        private void AddVariant_Button_Click(object sender, RoutedEventArgs e)
        {
            AddNewVariant();
            UpdateGUI();
        }
        /// <summary>
        /// Обробка події, коли натиснуто GUI кнопку RemoveLastVariant_Button
        /// </summary>
        /// <remarks>Видаляє останній в списку варіант відповіді та оновлює відображення GUI</remarks>
        private void RemoveLastVariant_Button_Click(object sender, RoutedEventArgs e)
        {
            RemoveLastVariant();
            UpdateGUI();
        }
        /// <summary>
        /// Оновлює видимість всіх змінних GUI елементів
        /// </summary>
        /// <remarks>Робить це на основі поточного запитання та кількості варіантів відповіді</remarks>
        private void UpdateGUI()
        {
            // Відображення індексу поточного запитання тесту та їх кількості
            CurrentQuestion_Text.Text = $"{currentQuestionIndex}/{totalQuestionsCount}";
            // Оновлення видимості кнопок переходу на попереднє та наступне запитання тесту
            if (currentQuestionIndex == 1)
            {
                PrevQuestion_Button.Visibility = Visibility.Collapsed;
            }
            else PrevQuestion_Button.Visibility = Visibility.Visible;
            if (currentQuestionIndex == 10)
            {
                NextQuestion_Button.Visibility = Visibility.Collapsed;
            }
            else NextQuestion_Button.Visibility = Visibility.Visible;

            // Оновлення видимості кнопок додавання нового та видалення останнього варіантів відповіді
            switch (dynamicWrapPanel.Children.Count)
            {
                // Якщо елементів 0, то кнопка видалення останнього варіанту невидима
                case 0:
                    RemoveLastVariant_Button.Visibility = Visibility.Collapsed;
                    AddVariant_Button.Visibility = Visibility.Visible;
                    break;
                // Якщо 1 елемент, кнопка видалення останнього варіанту видима
                case 1:
                    RemoveLastVariant_Button.Visibility = Visibility.Visible;
                    break;
                // Якщо елементів 8, то кнопка додавання нового елемента невидима
                case 8:
                    AddVariant_Button.Visibility = Visibility.Collapsed;
                    break;
                // У інших випадказ кнопки додавання/видалення варіантів видимі
                default:
                    RemoveLastVariant_Button.Visibility = Visibility.Visible;
                    AddVariant_Button.Visibility = Visibility.Visible;
                    break;
            }
            UpdateImageAppearance();
        }
        /// <summary>
        /// Стирає дані, що містились у QuestionInput; списку WrapPanel; словнику пар TextBox-CheckBox
        /// </summary>
        private void EraseElementsData()
        {
            QuestionInput.Text = string.Empty;
            dynamicWrapPanel.Children.Clear();
            variantsDict.Clear();
        }
        /// <summary>
        /// Перевірка, чи всі поля заповнені та чи існують варіанти відповідей
        /// </summary>
        /// <returns>true, якщо всі поля заповнені; false, якщо ні</returns>
        private bool AllTextboxesFilled()
        {
            string questionString = QuestionInput.Text;
            bool questionInputIsNull = string.IsNullOrWhiteSpace(questionString);
            if (questionInputIsNull) return false;
            foreach (var textBoxRelatedPair in variantsDict)
            {
                string tempStringOfVariant = textBoxRelatedPair.Key.Text;
                bool variantIsNotFilled = string.IsNullOrWhiteSpace(tempStringOfVariant) 
                    || string.Compare(tempStringOfVariant, "Введіть варіант відповіді") == 0;
                if (variantIsNotFilled || dynamicWrapPanel.Children.Count == 0)
                    return false;
            }
            return true;
        }
        /// <summary>
        /// Перевірка, чи всі поля заповнені належним чином
        /// </summary>
        /// <remarks>Поля повинні містити не тільки цифри</remarks>
        /// <returns>true, якщо всі поля заповнені правильно; false, якщо ні</returns>
        private bool AllTextboxesContainProperInfo()
        {
            string pattern = @"[^0-9]";
            bool resultOfCheck = Regex.IsMatch(QuestionInput.Text, pattern);
            foreach (var textBoxRelatedPair in variantsDict)
            {
                string tempStringOfVariant = textBoxRelatedPair.Key.Text;
                if (!Regex.IsMatch(tempStringOfVariant, pattern)) return false;
            }
            return resultOfCheck ? true : false;
        }
        /// <summary>
        /// Додає новий варіант відповіді на запитання тесту зі значенням по замовчуванню, приймає 0 аргументів
        /// </summary>
        /// <remarks>Варіант міститиме значення по замовчуванню</remarks>
        private void AddNewVariant()
        {
            // Якщо досягнуто ліміт варіантів, нічого не робимо
            bool variantsLimitReached = dynamicWrapPanel.Children.Count >= 8;
            if (variantsLimitReached) return;
            // Створення DockPanel для групування TextBox і CheckBox
            DockPanel dockPanel = new DockPanel();
            // Створення TextBox з підказкою
            TextBox textBox = new TextBox();
            textBox.Text = "Введіть варіант відповіді";
            textBox.Foreground = new SolidColorBrush(Colors.Black);
            textBox.Background = (Brush)new BrushConverter().ConvertFrom("#fff0f0");
            textBox.FontSize = 18;
            textBox.HorizontalAlignment = HorizontalAlignment.Right;
            textBox.VerticalAlignment = VerticalAlignment.Top;
            textBox.TextAlignment = TextAlignment.Center;
            textBox.TextWrapping = TextWrapping.NoWrap;
            textBox.ToolTip = "Варіант відповіді";
            textBox.Margin = new Thickness(3);
            textBox.MinWidth = 100;
            textBox.MaxWidth = 210;

            textBox.GotFocus += TextBox_GotFocus;
            textBox.LostFocus += TextBox_LostFocus;
            DockPanel.SetDock(textBox, Dock.Left);
            dockPanel.Children.Add(textBox);

            // Створення CheckBox для позначення варіанту як правильного
            CheckBox checkBox = new CheckBox();
            checkBox.HorizontalAlignment = HorizontalAlignment.Center;
            checkBox.Unchecked += CheckBox_Updated;
            checkBox.Checked += CheckBox_Updated;
            checkBox.FontStyle = FontStyles.Oblique;
            checkBox.Content = "Правильний";
            checkBox.ToolTip = "Позначити варіант як правильний";
            checkBox.Margin = new Thickness(3);
            DockPanel.SetDock(checkBox, Dock.Right);
            dockPanel.Children.Add(checkBox);

            // Додавання нових TextBox і CheckBox до словника їхніх пар
            variantsDict.Add(textBox, checkBox);

            // Додавання створеної StackPanel до WrapPanel
            dynamicWrapPanel.Children.Add(dockPanel);
        }
        /// <summary>
        /// Додає новий варіант відповіді на запитання тесту із заданим значенням, приймає 2 аргументи
        /// </summary>
        /// <param name="variantText">Значення (текст) цього варіанту відповіді</param>
        /// <param name="isCorrect">Цей варіант правильний?</param>
        private void AddNewVariant(string variantText, bool isCorrect)
        {
            // Якщо досягнуто ліміт варіантів, нічого не робимо
            bool variantsLimitReached = dynamicWrapPanel.Children.Count >= 8;
            if (variantsLimitReached) return;
            // Створення DockPanel для групування TextBox і CheckBox
            DockPanel dockPanel = new DockPanel();
            // Створення TextBox з текстом-підказкою (текстом по замовчуванню)
            TextBox textBox = new TextBox(); 
            textBox.Text = variantText;
            textBox.Foreground = new SolidColorBrush(Colors.Black);
            textBox.Background = (Brush)new BrushConverter().ConvertFrom("#fff0f0");
            textBox.FontSize = 18;
            textBox.HorizontalAlignment = HorizontalAlignment.Right;
            textBox.VerticalAlignment = VerticalAlignment.Top;
            textBox.TextAlignment = TextAlignment.Center;
            textBox.TextWrapping = TextWrapping.NoWrap;
            textBox.ToolTip = "Варіант відповіді";
            textBox.Margin = new Thickness(3);
            textBox.MinWidth = 100;
            textBox.MaxWidth = 210;

            textBox.GotFocus += TextBox_GotFocus;
            textBox.LostFocus += TextBox_LostFocus;
            DockPanel.SetDock(textBox, Dock.Left);
            dockPanel.Children.Add(textBox);

            // Створення CheckBox для позначення варіанту як правильного
            CheckBox checkBox = new CheckBox();
            checkBox.IsChecked = isCorrect;
            checkBox.HorizontalAlignment = HorizontalAlignment.Center;
            checkBox.Unchecked += CheckBox_Updated;
            checkBox.Checked += CheckBox_Updated;
            checkBox.FontStyle = FontStyles.Oblique;
            checkBox.Content = "Правильний";
            checkBox.ToolTip = "Позначити варіант як правильний";
            checkBox.Margin = new Thickness(3);
            DockPanel.SetDock(checkBox, Dock.Right);
            dockPanel.Children.Add(checkBox);

            // Додавання нових TextBox і CheckBox до словника їхніх пар
            variantsDict.Add(textBox, checkBox);

            // Додавання створеної StackPanel до WrapPanel
            dynamicWrapPanel.Children.Add(dockPanel);
        }
        /// <summary>
        /// Видаляє останній варіант відповіді
        /// </summary>
        private void RemoveLastVariant()
        {
            if (dynamicWrapPanel.Children.Count != 0)
            {
                dynamicWrapPanel.Children.RemoveAt(dynamicWrapPanel.Children.Count - 1);
                variantsDict.Remove(variantsDict.Keys.Last());
            }
        }
        /// <summary>
        /// Додає/оновлює дані про поточне запитання у списку структур Test.Question
        /// </summary>
        /// <returns>true при успішному доданні/оновленні; false - якщо трапилась помилка</returns>
        private bool UpdateCurrentQuestionInfo()
        {
            try
            {
                // Індекс для операцій в списку
                int indexOfCurrentQuestion = currentQuestionIndex - 1;
                // Структура даних поточного питання
                Test.Question currQuestion;

                if (!AllTextboxesFilled())
                {
                    throw new ArgumentException();
                }
                if(!AllTextboxesContainProperInfo())
                {
                    throw new FormatException();
                }
                // Заповнення структури всіма потрібними даними
                currQuestion.question = QuestionInput.Text.Trim();
                // Спроба пошуку картинки під індексом запитання (від 1 до 10)
                ImageManager.ImageInfo foundImage = imagesList.Find(x => x.questionIndex == currentQuestionIndex);
                // По замовчуванню hasLinkedImage = false
                currQuestion.hasLinkedImage = false;
                // Якщо повернено не значення по замовчуванню, то запис знайдено
                if (!foundImage.Equals(default(ImageInfo)))
                {
                    currQuestion.hasLinkedImage = true;
                }
                // Заповнення даними про варіанти
                currQuestion.variants = new List<string>();
                currQuestion.correctVariantsIndexes = new List<int>();
                int tempIndexForCorrectVariants = 0;
                foreach (var pairOfTextBoxAndCheckBox in variantsDict)
                {
                    // Якщо варіант позначено правильним, додаємо його індекс до списку індексів правильних варіантів
                    if ((bool)pairOfTextBoxAndCheckBox.Value.IsChecked)
                    {
                        currQuestion.correctVariantsIndexes.Add(tempIndexForCorrectVariants);
                    }
                    currQuestion.variants.Add(pairOfTextBoxAndCheckBox.Key.Text.Trim());
                    tempIndexForCorrectVariants++;
                }
                /* Якщо не позначено жодного правильного варіанту
                 * або загалом є лиш 1 варіант
                */
                if (currQuestion.correctVariantsIndexes.Count == 0 || currQuestion.variants.Count == 1)
                {
                    throw new ArgumentNullException();
                }
                // Якщо даний індекс вже містить дані, перезаписуємо їх на щойно сформовані
                if (questionsList.Count >= indexOfCurrentQuestion+1)
                {
                    // Вставка нової структури за даним індексом
                    questionsList.Insert(indexOfCurrentQuestion, currQuestion);
                    // Видалення старої структури
                    questionsList.RemoveAt(indexOfCurrentQuestion+1);
                }
                else
                {
                    // Інакше просто додаємо структуру як нове значення
                    questionsList.Add(currQuestion);
                }
                // Якщо в процесі не виникло жодної помилки
                return true;
            }
            catch (ArgumentNullException)
            {
                MessageBox.Show("Позначте хоча б один варіант відповіді як правильний");
                return false;
            }
            catch (ArgumentException)
            {
                MessageBox.Show("Будь ласка, заповніть всі поля");
                return false;
            }
            catch (FormatException)
            {
                MessageBox.Show("Використовуйте не тільки цифри для заповнення полів");
                return false;
            }
        }
        /// <summary>
        /// Оновлює кольори полів варіантів відповідей залежно від значень TextBoxів
        /// </summary>
        private void UpdateVariantsCheckedConditions()
        {
            try
            {
                foreach(var currVariant in variantsDict)
                {
                    if ((bool)currVariant.Value.IsChecked)
                    {
                        currVariant.Key.Background = new SolidColorBrush(Colors.DarkGreen);
                        currVariant.Key.Foreground = new SolidColorBrush(Colors.White);
                    }
                    else
                    {
                        currVariant.Key.Background = (Brush)new BrushConverter().ConvertFrom("#fff0f0");
                        currVariant.Key.Foreground = new SolidColorBrush(Colors.Black);
                    }
                }
            }
            catch(NullReferenceException)
            {
                MessageBox.Show($"Немає варіантів відповідей. Створіть їх, використовуючи кнопку \"{AddVariant_Button.Content}\"");
            }
        }
        /// <summary>
        /// Обробка події, коли натиснуто клавішу на клавіатурі
        /// </summary>
        /// <remarks>F1 - посібник користувача;
        /// Esc - попереднє запитання/вікно;
        /// Enter - наступне запитання/вікно збереження тесту</remarks>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1)
            {
                HelpCenter_Window helpCenter = new HelpCenter_Window();
                helpCenter.Show();
            }
            if (e.Key == Key.Escape)
            {
                // Якщо поточне запитання не перше, то відображаємо попереднє
                if(currentQuestionIndex >= 1)
                {
                    if (!UpdateCurrentQuestionInfo()) return;
                    currentQuestionIndex--;
                    ShowQuestionAtIndex(currentQuestionIndex - 1);
                    return;
                }
                // Інакше повертаємось на попереднє вікно
                GoToMainWithConfimation();
            }
            if(e.Key == Key.Enter)
            {
                // Якщо поточне запитання не останнє, то відображаємо наступне
                if(currentQuestionIndex != 10)
                {
                    GoToNextQuestion();
                    return;
                }
                // Інакше відкриваємо вікно збереження тесту
                GoToSavingWindow();
            }
        }
        /// <summary>
        /// Викликає ConfirmWindow для підтвердженням користувачем переходу на головне вікно
        /// </summary>
        private void GoToMainWithConfimation()
        {
            string confirmationMessage = "Всі дані цього тесту втратяться, коли ви перейдете на головну сторінку.";
            ConfirmWindow confirmWindow = new ConfirmWindow(ConfirmActionsWindowModes.TEST_CHANGE_TO_MAIN,
                confirmationMessage, questionsList, imagesList, testInfo, currentQuestionIndex);
            confirmWindow.Show();
            askForClosingComfirmation = false;
            Close();
        }
        /// <summary>
        /// Зберігає поточне запитання тесту, створює/відображає наступне
        /// </summary>
        private void GoToNextQuestion()
        {
            if (!UpdateCurrentQuestionInfo()) return;
            if (questionsList.Count > currentQuestionIndex)
            {
                currentQuestionIndex++;
                ShowQuestionAtIndex(currentQuestionIndex - 1);
                return;
            }
            if (currentQuestionIndex == totalQuestionsCount && currentQuestionIndex != 10)
            {
                totalQuestionsCount++;
                currentQuestionIndex++;
                UpdateGUI();
                EraseElementsData();
            }
        }
        /// <summary>
        /// Зберігає поточне запитання тесту, відкриває TestSaving_Window
        /// </summary>
        private void GoToSavingWindow()
        {
            if (!UpdateCurrentQuestionInfo()) return;
            /* Якщо режим створення, то ініціалізуємо TestSaving_Window в режимі створення тесту
             * Якщо не режим створення, то ініціалізуємо TestSaving_Window в режимі редагування тесту
             */
            ReturnDefaultImage();
            TestSaving_Window testSaving_Window = creatingMode ?
                new TestSaving_Window(questionsList, imagesList) :
                new TestSaving_Window(questionsList, imagesList, testInfo);
            testSaving_Window.Show();
            askForClosingComfirmation = false;
            Close();
        }
        /// <summary>
        /// Передає структуру даних поточного запитання в GUI
        /// </summary>
        /// <remarks>Розподіляє дані з структури по списках, формує та оновлює відповідний GUI</remarks>
        /// <param name="questions">Список структур запитань тесту</param>
        public void GetListAndPutItInGUI(List<Test.Question> questions)
        {
            Test.Question currentQuestion = questions[currentQuestionIndex - 1];
            QuestionInput.Text = currentQuestion.question;
            int tempIndexOfCorrectVariant = 0;
            foreach(string variant in currentQuestion.variants)
            {
                // В списку індексів правильних варіантів міститься/не міститься поточний індекс
                bool variantIsCorrect = currentQuestion.correctVariantsIndexes.Contains(tempIndexOfCorrectVariant);
                AddNewVariant(variant, variantIsCorrect);
                tempIndexOfCorrectVariant++;
            }
            UpdateVariantsCheckedConditions();
            UpdateGUI();
        }
        /// <summary>
        /// Відображає запитання тесту під заданим індексом
        /// </summary>
        /// <param name="indexOfElementToEdit">Індекс елемента для відображення (значення від 0 до 9)</param>
        private void ShowQuestionAtIndex(int indexOfElementToEdit)
        {
            currentQuestionIndex = ++indexOfElementToEdit;
            totalQuestionsCount = questionsList.Count;
            EraseElementsData();
            GetListAndPutItInGUI(questionsList);
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
        /// <summary>
        /// Обробка події, коли натиснуто GUI кнопку ImageChange_Button
        /// </summary>
        /// <remarks>Відкриває діалогове вікно вибору нової ілюстрації</remarks>
        private void ImageChange_Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.jpg, *.jpeg, *.png, *.bmp)|*.jpg;*.jpeg;*.png;*.bmp)";
            // Якщо обрано якийсь файл
            if (openFileDialog.ShowDialog() == true)
            {
                // Отримання шляху до обраного файлу
                string selectedFilePath = openFileDialog.FileName;
                openFileDialog = null;

                // Заповнення структури з інформацією про картинку
                ImageManager.ImageInfo currentImageInfo = new ImageManager.ImageInfo();
                currentImageInfo.questionIndex = currentQuestionIndex;
                currentImageInfo.imagePath = selectedFilePath;
                // Оновлення списку структур з інформацією про картинки
                PushImageSource(currentImageInfo);
                // Відображення обраної картинки
                BitmapImage tempImageSource = new BitmapImage(new Uri(selectedFilePath, UriKind.RelativeOrAbsolute));
                IllustrationImage.Source = tempImageSource;
                tempImageSource.StreamSource = null;
            }
        }
        /// <summary>
        /// Обробка події, коли натиснуто GUI кнопку ImageDeletion_Button
        /// </summary>
        /// <remarks>Видаляє прив'язку до ілюстрації</remarks>
        private void ImageDeletion_Button_Click(object sender, RoutedEventArgs e)
        {
            // Спроба пошуку картинки під індексом запитання (від 1 до 10)
            ImageManager.ImageInfo foundImage = imagesList.Find(x => x.questionIndex == currentQuestionIndex);
            // Якщо повернено не значення по замовчуванню, то запис знайдено
            if (!foundImage.Equals(default(ImageInfo)))
            {
                int indexToRemove = imagesList.IndexOf(foundImage);
                // Видалення старої структури
                imagesList.RemoveAt(indexToRemove);

                ReturnDefaultImage();
                // Видалення прив'язки до картинки за її наявності
                if (!creatingMode && questionsList.Count >= currentQuestionIndex)
                {
                    if (questionsList[currentQuestionIndex - 1].hasLinkedImage)
                    {
                        Test.Question questionToUpdate = questionsList[currentQuestionIndex - 1];
                        questionToUpdate.hasLinkedImage = false;
                        questionsList.Insert(currentQuestionIndex - 1, questionToUpdate);
                        questionsList.RemoveAt(currentQuestionIndex);
                    }
                }
            }
        }
        /// <summary>
        /// Повернення картинки для відображення по замовчуванню
        /// </summary>
        private void ReturnDefaultImage()
        {
            string defaultImageRelativePath = ConfigurationManager.AppSettings["defaultImageSource"];
            string defaultImageAbsolutePath = Path.GetFullPath(defaultImageRelativePath);

            BitmapImage defaultImage = new BitmapImage(
            new Uri(defaultImageAbsolutePath, UriKind.Absolute));
            IllustrationImage.Source = defaultImage;
            defaultImage.StreamSource = null;
        }
        /// <summary>
        /// Оновлює картинку, що відображається на поточному запитанні
        /// </summary>
        private void UpdateImageAppearance()
        {
            ReturnDefaultImage();
            // Якщо список картинок порожній, то більше нічого не робимо
            if (imagesList.Count == 0 || imagesList == null) return;
            // Спроба пошуку картинки під індексом запитання (від 1 до 10)
            ImageManager.ImageInfo foundImage = imagesList.Find(x => x.questionIndex == currentQuestionIndex);

            // Якщо повернено не значення по замовчуванню, то запис знайдено
            if (!foundImage.Equals(default(ImageInfo)))
            {
                // Відображення знайденої картинки
                BitmapImage foundImageBitmap = new BitmapImage(new Uri(foundImage.imagePath, UriKind.RelativeOrAbsolute));
                IllustrationImage.Source = foundImageBitmap;
                foundImageBitmap.StreamSource = null;
            }
        }
        /// <summary>
        /// Оновлює список інформації про ілюстрації
        /// </summary>
        /// <remarks>Додає або оновлює інформацію</remarks>
        /// <param name="imageInfoToPush">Структура інформації про картинку</param>
        private void PushImageSource(ImageManager.ImageInfo imageInfoToPush)
        {
            // Якщо список порожній, тільки додаємо поточну інформацію про картинку
            if (imagesList.Count == 0)
            {
                imagesList.Add(imageInfoToPush);
                return;
            }
            // Спроба пошуку картинки під індексом запитання (від 1 до 10)
            ImageManager.ImageInfo foundImage = imagesList.Find(x => x.questionIndex == currentQuestionIndex);
            // Якщо повернено не значення по замовчуванню, то запис знайдено
            if (!foundImage.Equals(default(ImageInfo)))
            {
                int indexToUpdate = imagesList.IndexOf(foundImage);
                // Вставка нової структури в список за потрібним індексом
                imagesList.Insert(indexToUpdate, imageInfoToPush);
                // Видалення старої структури
                imagesList.RemoveAt(indexToUpdate + 1);
            }
            else
            {
                // Інакше просто додаємо структуру в список
                imagesList.Add(imageInfoToPush);
            }
            // Сортування для покращення читабельності при подальшій розробці
            imagesList.Sort((a, b) => a.questionIndex.CompareTo(b.questionIndex));
        }
    }
}
