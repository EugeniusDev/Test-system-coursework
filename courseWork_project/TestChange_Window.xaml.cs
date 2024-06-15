using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using System.IO;
using System.Linq;
using courseWork_project.DatabaseRelated;
using courseWork_project.ImageManipulation;
using System.Drawing;
using courseWork_project.GuiManipulation;

namespace courseWork_project
{
    /// <summary>
    /// Interaction logic for TestChange_Window.xaml
    /// </summary>
    /// <remarks> TestChange_Window.xaml is used for creating/editing test's QuestionMetadatas</remarks>
    public partial class TestChange_Window : Window
    {
        private int currentQuestionIndex = 1;
        private int totalQuestionsCount = 1;
        private readonly bool isCreatingMode;
        private readonly List<TestStructs.QuestionMetadata> questionMetadatas;
        private readonly List<ImageManager.ImageMetadata> imageMetadatas;
        private readonly TestStructs.TestMetadata testMetadata;
        /// <summary>
        /// TextBox-CheckBox dictionary for variants data manipulation
        /// </summary>
        /// <remarks>Changes on adding and removing variants</remarks>
        private readonly Dictionary<TextBox, CheckBox> variantComponents = new Dictionary<TextBox, CheckBox>();
        /// <summary>
        /// Used to determine if window closing confirmation is needed
        /// </summary>
        bool isWindowClosingConfirmationRequired = true;
        /// <summary>
        /// List of ImageInfos scheduled for deletion
        /// </summary>
        private readonly List<ImageManager.ImageMetadata> imagesScheduledToDelete = new List<ImageManager.ImageMetadata>();

        public TestChange_Window()
        {
            InitializeComponent();
            questionMetadatas = new List<TestStructs.QuestionMetadata>();
            imageMetadatas = new List<ImageManager.ImageMetadata>();
            isCreatingMode = true;
            UpdateUI();
        }
        /// <summary>
        /// Editing mode constructor
        /// </summary>
        /// <param name="imageSources">List of images (can be empty)</param>
        /// <param name="indexOfElementToEdit">Index of question to edit (1-10)</param>
        public TestChange_Window(Test testToChange, List<ImageManager.ImageMetadata> imageSources, int indexOfElementToEdit)
        {
            isCreatingMode = false;
            questionMetadatas = testToChange.QuestionMetadatas;
            testMetadata = testToChange.TestMetadata;
            if (imageSources.Count == 0)
            {
                imageMetadatas = testToChange.GetRelatedImages();
            }
            else
            {
                imageMetadatas = imageSources;
            }

            InitializeComponent();

            ShowQuestionAtIndex(indexOfElementToEdit - 1);
        }
        /// <summary>
        /// Handling pressed BackToMain_Button
        /// </summary>
        /// <remarks>Opens confirmation of going to main page</remarks>
        private void BackToMain_Button_Click(object sender, RoutedEventArgs e)
        {
            if (!UpdateCurrentQuestionInfo())
            {
                return;
            }

            TryOpenMainWindow();
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
        /// Handling pressed PrevQuestion_Button
        /// </summary>
        /// <remarks>Saves current question and displays previous one</remarks>
        private void PrevQuestion_Button_Click(object sender, RoutedEventArgs e)
        {
            if (!UpdateCurrentQuestionInfo()) return;
            currentQuestionIndex--;
            ShowQuestionAtIndex(currentQuestionIndex-1);
        }
        /// <summary>
        /// Handling pressed SaveTest_Button
        /// </summary>
        /// <remarks>Calls GoToSavingWindow</remarks>
        private void SaveTest_Button_Click(object sender, RoutedEventArgs e)
        {
            GoToSavingWindow();
        }
        /// <summary>
        /// Handling change of a value of any CheckBox
        /// </summary>
        private void CheckBox_Updated(object sender, RoutedEventArgs e)
        {
            UpdateVariantsCheckedConditions();
        }
        /// <summary>
        /// Handling pressed AddVariant_Button
        /// </summary>
        /// <remarks>Adds new variant and updates GUI</remarks>
        private void AddVariant_Button_Click(object sender, RoutedEventArgs e)
        {
            AddNewVariant();
            UpdateUI();
        }
        /// <summary>
        /// Handling pressed RemoveLastVariant_Button
        /// </summary>
        /// <remarks>Deletes last variant and updates GUI</remarks>
        private void RemoveLastVariant_Button_Click(object sender, RoutedEventArgs e)
        {
            RemoveLastVariant();
            UpdateUI();
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
            CurrentQuestion_Text.Text = $"{currentQuestionIndex}/{totalQuestionsCount}";
        }

        private void UpdateNavigationButtonsVisibility()
        {
            if (currentQuestionIndex == 1)
            {
                PrevQuestion_Button.Visibility = Visibility.Collapsed;
            }
            else if (currentQuestionIndex == Properties.Settings.Default.questionsLimit)
            {
                NextQuestion_Button.Visibility = Visibility.Collapsed;
            }
            else
            {
                PrevQuestion_Button.Visibility = Visibility.Visible;
                NextQuestion_Button.Visibility = Visibility.Visible;
            }
        }

        private void UpdateVariantButtonsVisibility()
        {
            if (variantsPanel.Children.Count == 0)
            {
                AddVariant_Button.Visibility = Visibility.Visible;
                RemoveLastVariant_Button.Visibility = Visibility.Collapsed;
            }
            else if (variantsPanel.Children.Count == Properties.Settings.Default.variantsLimit)
            {
                AddVariant_Button.Visibility = Visibility.Collapsed;
            }
            else
            {
                AddVariant_Button.Visibility = Visibility.Visible;
                RemoveLastVariant_Button.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Erases data from QuestionInput; WrapPanel list; TextBox-CheckBox dictionary
        /// </summary>
        private void EraseInputElementsData()
        {
            QuestionInput.Text = string.Empty;
            variantsPanel.Children.Clear();
            variantComponents.Clear();
        }
        /// <summary>
        /// Checking if all required textboxes filled
        /// </summary>
        private bool AreAllTextboxesFilled()
        {
            string questionString = QuestionInput.Text;
            bool questionInputIsNull = string.IsNullOrWhiteSpace(questionString);
            if (questionInputIsNull) return false;
            foreach (var textBoxRelatedPair in variantComponents)
            {
                string tempStringOfVariant = textBoxRelatedPair.Key.Text;
                bool variantIsNotFilled = string.IsNullOrWhiteSpace(tempStringOfVariant) 
                    || string.Compare(tempStringOfVariant, "Введіть варіант відповіді") == 0;
                if (variantIsNotFilled || variantsPanel.Children.Count == 0)
                    return false;
            }
            return true;
        }
        /// <summary>
        /// Checking the proper filling of required fields
        /// </summary>
        /// <remarks>Fields must contain not only digits</remarks>
        private bool IsTextboxesInputValid()
        {
            string pattern = @"[^0-9]";
            bool resultOfCheck = Regex.IsMatch(QuestionInput.Text, pattern);
            foreach (var textBoxRelatedPair in variantComponents)
            {
                string tempStringOfVariant = textBoxRelatedPair.Key.Text;
                if (!Regex.IsMatch(tempStringOfVariant, pattern)) return false;
            }
            return resultOfCheck;
        }

        private void AddNewVariant(string variantText = "Введіть варіант відповіді", bool isVariantCorrect = false)
        {
            bool variantsLimitReached = variantsPanel.Children.Count >= Properties.Settings.Default.variantsLimit;
            if (variantsLimitReached)
            {
                return;
            }

            TextBox textBox = SampleGuiElementsFactory.MakeVariantTextbox(variantText);
            CheckBox checkBox = SampleGuiElementsFactory.MakeVariantCheckbox(isVariantCorrect);
            HangCheckingEvents(checkBox);

            variantComponents.Add(textBox, checkBox);

            DockPanel dockPanel = SampleGuiElementsFactory.MakeVariantDockpanel(textBox, checkBox);
            variantsPanel.Children.Add(dockPanel);
        }

        private void HangCheckingEvents(CheckBox checkBox)
        {
            checkBox.Unchecked += CheckBox_Updated;
            checkBox.Checked += CheckBox_Updated;
        }

        private void RemoveLastVariant()
        {
            if (variantsPanel.Children.Count != 0)
            {
                variantsPanel.Children.RemoveAt(variantsPanel.Children.Count - 1);
                variantComponents.Remove(variantComponents.Keys.Last());
            }
        }
        /// <summary>
        /// Updates data about current question
        /// </summary>
        /// <returns>true, if updates successful; false if not</returns>
        private bool UpdateCurrentQuestionInfo()
        {
            try
            {
                int indexOfCurrentQuestion = currentQuestionIndex - 1;
                TestStructs.QuestionMetadata currQuestion;

                if (!AreAllTextboxesFilled())
                {
                    throw new ArgumentException("At least one textbox is not filled");
                }
                if(!IsTextboxesInputValid())
                {
                    throw new FormatException();
                }

                currQuestion.question = QuestionInput.Text.Trim();
                ImageManager.ImageMetadata foundImage = imageMetadatas.Find(x => x.questionIndex == currentQuestionIndex);
                currQuestion.hasLinkedImage = false;
                if (!foundImage.Equals(default(ImageManager.ImageMetadata)))
                {
                    currQuestion.hasLinkedImage = true;
                }
                currQuestion.variants = new List<string>();
                currQuestion.correctVariantsIndeces = new List<int>();
                int tempIndexForCorrectVariants = 0;
                foreach (var pairOfTextBoxAndCheckBox in variantComponents)
                {
                    if ((bool)pairOfTextBoxAndCheckBox.Value.IsChecked)
                    {
                        currQuestion.correctVariantsIndeces.Add(tempIndexForCorrectVariants);
                    }
                    currQuestion.variants.Add(pairOfTextBoxAndCheckBox.Key.Text.Trim());
                    tempIndexForCorrectVariants++;
                }

                if (currQuestion.correctVariantsIndeces.Count == 0 || currQuestion.variants.Count == 1)
                {
                    throw new ArgumentNullException();
                }
                if (questionMetadatas.Count >= indexOfCurrentQuestion+1)
                {
                    questionMetadatas.Insert(indexOfCurrentQuestion, currQuestion);
                    questionMetadatas.RemoveAt(indexOfCurrentQuestion+1);
                }
                else
                {
                    questionMetadatas.Add(currQuestion);
                }

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
        /// Updates variants textboxes color according to corrresponding checkboxes state
        /// </summary>
        private void UpdateVariantsCheckedConditions()
        {
            try
            {
                foreach(var currVariant in variantComponents)
                {
                    if ((bool)currVariant.Value.IsChecked)
                    {
                        currVariant.Key.Background = new SolidColorBrush(Colors.DarkGreen);
                        currVariant.Key.Foreground = new SolidColorBrush(Colors.White);
                    }
                    else
                    {
                        currVariant.Key.Background = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#fff0f0");
                        currVariant.Key.Foreground = new SolidColorBrush(Colors.Black);
                    }
                }
            }
            catch (NullReferenceException)
            {
                MessageBox.Show($"Немає варіантів відповідей. Створіть їх, використовуючи кнопку \"{AddVariant_Button.Content}\"");
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
                if(currentQuestionIndex > 1)
                {
                    if (!UpdateCurrentQuestionInfo()) return;
                    currentQuestionIndex--;
                    ShowQuestionAtIndex(currentQuestionIndex - 1);
                    return;
                }

                TryOpenMainWindow();
            }
            if(e.Key == Key.Enter)
            {
                if(currentQuestionIndex != Properties.Settings.Default.questionsLimit)
                {
                    GoToNextQuestion();
                    return;
                }

                GoToSavingWindow();
            }
        }
        /// <summary>
        /// Opens confirmation of going to main page
        /// </summary>
        private void TryOpenMainWindow()
        {
            string confirmationString = "Всі дані цього тесту втратяться, коли ви перейдете на головну сторінку. Ви справді хочете це зробити?";
            MessageBoxResult result = MessageBox.Show(confirmationString,
                "Підтвердження переходу на головну сторінку", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result.Equals(MessageBoxResult.Yes))
            {
                EraseCurrentTestData();

                WindowCaller.ShowMain();
                this.CloseWindowAndDisableConfirmationPrompt(ref isWindowClosingConfirmationRequired);
            }
        }

        private void EraseCurrentTestData()
        {
            DataEraser.EraseTestDatabases(testMetadata);
            if (!isCreatingMode)
            {
                ImageManager.TryDeleteImages(imageMetadatas);
            }
        }

        /// <summary>
        /// Saves current test question, creates/displays the next
        /// </summary>
        private void GoToNextQuestion()
        {
            if (!UpdateCurrentQuestionInfo()) return;
            if (questionMetadatas.Count > currentQuestionIndex)
            {
                currentQuestionIndex++;
                ShowQuestionAtIndex(currentQuestionIndex - 1);
                return;
            }
            if (currentQuestionIndex == totalQuestionsCount && currentQuestionIndex != Properties.Settings.Default.questionsLimit)
            {
                totalQuestionsCount++;
                currentQuestionIndex++;
                UpdateUI();
                EraseInputElementsData();
            }
        }
        /// <summary>
        /// Saves current question and opens TestSaving_Window
        /// </summary>
        private void GoToSavingWindow()
        {
            if (!UpdateCurrentQuestionInfo()) return;
            ReturnDefaultImage();
            if (isCreatingMode)
            {
                WindowCaller.ShowTestSavingCreatingMode(questionMetadatas, imageMetadatas);
            }
            else
            {
                Test testToSave = new Test(questionMetadatas, testMetadata);
                WindowCaller.ShowTestSavingEditingMode(testToSave, imageMetadatas);
            }

            this.CloseWindowAndDisableConfirmationPrompt(ref isWindowClosingConfirmationRequired);
        }
        /// <summary>
        /// Puts current test's question list in GUI
        /// </summary>
        /// <param name="questions">Список структур запитань тесту</param>
        public void GetListAndPutItInGUI(List<TestStructs.QuestionMetadata> questions)
        {
            TestStructs.QuestionMetadata currentQuestion = questions[currentQuestionIndex - 1];
            QuestionInput.Text = currentQuestion.question;
            int tempIndexOfCorrectVariant = 0;
            foreach(string variant in currentQuestion.variants)
            {
                bool variantIsCorrect = currentQuestion.correctVariantsIndeces.Contains(tempIndexOfCorrectVariant);
                AddNewVariant(variant, variantIsCorrect);
                tempIndexOfCorrectVariant++;
            }
            UpdateVariantsCheckedConditions();
            UpdateUI();
        }
        /// <summary>
        /// Shows test's question under given index
        /// </summary>
        /// <param name="indexOfElementToEdit">Index of element to show (values from 0 to 9)</param>
        private void ShowQuestionAtIndex(int indexOfElementToEdit)
        {
            currentQuestionIndex = ++indexOfElementToEdit;
            totalQuestionsCount = questionMetadatas.Count;
            EraseInputElementsData();
            GetListAndPutItInGUI(questionMetadatas);
            UpdateUI();
        }
        /// <summary>
        /// Handling window closing event
        /// </summary>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // TODO make method for these two lines
            ImageManager.TryDeleteImages(imagesScheduledToDelete);
            imagesScheduledToDelete.Clear();

            if (isWindowClosingConfirmationRequired
                && e.GetClosingConfirmation())
            {
                    EraseCurrentTestData();
            }
        }
        /// <summary>
        /// Handling pressed ImageChange_Button
        /// </summary>
        /// <remarks>Opens dialog window to choose new image</remarks>
        private void ImageChange_Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files (*.jpg, *.jpeg, *.png, *.bmp)|*.jpg;*.jpeg;*.png;*.bmp)"
            };
            // If any file is chosen
            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFilePath = openFileDialog.FileName;

                ImageManager.ImageMetadata currentImageInfo = new ImageManager.ImageMetadata
                {
                    questionIndex = currentQuestionIndex,
                    imagePath = selectedFilePath
                };
                PushImageSource(currentImageInfo);
                // Displaying chosen image
                BitmapImage tempImageSource = new BitmapImage();
                tempImageSource.BeginInit();
                tempImageSource.UriSource = new Uri(selectedFilePath, UriKind.RelativeOrAbsolute);
                tempImageSource.CacheOption = BitmapCacheOption.OnLoad;
                tempImageSource.EndInit();
                IllustrationImage.Source = tempImageSource;
            }
        }
        /// <summary>
        /// Handling pressed ImageDeletion_Button
        /// </summary>
        /// <remarks>Deletes link to correspoding ImageMetadata</remarks>
        private void ImageDeletion_Button_Click(object sender, RoutedEventArgs e)
        {
            ImageManager.ImageMetadata imageToDelete = imageMetadatas.Find(x => x.questionIndex == currentQuestionIndex);
            if (!imageToDelete.Equals(default(ImageManager.ImageMetadata)))
            {
                imageMetadatas.RemoveAt(imageMetadatas.IndexOf(imageToDelete));

                ReturnDefaultImage();
                if (!isCreatingMode && questionMetadatas.Count >= currentQuestionIndex
                    && questionMetadatas[currentQuestionIndex - 1].hasLinkedImage)
                {
                    imagesScheduledToDelete.Add(imageToDelete);
                    TestStructs.QuestionMetadata questionToUpdate = questionMetadatas[currentQuestionIndex - 1];
                    questionToUpdate.hasLinkedImage = false;
                    questionMetadatas.Insert(currentQuestionIndex - 1, questionToUpdate);
                    questionMetadatas.RemoveAt(currentQuestionIndex);
                }
            }
        }
        /// <summary>
        /// Showing default image
        /// </summary>
        private void ReturnDefaultImage()
        {
            Bitmap defaultImage = DefaultImage.default_image;
            // Convert Bitmap to BitmapImage
            BitmapImage bitmapImage = null;
            using (MemoryStream memory = new MemoryStream())
            {
                // Save Bitmap to memory stream
                defaultImage.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);

                // Rewind the stream
                memory.Position = 0;

                // Create BitmapImage
                bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                // This is important to prevent file locks
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
            }
            IllustrationImage.Source = bitmapImage;
        }
        /// <summary>
        /// Updates what image to display at current question
        /// </summary>
        private void UpdateImageAppearance()
        {
            ReturnDefaultImage();
            if (imageMetadatas == null || imageMetadatas.Count == 0) return;
            ImageManager.ImageMetadata foundImage = imageMetadatas.Find(x => x.questionIndex == currentQuestionIndex);

            if (!foundImage.Equals(default(ImageManager.ImageMetadata)))
            {
                BitmapImage foundImageBitmap = new BitmapImage();
                foundImageBitmap.BeginInit();
                foundImageBitmap.UriSource = new Uri(foundImage.imagePath, UriKind.RelativeOrAbsolute);
                foundImageBitmap.CacheOption = BitmapCacheOption.OnLoad;
                foundImageBitmap.EndInit();
                IllustrationImage.Source = foundImageBitmap;
            }
        }
        /// <summary>
        /// Updates images list
        /// </summary>
        /// <param name="imageInfoToPush">ImageMetadata to push into list</param>
        private void PushImageSource(ImageManager.ImageMetadata imageInfoToPush)
        {
            if (imageMetadatas.Count == 0)
            {
                imageMetadatas.Add(imageInfoToPush);
                return;
            }

            ImageManager.ImageMetadata foundImage = imageMetadatas.Find(x => x.questionIndex == currentQuestionIndex);
            if (!foundImage.Equals(default(ImageManager.ImageMetadata)))
            {
                int indexToUpdate = imageMetadatas.IndexOf(foundImage);
                imageMetadatas.Insert(indexToUpdate, imageInfoToPush);
                imageMetadatas.RemoveAt(indexToUpdate + 1);
            }
            else
            {
                imageMetadatas.Add(imageInfoToPush);
            }
            imageMetadatas.Sort((a, b) => a.questionIndex.CompareTo(b.questionIndex));
        }
    }
}
