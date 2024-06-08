using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Windows;

namespace courseWork_project
{
    /// <summary>
    /// Клас, методи якого розкодовують інформацію з бази даних та формують потрібну структуру даних
    /// </summary>
    public static class DataDecoder
    {
        /// <summary>
        /// Словник транслітерування для використання рядків в якості шляхів до баз даних
        /// </summary>
        /// <remarks>Містить символи, що потребують заміни та самі символи заміни</remarks>
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
        /// <summary>
        /// TestInfo with sample data
        /// </summary>
        private static TestStructs.TestInfo NullTestInfo
        {
            get
            {
                TestStructs.TestInfo nullTestInfo;
                nullTestInfo.testTitle = "null";
                nullTestInfo.lastEditedTime = DateTime.Now;
                nullTestInfo.timerValue = 0;
                return nullTestInfo;
            }
        }
        /// <summary>
        /// Gets question info strings and forms Question structures from them
        /// </summary>
        /// <remarks>Uses FileReader under the hood</remarks>
        /// <param name="testTitle">Test title, not transliterated is also allowed</param>
        /// <returns>List of Questions, populated or empty</returns>
        public static List<TestStructs.Question> GetQuestionsByTitle(string testTitle)
        {
            List<TestStructs.Question> formedQuestionsList = new List<TestStructs.Question>();
            FileReader reader = new FileReader(testTitle);
            List<string> textInLines = reader.GetQuestionLines();
            foreach (string line in textInLines)
            {
                TestStructs.Question tempQuestion = new TestStructs.Question
                {
                    variants = new List<string>(),
                    correctVariantsIndeces = new List<int>()
                };
                string[] splitLine = line.Split(new char[] { '₴' }, StringSplitOptions.RemoveEmptyEntries);
                tempQuestion.question = splitLine[0];
                for (int i = 1; i < splitLine.Length - 1; i++)
                {
                    // If current part can be parsed to int, add it to correctVariantsIndeces list
                    if (int.TryParse(splitLine[i], out int correctAnswerIndex) && splitLine[i].Length == 1)
                    {
                        tempQuestion.correctVariantsIndeces.Add(correctAnswerIndex);
                    }
                    // Else add current part to a variants list
                    else if (tempQuestion.variants.Count < 8)
                    {
                        tempQuestion.variants.Add(splitLine[i]);
                    }
                }
                // Last part is converted into info about illustration
                tempQuestion.hasLinkedImage = bool.Parse(splitLine[splitLine.Length - 1]);
                formedQuestionsList.Add(tempQuestion);
            }
            return formedQuestionsList;
        }
        /// <summary>
        /// Gets first line from test's file-database and populates corresponding TestInfo structure
        /// </summary>
        /// <remarks>Uses FileReader under the hood</remarks>
        /// <param name="testTitle">Test title, not transliterated is also allowed</param>
        /// <returns>Populated or empty TestStructs.TestInfo</returns>
        public static TestStructs.TestInfo GetTestInfoByTitle(string testTitle)
        {
            try
            {
                FileReader reader = new FileReader(testTitle);
                string[] stringToSplit = reader.GetTestInfo().Split(new char[] { '₴' }, StringSplitOptions.RemoveEmptyEntries);
                if (stringToSplit.Length < 3) throw new FormatException();

                TestStructs.TestInfo currentTestInfo;
                currentTestInfo.testTitle = stringToSplit[0];
                currentTestInfo.lastEditedTime = DateTime.Parse(stringToSplit[1]);
                int.TryParse(stringToSplit[2], out int timerValue);
                currentTestInfo.timerValue = timerValue;
                return currentTestInfo;
            }
            catch (FormatException)
            {
                MessageBox.Show("Помилка! Дані з бази даних некоректні!");
                return NullTestInfo;
            }
        }

        public static string TransliterateToEnglish(this string inputString)
        {
            if (inputString is null) return string.Empty;

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

        /// <summary>
        /// Gets list of all Questions of all tests
        /// </summary>
        /// <param name="transliteratedTitles">List of transliterated test titles</param>
        /// <returns>List of all Questions available</returns>
        public static List<TestStructs.Question> GetAllExistingQuestionsByTestTitles(List<string> transliteratedTitles)
        {
            List<TestStructs.Question> listToReturn = new List<TestStructs.Question>();
            foreach (string transliteratedTitle in transliteratedTitles)
            {
                List<TestStructs.Question> tempQuestionsList = GetQuestionsByTitle(transliteratedTitle);
                listToReturn.AddRange(tempQuestionsList);
            }
            return listToReturn;
        }
        /// <summary>
        /// Gets list of all TestInfos of all tests
        /// </summary>
        /// <param name="transliteratedTitles">List of transliterated test titles</param>
        /// <returns>List of all TestInfos available</returns>
        public static List<TestStructs.TestInfo> GetAllExistingTestInfosByTitles(List<string> transliteratedTitles)
        {
            List<TestStructs.TestInfo> listToReturn = new List<TestStructs.TestInfo>();
            foreach (string transliteratedTitle in transliteratedTitles)
            {
                TestStructs.TestInfo currentTestInfo = GetTestInfoByTitle(transliteratedTitle);
                if (!currentTestInfo.Equals(NullTestInfo))
                {
                    listToReturn.Add(currentTestInfo);
                }
            }
            return listToReturn;
        }
    }
}
