using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace courseWork_project
{
    public static class Sorter
    {
        private static readonly Dictionary<TestSortOption, string> testSortDescriptions = new Dictionary<TestSortOption, string>()
        {
            { TestSortOption.BY_DATE, "датою останнього редагування" },
            { TestSortOption.BY_TIMER, "значенням таймера" },
            { TestSortOption.BY_QUESTIONS_COUNT, "кількістю запитань" },
            { TestSortOption.BY_TITLE, "назвою (лексикографічний порядок)" }
        };

        public static void ShowSortedTests(TestSortOption sortingOption, List<string> transliteratedTitles)
        {
            List<TestStructs.TestMetadata> sortedTests = GetSortedTests(sortingOption, transliteratedTitles);
            string resultOfSort = GetTestsSortingResults(sortedTests);
            string sortTypeInfo = $"Результат сортування тестів за {testSortDescriptions[sortingOption]}";
            ShowSortingResults(resultOfSort, sortTypeInfo);
        }

        private static List<TestStructs.TestMetadata> GetSortedTests(TestSortOption sortingOption, List<string> transliteratedTitles)
        {
            List<TestStructs.TestMetadata> allTests = DataDecoder.GetAllTestMetadatasByTitles(transliteratedTitles);
            switch (sortingOption)
            {
                case TestSortOption.BY_DATE:
                    allTests.Sort((a, b) => b.lastEditedTime
                    .CompareTo(a.lastEditedTime));
                    break;
                case TestSortOption.BY_TIMER:
                    allTests.Sort((a, b) => a.timerValueInMinutes
                    .CompareTo(b.timerValueInMinutes));
                    break;
                case TestSortOption.BY_QUESTIONS_COUNT:
                    allTests.Sort((a, b) =>
                        DataDecoder.GetQuestionMetadatasByTitle(a.testTitle).Count
                        .CompareTo(DataDecoder.GetQuestionMetadatasByTitle(b.testTitle).Count)
                        );
                    break;
                case TestSortOption.BY_TITLE:
                    allTests.Sort((a, b) => a.testTitle
                    .CompareTo(b.testTitle));
                    break;
            }

            return allTests;
        }

        private static string GetTestsSortingResults(List<TestStructs.TestMetadata> sortedTests)
        {
            if (sortedTests.Count == 0)
            {
                return string.Empty;
            }

            StringBuilder sortResults = new StringBuilder(string.Empty);
            foreach (TestStructs.TestMetadata testMetadata in sortedTests)
            {
                int questionsCount = DataDecoder.GetQuestionMetadatasByTitle(testMetadata.testTitle)
                    .Count;
                sortResults.AppendLine($"Назва: {testMetadata.testTitle}; " +
                    $"Дата: {testMetadata.lastEditedTime}; " +
                    $"Таймер: {testMetadata.timerValueInMinutes} хв; " +
                    $"Кількість запитань: {questionsCount}\n");
            }

            return sortResults.ToString();
        }

        private static readonly Dictionary<QuestionSortOption, string> questionSortDescriptions = new Dictionary<QuestionSortOption, string>()
        {
            { QuestionSortOption.BY_VARIANTS_COUNT, "кількістю варіантів відповідей" },
            { QuestionSortOption.BY_CORRECT_COUNT, "кількістю правильних відповідей" },
            { QuestionSortOption.BY_QUESTION_LENGTH, "довжиною запитання" },
            { QuestionSortOption.BY_QUESTION_TITLE, "запитанням (лексикографічний порядок)" }
        };

        public static void ShowSortedQuestions(QuestionSortOption sortingOption, List<string> transliteratedTitles)
        {
            List<TestStructs.QuestionMetadata> sortedQuestions = GetSortedQuestions(sortingOption, transliteratedTitles);
            string resultOfSort = GetQuestionsSortingResults(sortedQuestions);
            string sortTypeInfo = $"Резулат сортування запитань тестів за {questionSortDescriptions[sortingOption]}";
            ShowSortingResults(resultOfSort, sortTypeInfo);
        }

        private static List<TestStructs.QuestionMetadata> GetSortedQuestions(QuestionSortOption sortingOption, List<string> transliteratedTitles)
        {
            List<TestStructs.QuestionMetadata> allQuestions = DataDecoder.GetAllQuestionsByTestTitles(transliteratedTitles);
            switch (sortingOption)
            {
                case QuestionSortOption.BY_VARIANTS_COUNT:
                    allQuestions.Sort((a, b) => a.variants.Count
                    .CompareTo(b.variants.Count));
                    break;
                case QuestionSortOption.BY_CORRECT_COUNT:
                    allQuestions.Sort((a, b) => a.correctVariantsIndeces.Count
                    .CompareTo(b.correctVariantsIndeces.Count));
                    break;
                case QuestionSortOption.BY_QUESTION_LENGTH:
                    allQuestions.Sort((a, b) => a.question.Length
                    .CompareTo(b.question.Length));
                    break;
                case QuestionSortOption.BY_QUESTION_TITLE:
                    allQuestions.Sort((a, b) => a.question.CompareTo(b.question));
                    break;
            }

            return allQuestions;
        }

        private static string GetQuestionsSortingResults(List<TestStructs.QuestionMetadata> sortedQuestions)
        {
            if (sortedQuestions.Count == 0)
            {
                return string.Empty;
            }

            StringBuilder sortResults = new StringBuilder(string.Empty);
            foreach (TestStructs.QuestionMetadata currQuestion in sortedQuestions)
            {
                sortResults.AppendLine($"Запитання: {currQuestion.question}; " +
                    $"Всього варіантів: {currQuestion.variants.Count}; " +
                    $"Правильних варіантів: {currQuestion.correctVariantsIndeces.Count}\n");
            }

            return sortResults.ToString();
        }

        private static void ShowSortingResults(string result, string typeDescription)
        {
            if (result == string.Empty)
            {
                MessageBox.Show("Немає що сортувати", typeDescription);
                return;
            }

            MessageBox.Show(result, typeDescription);
        }
    }
}
