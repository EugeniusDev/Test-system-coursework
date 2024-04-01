using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace courseWork_project
{
    /// <summary>
    /// Логіка взаємодії з ConfirmWindow.xaml
    /// </summary>
    /// <remarks> Вікно ConfirmWindow.xaml використовується для підтвердження виконання дії</remarks>
    public partial class ConfirmWindow : Window
    {
        /// <summary>
        /// Список з TestStructs.Question для тимчасового збережння даних запитань тесту
        /// </summary>
        private List<TestStructs.Question> questionsThatCanBeLost;
        /// <summary>
        /// Масив структур для збереження даних про ілюстрації
        /// </summary>
        private List<ImageManager.ImageInfo> imagesList;
        /// <summary>
        /// Структура з інформацією про тест для тимчасового збереження даних тесту
        /// </summary>
        private TestStructs.TestInfo testInfo;
        /// <summary>
        /// Індекс запитання, яке потрібно буде відкрити в разі відмови виконання дії
        /// </summary>
        private int indexToResumeEditingFrom;
        /// <summary>
        /// Назва тесту, видалення якого потребує підтвердження
        /// </summary>
        private string testTitle;
        /// <summary>
        /// Змінна для збереження типу вікна підтвердження
        /// </summary>
        private ConfirmActionsWindowModes confirmationMode;
        /// <summary>
        /// Змінна, на основі якої буде з'являтись вікно підтвердження закриття вікна
        /// </summary>
        bool askForClosingComfirmation = true;

        /// <summary>
        /// Конструктор для вікна підтвердження; Дія, що потребує підтвердження, передбачає закриття вікна
        /// </summary>
        /// <remarks>Приймає 5 параметрів</remarks>
        /// <param name="confirmMode">Тип вікна підтвердження</param>
        /// <param name="actionDescription">Опис дії, яку повинен підтвердити користувач (ставте . в кінці)</param>
        /// <param name="questionsThatCanBeLost">Список структур TestStructs.Question, що буде втрачена внаслідок підтвердження дії</param>
        /// <param name="testInfo">Структура TestStructs.TestInfo, що буде втрачена внаслідок підтвердження дії</param>
        /// <param name="indexToResumeEditingFrom">Індекс запитання, значення від 1 до 10</param>
        public ConfirmWindow(ConfirmActionsWindowModes confirmMode, string actionDescription, List<TestStructs.Question> questionsThatCanBeLost, List<ImageManager.ImageInfo> imageInfos, TestStructs.TestInfo testInfo, int indexToResumeEditingFrom)
        {
            confirmationMode = confirmMode;
            this.questionsThatCanBeLost = questionsThatCanBeLost;
            this.indexToResumeEditingFrom = indexToResumeEditingFrom;
            this.testInfo = testInfo;
            imagesList = imageInfos;

            InitializeComponent();
            Action_Text.Text = string.Concat(actionDescription, " Ви впевнені, що хочете це зробити?");
        }
        /// <summary>
        /// Конструктор для вікна підтвердження; Дія, що потребує підтвердження, не закриває вікно
        /// </summary>
        /// <param name="confirmMode">Тип вікна підтвердження</param>
        /// <param name="actionDescription">Опис дії, яку повинен підтвердити користувач (ставте . в кінці)</param>
        /// <param name="testTitle"></param>
        public ConfirmWindow(ConfirmActionsWindowModes confirmMode, string actionDescription, string testTitle)
        {
            confirmationMode = confirmMode;
            this.testTitle = testTitle;
            InitializeComponent();
            Action_Text.Text = string.Concat(actionDescription, " Ви впевнені, що хочете це зробити?");
        }
        /// <summary>
        /// Обробка події, коли натиснуто GUI кнопку Yes_Button
        /// </summary>
        /// <remarks>Викликає ActionConfirmed</remarks>
        private void Yes_Button_Click(object sender, RoutedEventArgs e)
        {
            ActionConfirmed();
        }
        /// <summary>
        /// Обробка події, коли натиснуто GUI кнопку No_Button
        /// </summary>
        /// <remarks>Викликає ActionCancelled</remarks>
        private void No_Button_Click(object sender, RoutedEventArgs e)
        {
            ActionCancelled();
        }
        /// <summary>
        /// Обробка події, коли натиснуто клавішу на клавіатурі
        /// </summary>
        /// <remarks>F1 - посібник користувача;
        /// Esc - відмова від дії;
        /// Enter - підтвердження виконання дії</remarks>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1)
            {
                HelpCenter_Window helpCenter = new HelpCenter_Window();
                helpCenter.Show();
            }
            if(e.Key == Key.Escape)
            {
                ActionCancelled();
            }
            if(e.Key == Key.Enter)
            {
                ActionConfirmed();
            }
        }
        /// <summary>
        /// Виконує дію, що потребувала підтвердження
        /// </summary>
        /// <remarks>Ці дії відрізняються залежно від значення confirmationMode</remarks>
        private void ActionConfirmed()
        {
            switch (confirmationMode)
            {
                case ConfirmActionsWindowModes.TEST_CHANGE_TO_MAIN:
                    // Повернення до MainWindow
                    MainWindow mainWindow1 = new MainWindow();
                    mainWindow1.Show();
                    askForClosingComfirmation = false;
                    Close();
                    break;
                case ConfirmActionsWindowModes.MAIN_DELETE_TEST:
                    // Видалення обраного тесту
                    DataDecoder.EraseFolder(testTitle);
                    // Видалення прив'язаних до обраного тесту картинок
                    ImageManager.ImagesCleanup(testTitle);
                    // Повернення до MainWindow
                    MainWindow mainWindow2 = new MainWindow();
                    mainWindow2.Show();
                    askForClosingComfirmation = false;
                    Close();
                    break;
                case ConfirmActionsWindowModes.TEST_TAKING_TO_MAIN:
                    // Повернення до MainWindow
                    MainWindow mainWindow3 = new MainWindow();
                    mainWindow3.Show();
                    askForClosingComfirmation = false;
                    Close();
                    break;
            }
        }
        /// <summary>
        /// Повертається до попереднього вікна з поверненням всіх потрібних даних
        /// </summary>
        /// <remarks>Вікна та дані відрізняються залежно від значення confirmationMode</remarks>
        private void ActionCancelled()
        {
            switch (confirmationMode)
            {
                case ConfirmActionsWindowModes.TEST_CHANGE_TO_MAIN:
                    // Повернення на запитання з наданим індексом у вікні TestChange_Window
                    TestChange_Window testChange_Window = new TestChange_Window(questionsThatCanBeLost, imagesList, testInfo, indexToResumeEditingFrom);
                    testChange_Window.Show();
                    askForClosingComfirmation = false;
                    Close();
                    break;
                case ConfirmActionsWindowModes.MAIN_DELETE_TEST:
                    // Повернення до MainWindow
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    askForClosingComfirmation = false;
                    Close();
                    break;
                case ConfirmActionsWindowModes.TEST_TAKING_TO_MAIN:
                    // Перепроходження тесту шляхом повернення на NameEntry_Window
                    NameEntry_Window nameEntry_Window = new NameEntry_Window(questionsThatCanBeLost, testInfo);
                    nameEntry_Window.Show();
                    askForClosingComfirmation = false;
                    Close();
                    break;
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
            if (result == MessageBoxResult.No)
            {
                // Скасує процес закриття вікна
                e.Cancel = true;
            }
        }
    }
}
