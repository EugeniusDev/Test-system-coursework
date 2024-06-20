using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Collections.ObjectModel;
using courseWork_project.DatabaseRelated;
using System.Windows.Controls;
using courseWork_project.Common.StaticResources;

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
    /// Interaction logic for TestSave_Window.xaml.
    /// </summary>
    /// <remarks>TestSave_Window is used to save created/edited test into database</remarks>
    public partial class TestSave_Window : Window
    {
        private const string defaultTestTitle = "Введіть назву тесту";
        private readonly string transliterOldTestTitle = string.Empty;
        private TestStructs.TestMetadata testMetadata = TestStructs.EmptyTestMetadata;
        private readonly List<TestStructs.QuestionMetadata> questionMetadatas;
        private ObservableCollection<QuestionItem> questionItems;

        bool isWindowClosingConfirmationRequired = true;

        public TestSave_Window(List<TestStructs.QuestionMetadata> questionsToSave)
        {
            questionMetadatas = questionsToSave;

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
        /// In case TestMetadata exists
        /// </summary>
        public TestSave_Window(Test testToSave) : this(testToSave.QuestionMetadatas)
        {
            testMetadata = testToSave.TestMetadata;
            transliterOldTestTitle = DataDecoder.TransliterateToEnglish(testMetadata.testTitle);
            UpdateTestTitleUI(testMetadata.testTitle);
        }

        private void UpdateTestTitleUI(string newText = "")
        {
            TestTitleBox.Foreground = ColorBrushes.Black;
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
            return TestTitleBox.Text.Equals(defaultTestTitle);
        }

        private void TestTitleBlock_LostFocus(object sender, RoutedEventArgs e)
        {
            if (IsTextBoxEmpty(TestTitleBox))
            {
                TestTitleBox.Text = defaultTestTitle;
                TestTitleBox.Foreground = ColorBrushes.DarkGray;
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

            Test testToSave = new Test(testMetadata, questionMetadatas);
            EncodeAndSaveTest(testToSave);
            AppendNewTestToListOfExisting(testToSave);
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
                MessageBoxes.ShowWarning("Будь ласка, заповніть всі потрібні поля");
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
                int questionIndex = GetIndexOfChosenQuestion(questionItemToEdit);
                if (QuestionIndexIsValid(questionIndex))
                {
                    UpdateTitleRelatedDataIfChanged();
                    EditTestOnQuestionAtIndex(questionIndex);
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
                MessageBoxes.ShowError("Обране запитання не знайдено");
                return false;
            }

            return true;
        }

        private void EditTestOnQuestionAtIndex(int indexOfElementToEdit)
        {
            Test testToEdit = new Test(testMetadata, questionMetadatas);
            WindowCaller.ShowTestChangeEditingMode(testToEdit, indexOfElementToEdit);
            this.CloseWindowAndDisableConfirmationPrompt(ref isWindowClosingConfirmationRequired);
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            QuestionItem questionItemToDelete = new QuestionItem();
            if (GuiObjectsFinder.TryGetQuestionItemFromValidAncestor(sender, ref questionItemToDelete))
            {
                int questionIndex = GetIndexOfChosenQuestion(questionItemToDelete);
                if (QuestionIndexIsValid(questionIndex))
                {
                    questionMetadatas.RemoveAt(questionIndex);
                    questionItems.Remove(questionItemToDelete);
                }
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

        public void UpdateTitleRelatedDataIfChanged()
        {
            string supposedNewTransliterTestTitle = DataDecoder.TransliterateToEnglish(testMetadata.testTitle);
            bool titleChanged = string.Compare(transliterOldTestTitle, supposedNewTransliterTestTitle) != 0
                && transliterOldTestTitle != string.Empty;
            if (titleChanged)
            {
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

            MessageBoxes.ShowWarning("Некоректний ввід у поле таймера. Необхідно ввести число >= 0");
            return false;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (isWindowClosingConfirmationRequired 
                && e.TryGetClosingConfirmation("Всі дані цього тесту буде втрачено. "))
            {
                DataEraser.EraseTestDatabases(testMetadata);
            }
        }
    }
}
