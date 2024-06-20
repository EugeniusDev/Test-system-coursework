using courseWork_project.GuiManipulation;
using System.Collections.Generic;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static courseWork_project.ImageManager;

namespace courseWork_project
{
    public partial class TestPass_Window : Window
    {
        private readonly Test testToPass;

        private int currentQuestionIndex = 0;
        private int correctAnswersCount = 0;
        private readonly string testPasserUserName;
        private bool isCurrentVariantChosen = false;
        private readonly Dictionary<Button, bool> variants = new Dictionary<Button, bool>();

        private int timeLimitInSeconds = 0;
        private readonly Timer timer = new Timer();

        private bool isWindowClosingConfirmationRequired = true;
        private bool loadedSuccessfully = true;
        public bool LoadedSuccessfully { get { return loadedSuccessfully; } }

        public TestPass_Window(Test testToPass, string userName)
        {
            if (!IsTestDataValid(testToPass))
            {
                loadedSuccessfully = false;
                GoToMainWindow();
            }

            this.testToPass = testToPass;
            testPasserUserName = userName;
            timeLimitInSeconds = testToPass.TestMetadata.timerValueInMinutes * 60;

            InitializeComponent();

            PromptUserToConfirmStart(testToPass);
            DisplayFollowingQuestion();
            SetUpTimer();
        }

        private static bool IsTestDataValid(Test testToPass)
        {
            if (testToPass.QuestionMetadatas.Count == 0)
            {
                MessageBox.Show("Схоже, всі запитання тесту було видалено", "Помилка проходження тесту",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        private void PromptUserToConfirmStart(Test testToPass)
        {
            string generalInfoForMessageBox = $"Тест \"{testToPass.TestMetadata.testTitle}\"" +
                $"\nКількість запитань: {testToPass.QuestionMetadatas.Count}\n";

            generalInfoForMessageBox = IsTimeUnlimited() ?
                string.Concat(generalInfoForMessageBox, "Час проходження необмежений")
                : string.Concat(generalInfoForMessageBox, $"Часу на проходження: " +
                $"{testToPass.TestMetadata.timerValueInMinutes} хв");

            generalInfoForMessageBox = string.Concat(generalInfoForMessageBox,
                "\nДеякі запитання можуть мати декілька правильних варіантів відповідей" +
                "\nВи розпочнете спробу проходження тесту, коли закриєте це вікно");
            MessageBox.Show(generalInfoForMessageBox, "Інформація про тест");
        }

        private bool IsTimeUnlimited()
        {
            return timeLimitInSeconds == 0;
        }

        private void DisplayFollowingQuestion()
        {
            ClearElementsData();
            DisplayVariantsAndUpdateUI(testToPass.QuestionMetadatas[currentQuestionIndex]);
            currentQuestionIndex++;
        }

        private void ClearElementsData()
        {
            QuestionText.Text = string.Empty;
            wrapPanelOfVariants.Children.Clear();
            variants.Clear();
        }

        public void DisplayVariantsAndUpdateUI(TestStructs.QuestionMetadata questionMetadata)
        {
            QuestionText.Text = questionMetadata.question;
            int tempIndexOfCorrectVariant = 0;
            foreach (string variant in questionMetadata.variants)
            {
                bool variantIsCorrect = questionMetadata.correctVariantsIndeces.Contains(tempIndexOfCorrectVariant);
                AddNewVariant(variant, variantIsCorrect);
                tempIndexOfCorrectVariant++;
            }

            UpdateUI();
        }

        private void UpdateUI()
        {
            UpdateCurrentQuestionText();
            UpdateButtonsVisibility();
            UpdateImageAppearance();
        }

        private void UpdateCurrentQuestionText()
        {
            CurrentQuestion_Text.Text = $"{GetOrginalQuestionNumber()}/{testToPass.QuestionMetadatas.Count}";
        }

        private int GetOrginalQuestionNumber()
        {
            return currentQuestionIndex + 1;
        }

        private void UpdateButtonsVisibility()
        {
            if (GetOrginalQuestionNumber() == testToPass.QuestionMetadatas.Count)
            {
                NextQuestion_Button.Visibility = Visibility.Collapsed;
                EndTest_Button.Visibility = Visibility.Visible;
            }
            else NextQuestion_Button.Visibility = Visibility.Visible;
        }

        private void UpdateImageAppearance()
        {
            if (IsLinkedImageDefault(testToPass.QuestionMetadatas[currentQuestionIndex]))
            {
                QuestionText.HorizontalAlignment = HorizontalAlignment.Center;
                ViewboxWithImage.Visibility = Visibility.Collapsed;
                IllustrationImage.Visibility = Visibility.Collapsed;
            }
            else
            {
                string pathToCurrentImage = testToPass.QuestionMetadatas[currentQuestionIndex]
                    .linkedImagePath;
                try
                {
                    IllustrationImage.Source = GetBitmapImageByPath(pathToCurrentImage);
                }
                catch
                {
                    MessageBox.Show("Вказаної раніше картинки не існує або її було переміщено",
                        "Помилка завантаження картинки"
                        , MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                QuestionText.HorizontalAlignment = HorizontalAlignment.Left;
                ViewboxWithImage.Visibility = Visibility.Visible;
                IllustrationImage.Visibility = Visibility.Visible;
            }
        }

        private void AddNewVariant(string variantText, bool isVariantCorrect)
        {
            Button button = SampleGuiElementsFactory.MakeVariantButton(variantText);
            button.Click += VariantButton_Click;
            variants.Add(button, isVariantCorrect);
            wrapPanelOfVariants.Children.Add(button);
        }

        private void SetUpTimer()
        {
            if (IsTimeUnlimited())
            {
                Timer_TextBlock.Text = string.Empty;
                Timer_TextBlock.Visibility = Visibility.Collapsed;
                return;
            }

            UpdateTimerText();
            timer.Interval = 1000;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        private void UpdateTimerText()
        {
            Timer_TextBlock.Text = $"{timeLimitInSeconds / 60}:{timeLimitInSeconds % 60}";
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            timeLimitInSeconds--;

            if(timeLimitInSeconds == 0)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    TryToFinishTest($"Час вичерпано!");
                });
            }
            Timer_TextBlock.Dispatcher.Invoke(() =>
            {
                UpdateTimerText();
            }
            );
        }

        private void StopTimer()
        {
            timer.Stop();
        }

        private void BackToMain_Button_Click(object sender, RoutedEventArgs e)
        {
            PromptMainWindowRedirectionConfirmation();
        }

        private void PromptMainWindowRedirectionConfirmation()
        {
            if (IsMainWindowRedirectionConfirmed())
            {
                StopTimer();
                GoToMainWindow();
            }
        }

        private bool IsMainWindowRedirectionConfirmed()
        {
            string confirmationString = "Натиснувши \"Так\", ви перейдете на головну сторінку, втративши дані" +
                " проходження тесту. Ви справді хочете це зробити?";
            MessageBoxResult result = MessageBox.Show(confirmationString,
                "Підтвердження переходу на головну сторінку", MessageBoxButton.YesNo, MessageBoxImage.Question);
            return result.Equals(MessageBoxResult.Yes);
        }

        private void GoToMainWindow()
        {
            WindowCaller.ShowMain();
            this.CloseWindowAndDisableConfirmationPrompt(ref isWindowClosingConfirmationRequired);
        }

        private void NextQuestion_Button_Click(object sender, RoutedEventArgs e)
        {
            GoToNextQuestion();
        }

        private void GoToNextQuestion()
        {
            if (!isCurrentVariantChosen)
            {
                MessageBox.Show("Оберіть варіант відповіді, який вважаєте правильним");
                return;
            }

            if (testToPass.QuestionMetadatas.Count > currentQuestionIndex)
            {
                ResetAbilityToChooseVariant();
                DisplayFollowingQuestion();
            }
        }

        private void ResetAbilityToChooseVariant()
        {
            isCurrentVariantChosen = false;
        }

        private void EndTest_Button_Click(object sender, RoutedEventArgs e)
        {
            TryToFinishTest();
        }

        private void VariantButton_Click(object sender, RoutedEventArgs e)
        {
            if (isCurrentVariantChosen)
            {
                return;
            }

            isCurrentVariantChosen = true;

            if (IsClickedVariantCorrect((Button)sender))
            {
                correctAnswersCount++;
            }

            MakeKeyboardNavigationPossible();
            ColorMarkVariants();
        }

        private bool IsClickedVariantCorrect(Button clickedVariantButton)
        {
            string clickedVariant = clickedVariantButton.Content.ToString();
            foreach (var buttonBoolPair in variants)
            {
                string currentVariant = buttonBoolPair.Key.Content.ToString();
                if (currentVariant.Equals(clickedVariant))
                {
                    return buttonBoolPair.Value;
                }
            }

            return false;
        }

        private void MakeKeyboardNavigationPossible()
        {
            if (CurrentQuestionIsLast())
            {
                EndTest_Button.Focus();
            }
            else
            {
                NextQuestion_Button.Focus();
            }
        }

        private bool CurrentQuestionIsLast()
        {
            return GetOrginalQuestionNumber() == testToPass.QuestionMetadatas.Count;
        }

        private void ColorMarkVariants()
        {
            foreach (var currVariant in variants)
            {
                bool variantIsCorrect = currVariant.Value;
                if (variantIsCorrect)
                {
                    currVariant.Key.Background = ColorBrushes.DarkGreen;
                    currVariant.Key.Foreground = ColorBrushes.White;
                }
                else
                {
                    currVariant.Key.Background = ColorBrushes.DarkRed;
                    currVariant.Key.Foreground = ColorBrushes.White;
                }
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            e.OpenHelpCenterOnF1();
            if (e.Key == Key.Escape)
            {
                PromptMainWindowRedirectionConfirmation();
            }

            if(e.Key == Key.Enter)
            {
                if (CurrentQuestionIsLast())
                {
                    TryToFinishTest();
                    return;
                }

                GoToNextQuestion();
            }
        }

        private void TryToFinishTest(string customDescription = "")
        {
            if (!isCurrentVariantChosen)
            {
                MessageBox.Show("Оберіть варіант відповіді, який вважаєте правильним");
                return;
            }

            StopTimer();

            string testEndingDescription = GetTestEndingDescription(customDescription);
            string resultsOfTest = GetResultsOfTest();
            MessageBox.Show($"{testEndingDescription}" +
                $"\nРезультати тестування:\n{resultsOfTest}", "Результати проходження тесту");

            SaveResults(resultsOfTest);
            GoToMainWindow();
        }

        private string GetTestEndingDescription(string customDescription)
        {
            string defaultDescription = $"Тест \"{testToPass.TestMetadata.testTitle}\" успішно пройдено!";
            return IsCustomDescriptionAssigned(customDescription) ? customDescription
                : defaultDescription;
        }

        private static bool IsCustomDescriptionAssigned(string customDescription)
        {
            return !string.IsNullOrEmpty(customDescription);
        }

        private string GetResultsOfTest()
        {
            string resultsToReturn = $"{testPasserUserName}: {correctAnswersCount}/{testToPass.QuestionMetadatas.Count}";
            return resultsToReturn;
        }

        private void SaveResults(string currentResult)
        {
            FileWriter fileWriter = new FileWriter();
            fileWriter.AppendNewTestPassingData(testToPass.TestMetadata, currentResult);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (isWindowClosingConfirmationRequired)
            {
                e.GetClosingConfirmation("Дані проходження тесту буде втрачено! ");
            }
        }
    }
}
