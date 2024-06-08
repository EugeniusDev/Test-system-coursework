using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Configuration;
using static courseWork_project.ImageManager;
using System.Windows.Controls;
using courseWork_project.DatabaseRelated;

namespace courseWork_project
{
    /// <summary>
    /// Class for manipulation with ObservableCollection of Questions for GridView.
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
        /// List of Questions of the test
        /// </summary>
        private List<TestStructs.Question> questionsToSave;
        /// <summary>
        /// List of data about images
        /// </summary>
        private List<ImageManager.ImageInfo> imagesList;
        /// <summary>
        /// Overall test info structure
        /// </summary>
        private TestStructs.TestInfo testInfo;
        /// <summary>
        /// Used while updating sources of images
        /// </summary>
        /// <remarks>Contains deprecated transliterated test title</remarks>
        private string transliterOldTestTitle = string.Empty;
        /// <summary>
        /// ObservableCollection for QuestionsListView
        /// </summary>
        private ObservableCollection<QuestionItem> questionItems;
        /// <summary>
        /// Determines the window's mode
        /// </summary>
        /// <remarks>true - creating mode; false - editing mode</remarks>
        private bool creatingMode;
        /// <summary>
        /// Used to determine if window closing confirmation is needed
        /// </summary>
        bool askForClosingComfirmation = true;
        /// <summary>
        /// TestSaving_Window creating mode constructor
        /// </summary>
        /// <param name="questionsToSave">List of Questions of the test</param>
        /// <param name="imagesToSave">List of data about images</param>
        public TestSaving_Window(List<TestStructs.Question> questionsToSave, List<ImageManager.ImageInfo> imagesToSave)
        {
            creatingMode = true;
            this.questionsToSave = questionsToSave;
            imagesList = imagesToSave;
            // There are no time limitations by default
            testInfo.timerValue = 0;
            InitializeComponent();
            TimerInputBox.Text = testInfo.timerValue.ToString();

            questionItems = new ObservableCollection<QuestionItem>();
            QuestionsListView.ItemsSource = questionItems;
            GetListAndPutItInGUI(questionsToSave);
        }
        /// <summary>
        /// TestSaving_Window editing mode constructor
        /// </summary>
        /// <param name="questionsToSave">List of Questions of the test</param>
        /// <param name="currTestInfo">General test info structure</param>
        public TestSaving_Window(List<TestStructs.Question> questionsToSave, List<ImageManager.ImageInfo> imagesToSave, TestStructs.TestInfo currTestInfo)
        {
            creatingMode = false;
            this.questionsToSave = questionsToSave;
            testInfo = currTestInfo;
            transliterOldTestTitle = DataDecoder.TransliterateToEnglish(testInfo.testTitle);
            imagesList = imagesToSave;
            InitializeComponent();
            TimerInputBox.Text = testInfo.timerValue.ToString();

            TestTitleBlock.Foreground = new SolidColorBrush(Colors.Black);
            TestTitleBlock.Text = testInfo.testTitle;
            questionItems = new ObservableCollection<QuestionItem>();

            QuestionsListView.ItemsSource = questionItems;
            GetListAndPutItInGUI(questionsToSave);
        }
        /// <summary>
        /// Handling click on test title
        /// </summary>
        private void TestTitleBlock_GotFocus(object sender, RoutedEventArgs e)
        {
            bool titleContainsDefaultText = TestTitleBlock != null
                && string.Compare(TestTitleBlock.Text, "Введіть назву тесту") == 0;
            if (titleContainsDefaultText || testInfo.testTitle == null)
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

            Button button = sender as Button;
            // Find the parent ListViewItem of the clicked button
            ListViewItem itemContainer = GuiHelper.FindAncestor<ListViewItem>(button);
            if (button != null && itemContainer.DataContext is QuestionItem selectedItem)
            {
                int indexOfElementToEdit = 0;
                for (int i = 0; i < questionsToSave.Count; i++)
                {
                    if (string.Compare(questionsToSave[i].question, selectedItem.Question) == 0)
                    {
                        indexOfElementToEdit = i+1;
                        break;
                    }
                }
                testInfo.testTitle = TestTitleBlock.Text;
                UpdateTitleIfChanged();

                TestChange_Window testChange_Window = new TestChange_Window(questionsToSave, imagesList, testInfo, indexOfElementToEdit);
                testChange_Window.Show();
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
            Button button = sender as Button;
            // Find the parent ListViewItem of the clicked button
            ListViewItem itemContainer = GuiHelper.FindAncestor<ListViewItem>(button);
            if (button != null && itemContainer.DataContext is QuestionItem selectedItem)
            {
                if (imagesList.Count == 0)
                {
                    ImageListFormer imageListFormer = new ImageListFormer();
                    imagesList = imageListFormer.GetImageList(testInfo.testTitle, questionsToSave);
                }

                for (int i  = 0; i < questionsToSave.Count; i++)
                {
                    if (string.Compare(questionsToSave[i].question, selectedItem.Question) == 0)
                    {
                        questionsToSave.RemoveAt(i);
                        ImageManager.ImageInfo imageToDelete = imagesList.Find(x => x.questionIndex == i+1);
                        if (!imageToDelete.Equals(default(ImageInfo)))
                        {
                            imagesList.Remove(imageToDelete);
                            if (!creatingMode)
                            {
                                DataEraser.EraseImage(imageToDelete);
                            }
                        }
                        break;
                    }
                }

                questionItems.Remove(selectedItem);
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
                TestChange_Window testChange_Window = new TestChange_Window(questionsToSave, imagesList, testInfo, questionsToSave.Count);
                testChange_Window.Show();
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
            if (!InputIsCorrect()) return;
            // Saving encoded into lines test data to a file-database
            List<string> listToWrite = DataEncoder.EncodeAndReturnLines(testInfo, questionsToSave);
            FileWriter fileWriter = new FileWriter(testInfo.testTitle);
            fileWriter.WriteListInFileByLines(listToWrite);
            // Updating list of existing databases in corresponding text file
            FileReader fileReader = new FileReader();
            List<string> allTestsList = fileReader.UpdateListOfExistingTestsPaths();

            string tranliteratedCurrTestTitle = DataDecoder.TransliterateToEnglish(testInfo.testTitle);
            allTestsList.Add(tranliteratedCurrTestTitle);

            fileWriter = new FileWriter(fileReader.DirectoryPath, fileReader.FilePath);
            fileWriter.WriteListInFileByLines(allTestsList);
            // Copying all images to a corresponding image folder-database
            foreach(ImageInfo currentImageInfo in imagesList)
            {
                if (questionsToSave.Count < currentImageInfo.questionIndex) break;
                bool imageNeedsMovement = questionsToSave[currentImageInfo.questionIndex - 1].hasLinkedImage;
                if (imageNeedsMovement)
                {
                    string imageName = $"{tranliteratedCurrTestTitle}-{currentImageInfo.questionIndex}";
                    CopyImageToFolder(currentImageInfo.imagePath, imageName);
                }
            }

            UpdateTitleIfChanged();
            MessageBox.Show("Тест успішно збережено");

            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            askForClosingComfirmation = false;
            Close();
        }
        /// <summary>
        /// Changes test title in paths of images and resets(deletes) old test passing data
        /// </summary>
        public void UpdateTitleIfChanged()
        {
            bool titleChanged = string.Compare(transliterOldTestTitle,
                    DataDecoder.TransliterateToEnglish(testInfo.testTitle)) != 0
                    && transliterOldTestTitle != string.Empty;
            if (titleChanged)
            {
                // Renaming all the images
                string transliteratedNewTestTitle = DataDecoder.TransliterateToEnglish(testInfo.testTitle);
                ImageManager.RenameAll(transliterOldTestTitle, transliteratedNewTestTitle);
                foreach (ImageInfo currImageInfo in imagesList)
                {
                    if (currImageInfo.imagePath.Contains(transliterOldTestTitle))
                    {
                        currImageInfo.imagePath.Replace(transliterOldTestTitle, transliteratedNewTestTitle);
                    }
                }

                DataEraser.ErasePassingData(transliterOldTestTitle);
            }
        }
        /// <summary>
        /// Checks input and writes it into TestStructs.TestInfo
        /// </summary>
        /// <returns>true, if input is correct; false if not</returns>
        private bool InputIsCorrect()
        {
            try
            {
                bool titleBlockIsNotSet = string.IsNullOrWhiteSpace(TestTitleBlock.Text) || string.Compare(TestTitleBlock.Text, "Введіть назву тесту") == 0;

                bool timerIsEmpty = string.IsNullOrWhiteSpace(TimerInputBox.Text);
                if (titleBlockIsNotSet || timerIsEmpty) throw new ArgumentNullException();

                bool timerIsSetWrong = !int.TryParse(TimerInputBox.Text, out testInfo.timerValue) || int.Parse(TimerInputBox.Text) < 0;
                if (timerIsSetWrong) throw new FormatException();

                testInfo.lastEditedTime = DateTime.Now;
                testInfo.testTitle = TestTitleBlock.Text;
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
        /// Puts list of Questions into GUI
        /// </summary>
        /// <param name="questionsList">List of Questions structure</param>
        public void GetListAndPutItInGUI(List<TestStructs.Question> questionsList)
        {
            foreach (TestStructs.Question questionFromList in questionsList)
            {
                AddNewQuestionsListViewRow(questionFromList.question);
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
            MessageBoxResult result = MessageBox.Show("Дані тесту буде втрачено. Ви справді хочете закрити програму?", "Підтвердження закриття вікна", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result.Equals(MessageBoxResult.No))
            {
                // Cancelling closing process
                e.Cancel = true;
            }
            else
            {
                DataEraser.EraseCurrentTestData(testInfo, creatingMode, imagesList);
            }
        }
    }
}
