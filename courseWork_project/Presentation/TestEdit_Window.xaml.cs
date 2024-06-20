using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using System.Linq;
using courseWork_project.DatabaseRelated;
using courseWork_project.GuiManipulation;
using courseWork_project.Common.StaticResources;

namespace courseWork_project
{
    /// <summary>
    /// Interaction logic for TestEdit_Window.xaml
    /// </summary>
    /// <remarks> TestEdit_Window.xaml is used for creating/editing test's QuestionMetadatas</remarks>
    public partial class TestEdit_Window : Window
    {
        private int currentQuestionIndex = 0;
        private int totalQuestionsCount = 1;
        private readonly bool isCreatingMode;
        private readonly List<TestStructs.QuestionMetadata> questionMetadatas;
        private readonly TestStructs.TestMetadata testMetadata;
        private readonly Dictionary<TextBox, CheckBox> variantComponents = new Dictionary<TextBox, CheckBox>();
        private string currentImagePath = ImageManager.DefaultPath;
        bool isWindowClosingConfirmationRequired = true;

        public TestEdit_Window()
        {
            InitializeComponent();
            questionMetadatas = new List<TestStructs.QuestionMetadata>();
            isCreatingMode = true;
            UpdateUI();
        }
        /// <summary>
        /// Editing mode constructor
        /// </summary>
        /// <param name="indexOfQuestionToEdit">Index of question to edit (0-9)</param>
        public TestEdit_Window(Test testToChange, int indexOfQuestionToEdit)
        {
            isCreatingMode = false;
            questionMetadatas = testToChange.QuestionMetadatas;
            testMetadata = testToChange.TestMetadata;
            currentQuestionIndex = indexOfQuestionToEdit;
            InitializeComponent();

            ShowCurrentQuestion();
        }

        private void ShowCurrentQuestion()
        {
            totalQuestionsCount = questionMetadatas.Count;
            EraseInputElementsData();

            TestStructs.QuestionMetadata currentQuestion =
                questionMetadatas[currentQuestionIndex];
            DisplayQuestion(currentQuestion);
            UpdateUI();
        }

        private void EraseInputElementsData()
        {
            QuestionInput.Text = string.Empty;
            variantsPanel.Children.Clear();
            variantComponents.Clear();
        }

        public void DisplayQuestion(TestStructs.QuestionMetadata currentQuestion)
        {
            QuestionInput.Text = currentQuestion.question;
            DisplayVariantsOfQuestion(currentQuestion);
            ColormarkVariants();
            currentImagePath = currentQuestion.linkedImagePath;
        }

        private void DisplayVariantsOfQuestion(TestStructs.QuestionMetadata currentQuestion)
        {
            int tempIndexOfCorrectVariant = 0;
            foreach (string variant in currentQuestion.variants)
            {
                bool variantIsCorrect = currentQuestion.correctVariantsIndeces
                    .Contains(tempIndexOfCorrectVariant);
                TryAddNewVariant(variant, variantIsCorrect);
                tempIndexOfCorrectVariant++;
            }
        }

        private void TryAddNewVariant(string variantText = "Введіть варіант відповіді", bool isVariantCorrect = false)
        {
            if (IsVariantsLimitReached())
            {
                return;
            }

            TextBox textBox = UiElementsFactory.MakeVariantTextbox(variantText);
            CheckBox checkBox = UiElementsFactory.MakeVariantCheckbox(isVariantCorrect);
            HangCheckingEvents(checkBox);

            variantComponents.Add(textBox, checkBox);
            DockPanel dockPanel = UiElementsFactory.MakeVariantDockpanel(textBox, checkBox);
            variantsPanel.Children.Add(dockPanel);
        }

        private bool IsVariantsLimitReached()
        {
            return variantsPanel.Children.Count == Properties.Settings.Default.variantsLimit;
        }

        private void HangCheckingEvents(CheckBox checkBox)
        {
            checkBox.Unchecked += CheckBox_Updated;
            checkBox.Checked += CheckBox_Updated;
        }

        private void CheckBox_Updated(object sender, RoutedEventArgs e)
        {
            ColormarkVariants();
        }

        private void ColormarkVariants()
        {
            if (variantComponents.Count == 0)
            {
                MessageBox.Show($"Немає варіантів відповідей. Створіть їх," +
                    $" використовуючи кнопку \"{AddVariant_Button.Content}\"");
                return;
            }

            foreach (var currVariant in variantComponents)
            {
                if ((bool)currVariant.Value.IsChecked)
                {
                    currVariant.Key.Background = ColorBrushes.DarkGreen;
                    currVariant.Key.Foreground = ColorBrushes.White;
                }
                else
                {
                    currVariant.Key.Background = ColorBrushes.VariantBackground;
                    currVariant.Key.Foreground = ColorBrushes.Black;
                }
            }
        }

        private void UpdateUI()
        {
            UpdateCurrentQuestionText();
            UpdateNavigationButtonsVisibility();
            UpdateVariantButtonsVisibility();
            UpdateImageAppearance();
        }

        private void UpdateCurrentQuestionText()
        {
            CurrentQuestion_Text.Text = $"{GetOrginalQuestionNumber()}/{totalQuestionsCount}";
        }

        private int GetOrginalQuestionNumber()
        {
            return currentQuestionIndex + 1;
        }

        private void UpdateNavigationButtonsVisibility()
        {
            if (GetOrginalQuestionNumber() == 1)
            {
                PrevQuestion_Button.Visibility = Visibility.Collapsed;
                return;
            }

            if (IsQuestionLimitReached())
            {
                NextQuestion_Button.Visibility = Visibility.Collapsed;
                return;
            }

            PrevQuestion_Button.Visibility = Visibility.Visible;
            NextQuestion_Button.Visibility = Visibility.Visible;
        }

        private bool IsQuestionLimitReached()
        {
            return GetOrginalQuestionNumber() == Properties.Settings.Default.questionsLimit;
        }

        private void UpdateVariantButtonsVisibility()
        {
            if (VariantPanelIsEmpty())
            {
                RemoveLastVariant_Button.Visibility = Visibility.Collapsed;
                return;
            }

            if (IsVariantsLimitReached())
            {
                AddVariant_Button.Visibility = Visibility.Collapsed;
                return;
            }

            AddVariant_Button.Visibility = Visibility.Visible;
            RemoveLastVariant_Button.Visibility = Visibility.Visible;
        }

        private bool VariantPanelIsEmpty()
        {
            return variantsPanel.Children.Count == 0;
        }

        private void UpdateImageAppearance()
        {
            bool imageIsNotSet = currentImagePath.Equals(ImageManager.DefaultPath);
            if (imageIsNotSet)
            {
                ResetImageSourcesToDefault();
                return;
            }

            DisplayImageFromPath(currentImagePath);
        }

        private void ResetImageSourcesToDefault()
        {
            currentImagePath = ImageManager.DefaultPath;
            IllustrationImage.Source = ImageManager.DefaultBitmapImage();
        }

        private void BackToMain_Button_Click(object sender, RoutedEventArgs e)
        {
            if (!TryUpdateCurrentQuestionData())
            {
                return;
            }

            TryRedirectToMainWindow();
        }

        private bool TryUpdateCurrentQuestionData()
        {
            List<int> correctVariantsIndeces = GetCurrentCorrectVariantsIndeces();
            if (!QuestionDataInputIsValid(correctVariantsIndeces))
            {
                return false;
            }

            TestStructs.QuestionMetadata currQuestion = new TestStructs.QuestionMetadata()
            {
                question = QuestionInput.Text.Trim(),
                variants = GetCurrentVariants(),
                correctVariantsIndeces = correctVariantsIndeces,
                linkedImagePath = currentImagePath
            };

            if (CurrentQuestionExistsInList())
            {
                ReplaceCurrentQuestionBy(currQuestion);
            }
            else
            {
                questionMetadatas.Add(currQuestion);
            }

            return true;
        }

        private bool QuestionDataInputIsValid(List<int> correctVariantsIndeces)
        {
            if (!AreAllTextboxesFilled())
            {
                MessageBoxes.ShowWarning("Будь ласка, заповніть всі потрібні поля");
                return false;
            }

            if (variantsPanel.Children.Count < 2)
            {
                MessageBoxes.ShowWarning("Необхідно заповнити як мінімум 2 варіанти");
                return false;
            }

            if (correctVariantsIndeces.Count == 0)
            {
                MessageBoxes.ShowWarning("Позначте хоча б один варіант відповіді як правильний");
                return false;
            }

            if (!IsVariantsInputValid())
            {
                MessageBoxes.ShowWarning("Використовуйте не тільки цифри для заповнення варіантів");
                return false;
            }

            return true;
        }

        private List<string> GetCurrentVariants()
        {
            List<string> variants = new List<string>();
            foreach (var pairOfTextBoxAndCheckBox in variantComponents)
            {
                variants.Add(pairOfTextBoxAndCheckBox.Key.Text.Trim());
            }

            return variants;
        }

        private List<int> GetCurrentCorrectVariantsIndeces()
        {
            List<int> correctIndeces = new List<int>();
            var currentVariants = variantComponents.ToList();
            for (int i = 0; i < currentVariants.Count; i++)
            {
                if ((bool)currentVariants[i].Value.IsChecked)
                {
                    correctIndeces.Add(i);
                }
            }

            return correctIndeces;
        }

        private bool CurrentQuestionExistsInList()
        {
            return questionMetadatas.Count > currentQuestionIndex;
        }

        private void ReplaceCurrentQuestionBy(TestStructs.QuestionMetadata newQuestion)
        {
            questionMetadatas.Insert(currentQuestionIndex, newQuestion);
            questionMetadatas.RemoveAt(currentQuestionIndex + 1);
        }

        private bool AreAllTextboxesFilled()
        {
            foreach (var textBoxRelatedPair in variantComponents)
            {
                string variantValue = textBoxRelatedPair.Key.Text;
                bool variantIsNotFilled = string.IsNullOrWhiteSpace(variantValue)
                    || variantValue.Equals("Введіть варіант відповіді");
                if (variantIsNotFilled)
                {
                    return false;
                }
            }

            bool questionInputIsFilled = !string.IsNullOrWhiteSpace(QuestionInput.Text);
            return questionInputIsFilled;
        }

        private bool IsVariantsInputValid()
        {
            string containsNonDigitRegex = @"[^0-9]";
            foreach (var variantPair in variantComponents)
            {
                string variantValue = variantPair.Key.Text;
                if (!Regex.IsMatch(variantValue, containsNonDigitRegex))
                {
                    return false;
                }
            }

            return true;
        }

        private void TryRedirectToMainWindow()
        {
            if(IsMainWindowRedirectionConfirmed())
            {
                EraseCurrentTestData();

                WindowCaller.ShowMain();
                this.CloseWindowAndDisableConfirmationPrompt(ref isWindowClosingConfirmationRequired);
            }
        }

        private static bool IsMainWindowRedirectionConfirmed()
        {
            MessageBoxResult result = MessageBoxes.ShowConfirmationPrompt("Натиснувши \"Так\", " +
                "ви перейдете на головну сторінку, втративши всі дані цього тесту. " +
                "Ви справді хочете це зробити?",
                "Підтвердження переходу на головну сторінку");
            return result.Equals(MessageBoxResult.Yes);
        }

        private void EraseCurrentTestData()
        {
            DataEraser.EraseTestDatabases(testMetadata);
        }

        private void NextQuestion_Button_Click(object sender, RoutedEventArgs e)
        {
            TryGoToNextQuestion();
        }

        private void TryGoToNextQuestion()
        {
            if (!TryUpdateCurrentQuestionData())
            {
                return;
            }

            if (questionMetadatas.Count > GetOrginalQuestionNumber())
            {
                currentQuestionIndex++;
                ShowCurrentQuestion();
                return;
            }

            if (GetOrginalQuestionNumber() == totalQuestionsCount)
            {
                totalQuestionsCount++;
                currentQuestionIndex++;
                EraseInputElementsData();
                UpdateUI();
                ResetImageSourcesToDefault();
            }
        }

        private void PrevQuestion_Button_Click(object sender, RoutedEventArgs e)
        {
            TryGoToPreviousQuestion();
        }

        private void TryGoToPreviousQuestion()
        {
            if (!TryUpdateCurrentQuestionData())
            {
                return;
            }

            currentQuestionIndex--;
            ShowCurrentQuestion();
        }

        private void AddVariant_Button_Click(object sender, RoutedEventArgs e)
        {
            TryAddNewVariant();
            UpdateUI();
        }

        private void RemoveLastVariant_Button_Click(object sender, RoutedEventArgs e)
        {
            RemoveLastVariant();
            UpdateUI();
        }

        private void RemoveLastVariant()
        {
            variantsPanel.Children.RemoveAt(variantsPanel.Children.Count - 1);
            variantComponents.Remove(variantComponents.Keys.Last());
        }

        private void SaveTest_Button_Click(object sender, RoutedEventArgs e)
        {
            TryGoToSavingWindow();
        }

        private void TryGoToSavingWindow()
        {
            if (!TryUpdateCurrentQuestionData())
            {
                return;
            }

            ShowTestSavingWindow();
            this.CloseWindowAndDisableConfirmationPrompt(ref isWindowClosingConfirmationRequired);
        }

        private void ShowTestSavingWindow()
        {
            if (isCreatingMode)
            {
                WindowCaller.ShowTestSavingCreatingMode(questionMetadatas);
            }
            else
            {
                Test testToSave = new Test(testMetadata, questionMetadatas);
                WindowCaller.ShowTestSavingEditingMode(testToSave);
            }
        }

        /// <summary>
        /// Handling pressed keyboard keys
        /// </summary>
        /// <remarks>F1 - user manual;
        /// Esc - previous question/window;
        /// Enter - next question/test saving window</remarks>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            e.OpenHelpCenterOnF1();
            if (e.Key == Key.Escape)
            {
                if(GetOrginalQuestionNumber() == 1)
                {
                    TryRedirectToMainWindow();
                    return;
                }

                TryGoToPreviousQuestion();
            }

            if(e.Key == Key.Enter)
            {
                if (IsQuestionLimitReached())
                {
                    TryGoToSavingWindow();
                    return;
                }

                TryGoToNextQuestion();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (isWindowClosingConfirmationRequired
                && e.TryGetClosingConfirmation("Ви втратите всі дані цього тесту. "))
            {
                EraseCurrentTestData();
            }
        }

        private void ImageChange_Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog
            {
                Filter = "Image Files (*.jpg, *.jpeg, *.png, *.bmp)|*.jpg;*.jpeg;*.png;*.bmp)"
            };

            bool fileIsChosen = (bool)fileDialog.ShowDialog();
            if (fileIsChosen)
            {
                currentImagePath = fileDialog.FileName;
                DisplayImageFromPath(currentImagePath);
            }
        }

        private void DisplayImageFromPath(string path)
        {
            try
            {
                IllustrationImage.Source = ImageManager.GetBitmapImageByPath(path);
            }
            catch
            {
                MessageBoxes.ShowError("Вказаної раніше картинки не існує або її було переміщено");
                ResetImageSourcesToDefault();
                if (CurrentQuestionExistsInList())
                {
                    TestStructs.QuestionMetadata changedQuestionMetadata = 
                        questionMetadatas[currentQuestionIndex];
                    changedQuestionMetadata.linkedImagePath = ImageManager.DefaultPath;
                    ReplaceCurrentQuestionBy(changedQuestionMetadata);
                }
            }
        }

        private void ImageDeletion_Button_Click(object sender, RoutedEventArgs e)
        {
            ResetImageSourcesToDefault();
        }
    }
}
