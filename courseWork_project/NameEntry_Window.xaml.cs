using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace courseWork_project
{
    /// <summary>
    /// Логіка взаємодії з NameEntry_Window.xaml
    /// </summary>
    /// <remarks> Вікно NameEntry_Window.xaml використовується для введення імені користувача і відкриття TestTaking_Window</remarks>
    public partial class NameEntry_Window : Window
    {
        /// <summary>
        /// Список з TestStructs.Question для оперування даними запитань тесту
        /// </summary>
        private List<TestStructs.Question> questionsList;
        /// <summary>
        /// Структура з інформацією про тест
        /// </summary>
        private TestStructs.TestInfo testInfo;
        /// <summary>
        /// Змінна, на основі якої буде з'являтись вікно підтвердження закриття вікна
        /// </summary>
        bool askForClosingComfirmation = true;

        /// <summary>
        /// Конструктор NameEntry_Window, приймає 2 аргументи
        /// </summary>
        /// <remarks>Всі аргументи будуть передані у вікно TestTaking</remarks>
        /// <param name="questionsList">Список з TestStructs.Question для оперування даними запитань тесту</param>
        /// <param name="currTestInfo">Структура з інформацією про тест</param>
        public NameEntry_Window(List<TestStructs.Question> questionsList, TestStructs.TestInfo currTestInfo)
        {
            this.questionsList = questionsList;
            this.testInfo = currTestInfo;
            InitializeComponent();
        }
        /// <summary>
        /// Обробка події, коли натиснуто GUI кнопку BackToMain_Button
        /// </summary>
        /// <remarks>Викликає GoToMainWindow</remarks>
        private void BackToMain_Button_Click(object sender, RoutedEventArgs e)
        {
            GoToMainWindow();
        }
        /// <summary>
        /// Обробка події, коли клацнуто на поле вводу ім'я користувача
        /// </summary>
        private void UsernameTextBlock_GotFocus(object sender, RoutedEventArgs e)
        {
            // У полі текст по замовчуванню
            bool titleContainsDefaultText = UsernameTextBlock != null
                && string.Compare(UsernameTextBlock.Text, "Введіть ім'я тут") == 0;
            if (titleContainsDefaultText || testInfo.testTitle == null)
            {
                // Стираємо поле назви тесту та міняємо колір тексту
                UsernameTextBlock.Foreground = new SolidColorBrush(Colors.Black);
                UsernameTextBlock.Text = string.Empty;
            }
        }
        /// <summary>
        /// Обробка події, коли поле вводу ім'я користувача втратило фокус
        /// </summary>
        /// <remarks>Тобто коли після фокусу на полі клацнуто на будь-що інше</remarks>
        private void UsernameTextBlock_LostFocus(object sender, RoutedEventArgs e)
        {
            // Якщо поле порожнє, то повертаємо значення по замовчуванню
            bool fieldIsEmpty = UsernameTextBlock != null && string.IsNullOrWhiteSpace(UsernameTextBlock.Text);
            if (fieldIsEmpty)
            {
                // Міняємо значення поля назви тесту та міняємо колір тексту
                UsernameTextBlock.Text = "Введіть ім'я тут";
                UsernameTextBlock.Foreground = new SolidColorBrush(Colors.DarkGray);
            }
        }
        /// <summary>
        /// Обробка події, коли натиснуто GUI кнопку BeginTest_Button
        /// </summary>
        /// <remarks>Викликає TryToBeginTest</remarks
        private void BeginTest_Button_Click(object sender, RoutedEventArgs e)
        {
            TryToBeginTest();
        }
        /// <summary>
        /// Обробка події, коли натиснуто клавішу на клавіатурі
        /// </summary>
        /// <remarks>F1 - посібник користувача;
        /// Esc - повернення до MainWindow;
        /// Enter - виклик TryToBeginTest</remarks>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1)
            {
                HelpCenter_Window helpCenter = new HelpCenter_Window();
                helpCenter.Show();
            }
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
        /// Відкриває вікно MainWindow
        /// </summary>
        private void GoToMainWindow()
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            askForClosingComfirmation = false;
            Close();
        }
        /// <summary>
        /// За наявності введеного ім'я користувача відкриває TestTaking_Window
        /// </summary>
        private void TryToBeginTest()
        {
            try
            {
                string userName = UsernameTextBlock.Text;
                bool fieldIsEmptyOrDefault = UsernameTextBlock != null && string.IsNullOrWhiteSpace(userName)
                    || string.Compare(UsernameTextBlock.Text, "Введіть ім'я тут") == 0;
                if (fieldIsEmptyOrDefault) throw new ArgumentNullException();

                TestTaking_Window testTaking_Window = new TestTaking_Window(questionsList, testInfo, userName);
                testTaking_Window.Show();
                askForClosingComfirmation = false;
                Close();
            }
            catch (ArgumentNullException)
            {
                MessageBox.Show("Введіть ім'я перед тим, як розпочати тест");
            }
        }
        /// <summary>
        /// Обробка події, коли вікно закривається
        /// </summary>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Якщо підтвердження закриття не потрібне, то нічого не робимо
            if (!askForClosingComfirmation) return;
            MessageBoxResult result = MessageBox.Show("Ви справді хочете закрити програму?", "Підтвердження закриття вікна", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result.Equals(MessageBoxResult.No))
            {
                // Скасує процес закриття вікна
                e.Cancel = true;
            }
        }
    }
}
