using System;
using System.Collections.Generic;
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
    /// Interaction logic for TestTaking_Window.xaml
    /// </summary>
    /// <remarks>TestTaking_Window.xaml is used for tasking tests</remarks>
    public partial class TestTaking_Window : Window
    {
        /// <summary>
        /// Index of test's current question
        /// </summary>
        /// <remarks>Values from 1 to 10</remarks>
        private int currentQuestionIndex = 1;
        /// <summary>
        /// Total count of questions of a test
        /// </summary>
        /// <remarks>Values from 1 to 10</remarks>
        private int totalQuestionsCount;
        /// <summary>
        /// Count of correctly answered questions
        /// </summary>
        /// <remarks>Values from 1 to 10</remarks>
        private int correctAnswersCount = 0;
        /// <summary>
        /// Prompted name of a user who takes a test
        /// </summary>
        private readonly string userName;
        /// <summary>
        /// Used to determine if a variant was selected
        /// </summary>
        private bool buttonClicked = false;
        /// <summary>
        /// List of test's questions
        /// </summary>
        private readonly List<TestStructs.Question> questionsList;
        /// <summary>
        /// Structure with test's info
        /// </summary>
        private TestStructs.TestInfo testInfo;
        /// <summary>
        /// Used for operating with images if they exist
        /// </summary>
        private readonly string transliteratedTestTitle;
        /// <summary>
        /// Button-bool dictionary used for determining correct variants
        /// </summary>
        /// <remarks>Changes while passing through questions</remarks>
        private Dictionary<Button, bool> variantsDict = new Dictionary<Button, bool>();
        /// <summary>
        /// Used for controlling a timer
        /// </summary>
        private int timeLimitInSeconds = 0;
        /// <summary>
        /// Timer object
        /// </summary>
        private Timer timer = new Timer();
        /// <summary>
        /// Used to determine if window closing confirmation is needed
        /// </summary>
        bool askForClosingComfirmation = true;
        /// <summary>
        /// Used to determine if test is not empty
        /// </summary>
        readonly bool loadedSuccessfully = true;
        public bool LoadedSuccessfully { get { return loadedSuccessfully; } }

        /// <summary>
        /// 3-argument TestTaking_Window constructor
        /// </summary>
        /// <param name="questionsList">Test's question list</param>
        /// <param name="currTestInfo">Structure with test's general info</param>
        /// <param name="userName">Name of user that takes a test</param>
        public TestTaking_Window(List<TestStructs.Question> questionsList, TestStructs.TestInfo currTestInfo, string userName)
        {
            // If there are no questions in the test, close the window
            if (questionsList.Count == 0)
            {
                MessageBox.Show("Схоже, всі запитання тесту було видалено", "Помилка проходження тесту",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                askForClosingComfirmation = false;
                loadedSuccessfully = false;
                Close();
                return;
            }

            this.questionsList = questionsList;
            testInfo = currTestInfo;
            transliteratedTestTitle = DataDecoder.TransliterateAString(testInfo.testTitle);
            totalQuestionsCount = questionsList.Count;
            this.userName = userName;

            InitializeComponent();

            // Displaying general info about test before an attempt of passing it
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
            // If there are no limitations, timer does not work
            if (noTimeLimits)
            {
                Timer_TextBlock.Text = string.Empty;
                Timer_TextBlock.Visibility = Visibility.Collapsed;
                return;
            }
            // Initial timer value (time limit itself)
            Timer_TextBlock.Text = $"{timeLimitInSeconds / 60}:{timeLimitInSeconds % 60}";
            // Initialization and start of a timer
            timer.Interval = 1000;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }
        /// <summary>
        /// Called by a timer every second
        /// </summary>
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            timeLimitInSeconds--;
            // If time is up, stop the timer and the test taking and show the results
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
        /// Handling pressed BackToMain_Button
        /// </summary>
        /// <remarks>Calls TryOpenMainWindow</remarks>
        private void BackToMain_Button_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            GoToMainWithConfimation();
        }
        /// <summary>
        /// Handling pressed NextQuestion_Button
        /// </summary>
        /// <remarks>Calls GoToNextQuestion</remarks>
        private void NextQuestion_Button_Click(object sender, RoutedEventArgs e)
        {
            GoToNextQuestion();
        }
        /// <summary>
        /// Handling pressed EndTest_Button
        /// </summary>
        /// <remarks>Calls TryToFinishTheTest</remarks
        private void EndTest_Button_Click(object sender, RoutedEventArgs e)
        {
            TryToFinishTheTest();
        }
        /// <summary>
        /// Forms results of test passing
        /// </summary>
        /// <returns>String with results info</returns>
        private string FormResultsOfTest()
        {
            string resultsToReturn = $"{userName}: {correctAnswersCount}/{totalQuestionsCount}";
            return resultsToReturn;
        }
        /// <summary>
        /// Updates results list of current test and opens MainWindow
        /// </summary>
        /// <param name="currentResult">String with current test's passing results</param>
        private void EndTestAndSaveResults(string currentResult)
        {
            FileWriter fileWriter = new FileWriter(testInfo.testTitle);
            fileWriter.AppendTestTakingData(testInfo, currentResult);

            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            askForClosingComfirmation = false;
            Close();
        }
        /// <summary>
        /// Updates visibility of all changeable GUI elements
        /// </summary>
        private void UpdateGUI()
        {
            CurrentQuestion_Text.Text = $"{currentQuestionIndex}/{totalQuestionsCount}";

            if (currentQuestionIndex == totalQuestionsCount)
            {
                NextQuestion_Button.Visibility = Visibility.Collapsed;
            }
            else NextQuestion_Button.Visibility = Visibility.Visible;

            if (currentQuestionIndex == totalQuestionsCount)
            {
                EndTest_Button.Visibility = Visibility.Visible;
            }
        }
        /// <summary>
        /// Updates image of current question (if such is linked)
        /// </summary>
        private void UpdateImageAppearance()
        {
            try
            {
                string[] allImagesPaths = Directory.GetFiles(ImagesDirectory);
                if (allImagesPaths.Length == 0) throw new ArgumentNullException();

                foreach (string currentImagePath in allImagesPaths)
                {
                    // Finding required image from images folder
                    if (questionsList[currentQuestionIndex-1].hasLinkedImage
                        && currentImagePath.Contains($"{transliteratedTestTitle}-{currentQuestionIndex}"))
                    {
                        // Displaying found image
                        string absoluteImagePath = Path.GetFullPath(currentImagePath);
                        BitmapImage foundImageBitmap = new BitmapImage();
                        foundImageBitmap.BeginInit();
                        foundImageBitmap.UriSource = new Uri(absoluteImagePath, UriKind.Absolute);
                        foundImageBitmap.EndInit();
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
        /// Erases data from QuestionText, WrapPanel, Button-bool dictionary
        /// </summary>
        private void ClearElementsData()
        {
            QuestionText.Text = string.Empty;
            wrapPanelOfVariants.Children.Clear();
            variantsDict.Clear();
        }
        /// <summary>
        /// Displays a question from a list at given index
        /// </summary>
        /// <param name="indexOfElementToReturnTo">Index of question (values from 0 to 9)</param>
        private void ShowQuestionAtIndex(int indexOfElementToReturnTo)
        {
            currentQuestionIndex = ++indexOfElementToReturnTo;
            ClearElementsData();
            UpdateGUI();
            GetListAndPutItInGUI(questionsList);
        }
        /// <summary>
        /// Adds answer variant with specified values
        /// </summary>
        /// <param name="variantText">Text of answer variant</param>
        /// <param name="isCorrect">Is the variant correct?</param>
        private void AddNewVariant(string variantText, bool isCorrect)
        {
            if (wrapPanelOfVariants.Children.Count >= 8) return;

            Button button = new Button
            {
                Content = variantText,
                Foreground = new SolidColorBrush(Colors.Black),
                Background = (Brush)new BrushConverter().ConvertFrom("#fff0f0"),
                FontSize = 24,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                ToolTip = "Клацніть, щоб обрати цей варіант",
                Margin = new Thickness(3),
                MinWidth = 200,
                MaxWidth = 260
            };
            button.Click += VariantButton_Click;
            variantsDict.Add(button, isCorrect);
            wrapPanelOfVariants.Children.Add(button);
        }
        /// <summary>
        /// Checking and color-marking choice of user
        /// </summary>
        private void VariantButton_Click(object sender, RoutedEventArgs e)
        {
            if (buttonClicked) return;

            buttonClicked = true;

            Button clickedButton = (Button)sender;
            string clickedButtonContent = clickedButton.Content.ToString();
            // Changing focus on other buttons for convenient Enter usage (depends on question index)
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
                if (string.Compare(currButtonContent, clickedButtonContent) == 0)
                {
                    buttonBoolPair.Key.Background = new SolidColorBrush(Colors.DarkRed);
                    buttonBoolPair.Key.Foreground = new SolidColorBrush(Colors.White);

                    bool isSelectedVariandCorrect = buttonBoolPair.Value;
                    if (isSelectedVariandCorrect)
                    {
                        correctAnswersCount++;
                    }

                    break;
                }
            }

            ShowCorrectAnswers();
        }
        /// <summary>
        /// Changes colors of buttons of variants according to their correctness
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
        /// Handling pressed keyboard keys
        /// </summary>
        /// <remarks>F1 - user manual;
        /// Esc - previous window;
        /// Enter - next question/end test passing attempt</remarks>
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
                if (currentQuestionIndex != totalQuestionsCount)
                {
                    GoToNextQuestion();
                    return;
                }

                TryToFinishTheTest();
            }
        }
        /// <summary>
        /// User must to confirm going to MainWindow
        /// </summary>
        private void GoToMainWithConfimation()
        {
            string confirmationString = "Натиснувши \"Так\", ви перейдете на головну сторінку, втративши дані проходження тесту. Ви справді хочете це зробити?";
            MessageBoxResult result = MessageBox.Show(confirmationString,
                "Підтвердження переходу на головну сторінку", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result.Equals(MessageBoxResult.Yes))
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                askForClosingComfirmation = false;
                Close();
            }
            // If user did not confirmed the redirection, resume timer
            timer.Start();
        }
        /// <summary>
        /// Shows results of passing attempt and calls method for their saving
        /// </summary>
        private void TryToFinishTheTest()
        {
            try
            {
                if (!buttonClicked)
                {
                    throw new ArgumentNullException();
                }

                timer.Stop();

                string resultsOfTest = FormResultsOfTest();
                MessageBox.Show($"Тест \"{testInfo.testTitle}\" пройдено!\nРезультати тестування:\n{resultsOfTest}", "Результати проходження тесту");

                EndTestAndSaveResults(resultsOfTest);
            }
            catch (ArgumentNullException)
            {
                MessageBox.Show("Оберіть варіант відповіді, який вважаєте правильним");
            }
        }
        /// <summary>
        /// Displays next question if such exist
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
        /// Puts questions structure in GUI
        /// </summary>
        /// <param name="questions">List of questions</param>
        public void GetListAndPutItInGUI(List<TestStructs.Question> questions)
        {
            TestStructs.Question currentQuestion = questions[currentQuestionIndex - 1];
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
                bool variantIsCorrect = currentQuestion.correctVariantsIndeces.Contains(tempIndexOfCorrectVariant);
                AddNewVariant(variant, variantIsCorrect);
                tempIndexOfCorrectVariant++;
            }

            UpdateGUI();
        }
        /// <summary>
        /// Handling window closing event
        /// </summary>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // If closing confirmation is not needed, just close the window
            if (!askForClosingComfirmation) return;
            MessageBoxResult result = MessageBox.Show("Ви справді хочете закрити програму?", "Підтвердження закриття вікна", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result.Equals(MessageBoxResult.No))
            {
                // Cancelling closing process
                e.Cancel = true;
            }
        }
    }
}
