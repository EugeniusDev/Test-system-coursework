using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.ObjectModel;
using static courseWork_project.ImageManager;
using courseWork_project.DatabaseRelated;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace courseWork_project
{
    /// <summary>
    /// Class for manipulation with ObservableCollection of QuestionMetadatas
    /// </summary>
    public class QuestionItem
    {
        public string Question { get; set; }
    }
    /// <summary>
    /// Interaction logic for TestSaving_Window.xaml.
    /// </summary>
    /// <remarks>TestSaving_Window is used to save created/edited test into database</remarks>
    public partial class TestSaving_Window : Window
    {
        private readonly List<TestStructs.QuestionMetadata> questionMetadatas;
        private List<ImageManager.ImageMetadata> imageMetadatas;
        private TestStructs.TestMetadata testMetadata = TestStructs.EmptyTestMetadata;
        private readonly string transliterOldTestTitle = string.Empty;

        private ObservableCollection<QuestionItem> questionItems;

        private readonly bool isCreatingMode;
        bool isWindowClosingConfirmationRequired = true;

        /// <summary>
        /// TestSaving_Window creating mode constructor
        /// </summary>
        /// <param name="questionsToSave">List of QuestionMetadatas of the test</param>
        /// <param name="imagesToSave">List of data about images</param>
        public TestSaving_Window(List<TestStructs.QuestionMetadata> questionsToSave, List<ImageManager.ImageMetadata> imagesToSave)
        {
            isCreatingMode = true;
            this.questionMetadatas = questionsToSave;
            imageMetadatas = imagesToSave;

            InitializeComponent();

            SetUpQuestionAndTimerUI();
            DisplayQuestionsFromMetadatas(questionsToSave);
        }

        private void SetUpQuestionAndTimerUI()
        {
            questionItems = new ObservableCollection<QuestionItem>();
            QuestionsListView.ItemsSource = questionItems;
            TimerInputBox.Text = testMetadata.timerValueInMinutes.ToString();
        }

        public void DisplayQuestionsFromMetadatas(List<TestStructs.QuestionMetadata> questionMetadatas)
        {
            foreach (TestStructs.QuestionMetadata metadata in questionMetadatas)
            {
                AddNewQuestionsListViewRow(metadata.question);
            }
        }

        private void AddNewQuestionsListViewRow(string questionText)
        {
            QuestionItem newItem = new QuestionItem
            {
                Question = questionText
            };

            questionItems.Add(newItem);
        }

        /// <summary>
        /// TestSaving_Window editing mode constructor
        /// </summary>
        public TestSaving_Window(Test testToSave, List<ImageManager.ImageMetadata> imagesToSave)
        {
            isCreatingMode = false;
            questionMetadatas = testToSave.QuestionMetadatas;
            testMetadata = testToSave.TestMetadata;
            transliterOldTestTitle = DataDecoder.TransliterateToEnglish(testMetadata.testTitle);
            imageMetadatas = imagesToSave;

            InitializeComponent();

            SetUpQuestionAndTimerUI();
            UpdateTestTitleUI(testMetadata.testTitle);
            DisplayQuestionsFromMetadatas(questionMetadatas);
        }

        private void UpdateTestTitleUI(string newText = "")
        {
            TestTitleBox.Foreground = new SolidColorBrush(Colors.Black);
            TestTitleBox.Text = newText;
        }

        private void TestTitleBlock_GotFocus(object sender, RoutedEventArgs e)
        {
            if (IsTestTitleUiDefault() || testMetadata.testTitle == string.Empty)
            {
                UpdateTestTitleUI();
            }
        }

        private bool IsTestTitleUiDefault()
        {
            return TestTitleBox.Text.Equals("Введіть назву тесту");
        }

        private void TestTitleBlock_LostFocus(object sender, RoutedEventArgs e)
        {
            if (IsTextBoxEmpty(TestTitleBox))
            {
                TestTitleBox.Text = "Введіть назву тесту";
                TestTitleBox.Foreground = new SolidColorBrush(Colors.DarkGray);
            }
        }

        private bool IsTextBoxEmpty(TextBox textBox)
        {
            return string.IsNullOrWhiteSpace(textBox.Text);
        }

        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            SaveDataAndGoToMain();
        }

        private void SaveDataAndGoToMain()
        {
            if (!TryParseInputToMetadata())
            {
                return;
            }

            Test testToSave = new Test(questionMetadatas, testMetadata);
            EncodeAndSaveTest(testToSave);
            AppendNewTestToListOfExisting(testToSave);
            CopyImagesToDatabaseDirectory(testToSave);
            UpdateTitleRelatedDataIfChanged();

            MessageBox.Show("Тест успішно збережено");
            GoToMainWindow();
        }

        private bool TryParseInputToMetadata()
        {
            bool titleBlockIsNotSet = IsTextBoxEmpty(TestTitleBox) || IsTestTitleUiDefault();
            bool timerIsEmpty = IsTextBoxEmpty(TimerInputBox);
            if (titleBlockIsNotSet || timerIsEmpty)
            {
                MessageBox.Show("Будь ласка, заповніть всі потрібні поля");
                return false;
            }

            if (TryParseValidTimerValue(out int timerValue))
            {
                testMetadata.timerValueInMinutes = timerValue;
            }

            testMetadata.lastEditedTime = DateTime.Now;
            testMetadata.testTitle = TestTitleBox.Text;
            return true;
        }

        private static void EncodeAndSaveTest(Test testToSave)
        {
            List<string> testDataLines = testToSave.EncodeToLines();
            FileWriter fileWriter = new FileWriter(testToSave.TestMetadata.testTitle);
            fileWriter.WriteListLineByLine(testDataLines);
        }

        private static void AppendNewTestToListOfExisting(Test test)
        {
            string transliteratedTestTitle = DataDecoder.TransliterateToEnglish(test.TestMetadata.testTitle);
            FileWriter fileWriter = new FileWriter();
            fileWriter.AppendLineToFile(transliteratedTestTitle);
        }

        private void CopyImagesToDatabaseDirectory(Test testToSave)
        {
            foreach (ImageManager.ImageMetadata imageMetadata in imageMetadatas)
            {
                if (!IsImageInCorrectPlace(imageMetadata, testToSave))
                {
                    string transliteratedTestTitle = testToSave.TestMetadata.testTitle.TransliterateToEnglish();
                    imageMetadata.CopyToDatabaseDirectoryWithNameOf(transliteratedTestTitle);
                }
            }
        }

        private void GoToMainWindow()
        {
            WindowCaller.ShowMain();
            this.CloseWindowAndDisableConfirmationPrompt(ref isWindowClosingConfirmationRequired);
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (!TryParseInputToMetadata())
            {
                return;
            }

            QuestionItem questionItemToEdit = new QuestionItem();
            if (GuiObjectsFinder.TryGetQuestionItemFromValidAncestor(sender, ref questionItemToEdit))
            {
                int indexOfElementToEdit = GetIndexOfChosenQuestion(questionItemToEdit);
                if (QuestionIndexIsValid(indexOfElementToEdit))
                {
                    UpdateTitleRelatedDataIfChanged();
                    EditTestOnQuestionAtIndex(indexOfElementToEdit);
                }
            }
        }

        private int GetIndexOfChosenQuestion(QuestionItem questionItemToEdit)
        {
            for (int i = 0; i < questionMetadatas.Count; i++)
            {
                if (questionMetadatas[i].question.Equals(questionItemToEdit.Question))
                {
                    return i;
                }
            }

            return -1;
        }

        private static bool QuestionIndexIsValid(int indexOfQuestion)
        {
            if (indexOfQuestion == -1)
            {
                MessageBox.Show("Обране запитання не знайдено", "Помилка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        private void EditTestOnQuestionAtIndex(int indexOfElementToEdit)
        {
            Test testToEdit = new Test(questionMetadatas, testMetadata);
            WindowCaller.ShowTestChangeEditingMode(testToEdit, imageMetadatas, indexOfElementToEdit);
            this.CloseWindowAndDisableConfirmationPrompt(ref isWindowClosingConfirmationRequired);
        }

        // TODO remake image manipulation entirely and remake this method
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            QuestionItem questionItemToDelete = new QuestionItem();
            if (GuiObjectsFinder.TryGetQuestionItemFromValidAncestor(sender, ref questionItemToDelete))
            {
                if (imageMetadatas.Count == 0)
                {
                    Test test = new Test(questionMetadatas, testMetadata);
                    imageMetadatas = test.GetRelatedImages();
                }

                for (int i  = 0; i < questionMetadatas.Count; i++)
                {
                    // TODO GetIndexOfChosenQuestion here instead
                    if (string.Compare(questionMetadatas[i].question, questionItemToDelete.Question) == 0)
                    {
                        questionMetadatas.RemoveAt(i);
                        // TODO get rid of this complicated image stuff and replace it with list of images in Test
                        ImageManager.ImageMetadata imageToDelete = imageMetadatas.Find(x => x.questionIndex == i);
                        if (!imageToDelete.Equals(default(ImageManager.ImageMetadata)))
                        {
                            imageMetadatas.Remove(imageToDelete);
                            if (!isCreatingMode)
                            {
                                TryDeleteImage(imageToDelete);
                            }
                        }
                        break;
                    }
                }

                questionItems.Remove(questionItemToDelete);
            }
        }
        /// <summary>
        /// Handling pressed keyboard keys
        /// </summary>
        /// <remarks>F1 - user manual;
        /// Esc - previous window;
        /// Enter - saving the test</remarks>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            e.OpenHelpCenterOnF1();
            if (e.Key == Key.Escape)
            {
                if (!TryParseInputToMetadata())
                {
                    return;
                }

                UpdateTitleRelatedDataIfChanged();
                EditTestOnQuestionAtIndex(GetLastQuestionIndex());
            }
            if (e.Key == Key.Enter)
            {
                SaveDataAndGoToMain();
            }
        }

        private int GetLastQuestionIndex()
        {
            return questionMetadatas.Count - 1;
        }

        /// <summary>
        /// Changes test title in paths of images and resets(deletes) old test passing data
        /// </summary>
        public void UpdateTitleRelatedDataIfChanged()
        {
            string supposedNewTransliterTestTitle = DataDecoder.TransliterateToEnglish(testMetadata.testTitle);
            bool titleChanged = string.Compare(transliterOldTestTitle, 
                supposedNewTransliterTestTitle) != 0
                && transliterOldTestTitle != string.Empty;
            if (titleChanged)
            {
                for(int i = 0; i < imageMetadatas.Count; i++)
                {
                    if (imageMetadatas[i].path.Contains(transliterOldTestTitle))
                    {
                        ImageManager.ImageMetadata deprecatedImageMetadata = imageMetadatas[i];
                        imageMetadatas[i] = new ImageManager.ImageMetadata()
                        {
                            path = deprecatedImageMetadata.path.Replace(transliterOldTestTitle, supposedNewTransliterTestTitle),
                            questionIndex = deprecatedImageMetadata.questionIndex
                        };
                        deprecatedImageMetadata.CopyToDatabaseDirectoryWithNameOf(supposedNewTransliterTestTitle);
                        TryDeleteImage(deprecatedImageMetadata);
                    }
                }

                DataEraser.EraseTestPassingDataByTitle(transliterOldTestTitle);
            }
        }

        private bool TryParseValidTimerValue(out int timerValueInMinutes)
        {
            bool timerInputIsNumerical = int.TryParse(TimerInputBox.Text, out timerValueInMinutes);
            if (timerInputIsNumerical)
            {
                bool timerValueIsCorrect = int.Parse(TimerInputBox.Text) >= 0;
                if (timerValueIsCorrect)
                {
                    return true;
                }
            }

            MessageBox.Show("Некоректний ввід у поле таймера. Необхідно ввести число >= 0");
            return true;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (isWindowClosingConfirmationRequired)
            {
                string additionalMessage = "Дані тесту буде втрачено. ";

                if (e.GetClosingConfirmation(additionalMessage))
                {
                    DataEraser.EraseTestDatabases(testMetadata);
                    if (!isCreatingMode)
                    {
                        TryDeleteImages(imageMetadatas);
                    }
                }
            }
        }
    }
}
