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
        /// TestMetadata with sample data
        /// </summary>
        private static TestStructs.TestMetadata EmptyTestMetadata = new TestStructs.TestMetadata()
        {
            testTitle = "empty",
            lastEditedTime = DateTime.Now,
            timerValue = 0
        };

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

        public static List<TestStructs.QuestionMetadata> GetAllQuestionsByTestTitles(List<string> transliteratedTitles)
        {
            List<TestStructs.QuestionMetadata> listToReturn = new List<TestStructs.QuestionMetadata>();
            foreach (string transliteratedTitle in transliteratedTitles)
            {
                List<TestStructs.QuestionMetadata> tempQuestionsList = GetQuestionMetadatasByTitle(transliteratedTitle);
                listToReturn.AddRange(tempQuestionsList);
            }
            return listToReturn;
        }
        public static List<TestStructs.QuestionMetadata> GetQuestionMetadatasByTitle(string testTitle)
        {
            List<TestStructs.QuestionMetadata> formedQuestionsList = new List<TestStructs.QuestionMetadata>();
            FileReader reader = new FileReader(testTitle);
            List<string> textInLines = reader.GetQuestionLines();
            foreach (string line in textInLines)
            {
                TestStructs.QuestionMetadata tempQuestion = new TestStructs.QuestionMetadata
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

        public static List<TestStructs.TestMetadata> GetAllTestMetadatasByTitles(List<string> transliteratedTitles)
        {
            List<TestStructs.TestMetadata> listToReturn = new List<TestStructs.TestMetadata>();
            foreach (string transliteratedTitle in transliteratedTitles)
            {
                TestStructs.TestMetadata currentTestMetadata = GetTestMetadataByTitle(transliteratedTitle);
                if (!currentTestMetadata.Equals(EmptyTestMetadata))
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
                FileReader reader = new FileReader(testTitle);
                string[] stringToSplit = reader.GetTestMetadataFromFile().Split(new char[] { '₴' }, StringSplitOptions.RemoveEmptyEntries);
                if (stringToSplit.Length < 3) throw new FormatException();

                int.TryParse(stringToSplit[2], out int timerValue);
                TestStructs.TestMetadata readTestMetadata = new TestStructs.TestMetadata()
                {
                    testTitle = stringToSplit[0],
                    lastEditedTime = DateTime.Parse(stringToSplit[1]),
                    timerValue = timerValue
                };

                return readTestMetadata;
            }
            catch (FormatException)
            {
                MessageBox.Show("Помилка! Дані з бази даних некоректні!");
                return EmptyTestMetadata;
            }
        }

    }
}
