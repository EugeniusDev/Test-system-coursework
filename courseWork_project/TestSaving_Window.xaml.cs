using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.ObjectModel;
using static courseWork_project.ImageManager;
using System.Windows.Controls;
using courseWork_project.DatabaseRelated;
using System.Windows.Media.Animation;

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
        /// <summary>
        /// List of QuestionMetadatas of the test
        /// </summary>
        private readonly List<TestStructs.QuestionMetadata> questionsToSave;
        /// <summary>
        /// List of data about images
        /// </summary>
        private List<ImageManager.ImageMetadata> imageMetadatas;
        /// <summary>
        /// Overall test info structure
        /// </summary>
        private TestStructs.TestMetadata testMetadata;
        /// <summary>
        /// Used while updating sources of images
        /// </summary>
        /// <remarks>Contains deprecated transliterated test title</remarks>
        private readonly string transliterOldTestTitle = string.Empty;
        /// <summary>
        /// ObservableCollection for QuestionsListView
        /// </summary>
        private readonly ObservableCollection<QuestionItem> questionItems;
        /// <summary>
        /// Determines the window's mode
        /// </summary>
        /// <remarks>true - creating mode; false - editing mode</remarks>
        private readonly bool isCreatingMode;
        /// <summary>
        /// Used to determine if window closing confirmation is needed
        /// </summary>
        bool askForClosingComfirmation = true;
        /// <summary>
        /// TestSaving_Window creating mode constructor
        /// </summary>
        /// <param name="questionsToSave">List of QuestionMetadatas of the test</param>
        /// <param name="imagesToSave">List of data about images</param>
        public TestSaving_Window(List<TestStructs.QuestionMetadata> questionsToSave, List<ImageManager.ImageMetadata> imagesToSave)
        {
            isCreatingMode = true;
            this.questionsToSave = questionsToSave;
            imageMetadatas = imagesToSave;
            // There are no time limitations by default
            testMetadata.timerValue = 0;
            InitializeComponent();
            TimerInputBox.Text = testMetadata.timerValue.ToString();

            questionItems = new ObservableCollection<QuestionItem>();
            QuestionsListView.ItemsSource = questionItems;
            DisplayQuestionsFromMetadatas(questionsToSave);
        }
        /// <summary>
        /// TestSaving_Window editing mode constructor
        /// </summary>
        public TestSaving_Window(Test testToSave, List<ImageManager.ImageMetadata> imagesToSave)
        {
            isCreatingMode = false;
            questionsToSave = testToSave.QuestionMetadatas;
            testMetadata = testToSave.TestMetadata;
            transliterOldTestTitle = DataDecoder.TransliterateToEnglish(testMetadata.testTitle);
            imageMetadatas = imagesToSave;
            InitializeComponent();
            TimerInputBox.Text = testMetadata.timerValue.ToString();

            TestTitleBlock.Foreground = new SolidColorBrush(Colors.Black);
            TestTitleBlock.Text = testMetadata.testTitle;
            questionItems = new ObservableCollection<QuestionItem>();

            QuestionsListView.ItemsSource = questionItems;
            DisplayQuestionsFromMetadatas(questionsToSave);
        }
        /// <summary>
        /// Handling click on test title
        /// </summary>
        private void TestTitleBlock_GotFocus(object sender, RoutedEventArgs e)
        {
            bool titleContainsDefaultText = TestTitleBlock != null
                && string.Compare(TestTitleBlock.Text, "Введіть назву тесту") == 0;
            if (titleContainsDefaultText || testMetadata.testTitle == null)
            {
                TestTitleBlock.Foreground = new SolidColorBrush(Colors.Black);
                TestTitleBlock.Text = string.Empty;
            }
        }
        /// <summary>
        /// Handling lost focus on test title
        /// </summary>
        private void TestTitleBlock_LostFocus(object sender, RoutedEventArgs e)
        {
            bool fieldIsEmpty = TestTitleBlock != null && string.IsNullOrWhiteSpace(TestTitleBlock.Text);
            if (fieldIsEmpty)
            {
                TestTitleBlock.Text = "Введіть назву тесту";
                TestTitleBlock.Foreground = new SolidColorBrush(Colors.DarkGray);
            }
        }
        /// <summary>
        /// Handling pressed Save_Button
        /// </summary>
        /// <remarks>Calls SaveDataAndGoToMain</remarks>
        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            SaveDataAndGoToMain();
        }
        /// <summary>
        /// Handling pressed button of type EditButton
        /// </summary>
        /// <remarks>Finds selected question and opens TestChange_Window in editing mode</remarks>
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (!InputIsCorrect()) return;

            QuestionItem questionItemToEdit = new QuestionItem();
            if (GuiObjectsFinder.TryGetQuestionItemFromValidAncestor(sender, ref questionItemToEdit))
            {
                int indexOfElementToEdit = 0;
                for (int i = 0; i < questionsToSave.Count; i++)
                {
                    if (string.Compare(questionsToSave[i].question, questionItemToEdit.Question) == 0)
                    {
                        indexOfElementToEdit = i+1;
                        break;
                    }
                }
                testMetadata.testTitle = TestTitleBlock.Text;
                UpdateTitleIfChanged();
                Test testToEdit = new Test(questionsToSave, testMetadata);
                WindowCaller.ShowTestChangeEditingMode(testToEdit, imageMetadatas, indexOfElementToEdit);
                askForClosingComfirmation = false;
                Close();
            }
        }
        /// <summary>
        /// Handling pressed button of type DeleteButton
        /// </summary>
        /// <remarks>Deletes selected question from list and GUI</remarks>
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            QuestionItem questionItemToDelete = new QuestionItem();
            if (GuiObjectsFinder.TryGetQuestionItemFromValidAncestor(sender, ref questionItemToDelete))
            {
                if (imageMetadatas.Count == 0)
                {
                    ImageListFormer imageListFormer = new ImageListFormer();
                    imageMetadatas = imageListFormer.GetImageList(testMetadata.testTitle, questionsToSave);
                }

                for (int i  = 0; i < questionsToSave.Count; i++)
                {
                    if (string.Compare(questionsToSave[i].question, questionItemToDelete.Question) == 0)
                    {
                        questionsToSave.RemoveAt(i);
                        ImageManager.ImageMetadata imageToDelete = imageMetadatas.Find(x => x.questionIndex == i+1);
                        if (!imageToDelete.Equals(default(ImageManager.ImageMetadata)))
                        {
                            imageMetadatas.Remove(imageToDelete);
                            if (!isCreatingMode)
                            {
                                DataEraser.EraseImage(imageToDelete);
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
                if (!InputIsCorrect()) return;

                UpdateTitleIfChanged();
                Test testToEdit = new Test(questionsToSave, testMetadata);
                WindowCaller.ShowTestChangeEditingMode(testToEdit, imageMetadatas, questionsToSave.Count);
                askForClosingComfirmation = false;
                Close();
            }
            if (e.Key == Key.Enter)
            {
                SaveDataAndGoToMain();
            }
        }
        /// <summary>
        /// Creates database, writes data in it and returns to MainWindow
        /// </summary>
        private void SaveDataAndGoToMain()
        {
            if (!InputIsCorrect())
            {
                return;
            }
            // Saving encoded into lines test data to a file-database
            Test testToSave = new Test(questionsToSave, testMetadata);
            List<string> testDataLines = testToSave.EncodeToLines();
            FileWriter fileWriter = new FileWriter(testMetadata.testTitle);
            fileWriter.WriteListLineByLine(testDataLines);
            // Append new test's title to list of existing
            string tranliteratedTestTitle = DataDecoder.TransliterateToEnglish(testMetadata.testTitle);
            fileWriter = new FileWriter();
            fileWriter.AppendLineToFile(tranliteratedTestTitle);

            // Copying all images to a corresponding image folder-database
            foreach(ImageManager.ImageMetadata currentImageInfo in imageMetadatas)
            {
                if (questionsToSave.Count < currentImageInfo.questionIndex) break;
                bool imageNeedsMovement = questionsToSave[currentImageInfo.questionIndex - 1].hasLinkedImage;
                if (imageNeedsMovement)
                {
                    string imageName = $"{tranliteratedTestTitle}-{currentImageInfo.questionIndex}";
                    CopyImageToFolder(currentImageInfo.imagePath, imageName);
                }
            }

            UpdateTitleIfChanged();
            MessageBox.Show("Тест успішно збережено");

            WindowCaller.ShowMain();
            askForClosingComfirmation = false;
            Close();
        }
        /// <summary>
        /// Changes test title in paths of images and resets(deletes) old test passing data
        /// </summary>
        public void UpdateTitleIfChanged()
        {
            bool titleChanged = string.Compare(transliterOldTestTitle,
                    DataDecoder.TransliterateToEnglish(testMetadata.testTitle)) != 0
                    && transliterOldTestTitle != string.Empty;
            if (titleChanged)
            {
                // Renaming all the images
                string transliteratedNewTestTitle = DataDecoder.TransliterateToEnglish(testMetadata.testTitle);
                ImageManager.RenameImagesByTitles(transliterOldTestTitle, transliteratedNewTestTitle);
                foreach (ImageManager.ImageMetadata currImageInfo in imageMetadatas)
                {
                    if (currImageInfo.imagePath.Contains(transliterOldTestTitle))
                    {
                        currImageInfo.imagePath.Replace(transliterOldTestTitle, transliteratedNewTestTitle);
                    }
                }

                DataEraser.EraseTestPassingDataByTitle(transliterOldTestTitle);
            }
        }
        /// <summary>
        /// Checks input and writes it into TestStructs.TestMetadata
        /// </summary>
        /// <returns>true, if input is correct; false if not</returns>
        private bool InputIsCorrect()
        {
            try
            {
                bool titleBlockIsNotSet = string.IsNullOrWhiteSpace(TestTitleBlock.Text) || string.Compare(TestTitleBlock.Text, "Введіть назву тесту") == 0;

                bool timerIsEmpty = string.IsNullOrWhiteSpace(TimerInputBox.Text);
                if (titleBlockIsNotSet || timerIsEmpty) throw new ArgumentNullException();

                bool timerIsSetWrong = !int.TryParse(TimerInputBox.Text, out testMetadata.timerValue) || int.Parse(TimerInputBox.Text) < 0;
                if (timerIsSetWrong) throw new FormatException();

                testMetadata.lastEditedTime = DateTime.Now;
                testMetadata.testTitle = TestTitleBlock.Text;
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
        /// Puts list of QuestionMetadatas into GUI
        /// </summary>
        /// <param name="questionMetadatas">List of QuestionMetadatas structure</param>
        public void DisplayQuestionsFromMetadatas(List<TestStructs.QuestionMetadata> questionMetadatas)
        {
            foreach (TestStructs.QuestionMetadata metadata in questionMetadatas)
            {
                AddNewQuestionsListViewRow(metadata.question);
            }
        }
        /// <summary>
        /// Creates a new row of QuestionsListView
        /// </summary>
        /// <param name="questionText">Current quesiton's text</param>
        private void AddNewQuestionsListViewRow(string questionText)
        {
            QuestionItem newItem = new QuestionItem
            {
                Question = questionText
            };

            questionItems.Add(newItem);
        }
        /// <summary>
        /// Handling window closing event
        /// </summary>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // If closing confirmation is not needed, just close the window
            if (!askForClosingComfirmation) return;
            string additionalMessage = "Дані тесту буде втрачено. ";

            if (e.GetClosingConfirmation(additionalMessage))
            {
                if (isCreatingMode)
                {
                    DataEraser.EraseTestCreatingMode(testMetadata);
                }
                else
                {
                    DataEraser.EraseTestEditingMode(testMetadata, imageMetadatas);
                }
            }
        }
    }
}
