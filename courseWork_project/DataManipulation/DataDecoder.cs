using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace courseWork_project
{
    /// <summary>
    /// Клас, методи якого розкодовують інформацію з бази даних та формують потрібну структуру даних
    /// </summary>
    public static class DataDecoder
    {
        #region Transliteration
        private static readonly Dictionary<char, string> transliterationTable = new Dictionary<char, string>
        {
            {'а', "a"}, {'б', "b"}, {'в', "v"}, {'г', "g"}, {'д', "d"}, {'е', "e"},
            {'є', "ye"}, {'ж', "zh"}, {'з', "z"}, {'и', "y"}, {'і', "i"}, {'ї', "yi"},
            {'й', "y"}, {'к', "k"}, {'л', "l"}, {'м', "m"}, {'н', "n"}, {'о', "o"},
            {'п', "p"}, {'р', "r"}, {'с', "s"}, {'т', "t"}, {'у', "u"}, {'ф', "f"},
            {'х', "kh"}, {'ц', "ts"}, {'ч', "ch"}, {'ш', "sh"}, {'щ', "shch"}, {'ь', ""},
            {'ю', "yu"}, {'я', "ya"}, {'ґ', "g"}, {' ', "_"}, {'<', "_"}, {'>', "_"}, {':', "_"},
            {'\"', "_"}, {'/', "_"}, {'\\', "_"}, {'|', "_"}, {'?', "_"}, {'*', "_"}
        };
        private static readonly char separator = Properties.Settings.Default.dataSeparator;
        private const int indexOfTestMetadataLine = 0;
        private const int minimumRequiredLinesCount = 2;

        public static string TransliterateToEnglish(this string inputString)
        {
            if (inputString is null)
            {
                return string.Empty;
            }

            if (inputString.IsTransliterated())
            {
                return inputString;
            }

            string transliteratedString = string.Empty;
            foreach(char c in inputString.ToLower())
            {
                transliteratedString = transliterationTable.ContainsKey(c) ? 
                    string.Concat(transliteratedString, transliterationTable[c])
                    : string.Concat(transliteratedString, c);
            }

            return transliteratedString;
        }

        private static bool IsTransliterated(this string testTitle)
        {
            return !transliterationTable.Keys.Any(character => testTitle.Contains(character));
        }
        #endregion
        #region QuestionMetadata related
        public static List<TestStructs.QuestionMetadata> GetAllQuestionsByTestTitles(List<string> transliteratedTitles)
        {
            List<TestStructs.QuestionMetadata> allExistingQuestions = new List<TestStructs.QuestionMetadata>();
            foreach (string transliteratedTitle in transliteratedTitles)
            {
                List<TestStructs.QuestionMetadata> tempQuestionsList = GetQuestionMetadatasByTitle(transliteratedTitle);
                allExistingQuestions.AddRange(tempQuestionsList);
            }

            return allExistingQuestions;
        }

        public static List<TestStructs.QuestionMetadata> GetQuestionMetadatasByTitle(string testTitle)
        {
            List<TestStructs.QuestionMetadata> formedQuestionsList = new List<TestStructs.QuestionMetadata>();
            try
            {
                List<string> questionMetadataLines = GetQuestionMetadataLines(testTitle);
                foreach (string line in questionMetadataLines)
                {
                    TestStructs.QuestionMetadata currentMetadata = new TestStructs.QuestionMetadata();
                    currentMetadata = line.ParseToQuestionMetadata();
                    formedQuestionsList.Add(currentMetadata);
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Помилка! Дані з бази даних некоректні!");
                return formedQuestionsList;
            }

            return formedQuestionsList;
        }

        private static TestStructs.QuestionMetadata ParseToQuestionMetadata(this string input)
        {
            TestStructs.QuestionMetadata questionMetadata = new TestStructs.QuestionMetadata();
            questionMetadata.variants = new List<string>();
            questionMetadata.correctVariantsIndeces = new List<int>();

            string[] splitLine = input.Split(new char[]{ separator }, StringSplitOptions.RemoveEmptyEntries);
            questionMetadata.question = splitLine[0];
            int imageInfoIndex = splitLine.Length - 1;
            for (int i = 1; i < imageInfoIndex; i++)
            {
                bool currentPartIsInt = int.TryParse(splitLine[i], out int correctAnswerIndex)
                    && splitLine[i].Length == 1;
                if (currentPartIsInt)
                {
                    questionMetadata.correctVariantsIndeces.Add(correctAnswerIndex);
                }
                else
                {
                    questionMetadata.variants.Add(splitLine[i]);
                }
            }

            if (!bool.TryParse(splitLine[imageInfoIndex], out bool imageIsLinked))
            {
                imageIsLinked = false;
            }

            questionMetadata.hasLinkedImage = imageIsLinked;

            return questionMetadata;
        }

        private static List<string> GetQuestionMetadataLines(string testTitle)
        {
            List<string> wholeTestData = GetValidTestData(testTitle);

            wholeTestData.RemoveAt(indexOfTestMetadataLine);

            return wholeTestData;
        }
        #endregion
        #region Test aspects in general
        private static List<string> GetValidTestData(string testTitle)
        {
            FileReader reader = new FileReader(testTitle);
            List<string> wholeTestData = reader.GetFileContentInLines();
            ValidateWholeTestData(wholeTestData);

            return wholeTestData;
        }

        private static void ValidateWholeTestData(List<string> wholeTestData)
        {
            if (wholeTestData.Count < minimumRequiredLinesCount)
            {
                throw new FormatException();
            }
        }

        public static Test GetTestByTestItem(TestItem selectedItem)
        {
            return GetTestByTitle(selectedItem.TestTitle);
        }

        public static Test GetTestByTitle(string testTitle)
        {
            return new Test(GetQuestionMetadatasByTitle(testTitle),
                GetTestMetadataByTitle(testTitle));
        }

        private static readonly string resultsDirectoryName = Properties.Settings.Default.testResultsDirectory;
        public static List<string> GetTestResultsByTitle(string testTitle)
        {
            FileReader fileReader = new FileReader(testTitle);
            fileReader.UpdateDirectoryName(resultsDirectoryName);
            return fileReader.GetFileContentInLines();
        }
        #endregion
        #region TestMetadata related
        public static List<TestStructs.TestMetadata> GetAllTestMetadatasByTitles(List<string> transliteratedTitles)
        {
            List<TestStructs.TestMetadata> listToReturn = new List<TestStructs.TestMetadata>();
            foreach (string transliteratedTitle in transliteratedTitles)
            {
                TestStructs.TestMetadata currentTestMetadata = GetTestMetadataByTitle(transliteratedTitle);
                if (!currentTestMetadata.Equals(TestStructs.EmptyTestMetadata))
                {
                    listToReturn.Add(currentTestMetadata);
                }
            }
            return listToReturn;
        }

        public static TestStructs.TestMetadata GetTestMetadataByTitle(string testTitle)
        {
            try
            {
                string testMetadataLine = GetTestMetadataLine(testTitle);
                return testMetadataLine.ParseToTestMetadata();
            }
            catch (FormatException)
            {
                MessageBox.Show("Помилка! Дані з бази даних некоректні!");
                return TestStructs.EmptyTestMetadata;
            }
        }

        private static string GetTestMetadataLine(string testTitle)
        {
            List<string> wholeTestData = GetValidTestData(testTitle);

            string testMetadataLine = wholeTestData[indexOfTestMetadataLine];
            return testMetadataLine;
        }
        
        private static TestStructs.TestMetadata ParseToTestMetadata(this string input)
        {
            string[] stringToSplit = input.Split(new char[] { separator }, StringSplitOptions.RemoveEmptyEntries);
            if (stringToSplit.Length != 3)
            {
                throw new FormatException();
            }

            if (!DateTime.TryParse(stringToSplit[1], out DateTime editingDate))
            {
                editingDate = DateTime.Now;
            }

            if (!int.TryParse(stringToSplit[2], out int timerValue))
            {
                timerValue = 0;
            }

            return new TestStructs.TestMetadata()
            {
                testTitle = stringToSplit[0],
                lastEditedTime = editingDate,
                timerValueInMinutes = timerValue
            };
        }
        #endregion
        #region ImageMetadata related
        public static ImageManager.ImageMetadata ParseToImageMetadata(this string fileName)
        {
            ImageManager.ImageMetadata imageMetadata = ImageManager.EmptyImageMetadata;
            if (fileName.TryRetrieveValidQuestionIndex(ref imageMetadata.questionIndex))
            {
                imageMetadata.path = Path.GetFullPath(fileName);
            }

            return imageMetadata;
        }

        private static bool TryRetrieveValidQuestionIndex(this string fileName, ref int linkedQuestionIndex)
        {
            int lastHyphenIndex = fileName.LastIndexOf('-');
            int lastDotIndex = fileName.LastIndexOf('.');
            if (!ImageFilenameIsValid(lastHyphenIndex, lastDotIndex))
            {
                return false;
            }

            int firstDigitIndex = lastHyphenIndex + 1;
            int lengthOfQuestionIndex = lastDotIndex - firstDigitIndex;
            string supposedQuestionIndex = fileName.Substring(firstDigitIndex, lengthOfQuestionIndex);

            return int.TryParse(supposedQuestionIndex, out linkedQuestionIndex);
        }

        private static bool ImageFilenameIsValid(int lastHyphenIndex, int lastDotIndex)
        {
            return lastHyphenIndex > 0 && lastDotIndex > 0 && lastHyphenIndex < lastDotIndex;
        }
        #endregion
    }
}
