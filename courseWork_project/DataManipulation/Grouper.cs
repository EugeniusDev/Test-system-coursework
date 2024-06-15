using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace courseWork_project
{
    public static class Grouper
    {
        private static readonly Dictionary<TestGroupOption, string> testGroupDescriptions = new Dictionary<TestGroupOption, string>()
        {
            { TestGroupOption.TIMER_EXISTANCE, "час проходження яких обмежений" },
            { TestGroupOption.EDITED_TODAY, "що були відредаговані сьогодні" },
            { TestGroupOption.CONTROL_WORK, "що є контрольними роботами" }
        };

        public static void ShowTestsGroup(TestGroupOption groupingOption, List<string> transliteratedTitles)
        {
            List<TestStructs.TestMetadata> testsToGroup = DataDecoder.GetAllTestMetadatasByTitles(transliteratedTitles);
            List<TestStructs.TestMetadata> groupedTests = GetGroupOfTests(groupingOption, testsToGroup);
            string resultOfGrouping = GetTestsGroupingResults(groupedTests);
            string typeDescription = $"Група тестів, {testGroupDescriptions[groupingOption]}";
            ShowGroupingResults(resultOfGrouping, typeDescription);
        }

        private static List<TestStructs.TestMetadata> GetGroupOfTests(TestGroupOption groupingOption, List<TestStructs.TestMetadata> testsToGroup)
        {
            List<TestStructs.TestMetadata> groupOfTests = new List<TestStructs.TestMetadata>();
            switch (groupingOption)
            {
                case TestGroupOption.TIMER_EXISTANCE:
                    groupOfTests = testsToGroup.FindAll(a => a.timerValue != 0);
                    break;
                case TestGroupOption.EDITED_TODAY:
                    groupOfTests = testsToGroup
                        .FindAll(a => a.lastEditedTime.Date == DateTime.Today);
                    break;
                case TestGroupOption.CONTROL_WORK:
                    groupOfTests = testsToGroup
                        .FindAll(a => a.testTitle.ToLower().Contains("контрольна робота"));
                    break;
            }

            return groupOfTests;
        }

        private static string GetTestsGroupingResults(List<TestStructs.TestMetadata> groupUfTests)
        {
            if (groupUfTests.Count == 0)
            {
                return string.Empty;
            }

            StringBuilder groupingResult = new StringBuilder(string.Empty);
            foreach (TestStructs.TestMetadata currentTestMetadatas in groupUfTests)
            {
                groupingResult.AppendLine($"Назва: {currentTestMetadatas.testTitle}; " +
                    $"Дата: {currentTestMetadatas.lastEditedTime}; " +
                    $"Таймер: {currentTestMetadatas.timerValue} хв\n");
            }

            return groupingResult.ToString();
        }


        private static readonly Dictionary<QuestionGroupOption, string> questionGroupDescriptions = new Dictionary<QuestionGroupOption, string>()
        {
            { QuestionGroupOption.WITH_IMAGE, "до яких додано ілюстрацію" },
            { QuestionGroupOption.ALL_VARIANTS_CORRECT, "всі варіанти яких правильні" }
        };

        public static void ShowQuestionsGroup(QuestionGroupOption groupingOption, List<string> transliteratedTitles)
        {
            List<TestStructs.QuestionMetadata> questionsToGroup = DataDecoder.GetAllQuestionsByTestTitles(transliteratedTitles);
            List<TestStructs.QuestionMetadata> groupedQuestions = GetGroupOfQuestions(groupingOption, questionsToGroup);
            string resultOfGrouping = GetQuestionsGroupingResults(groupedQuestions);
            string typeDescription = $"Група запитань тесту, {questionGroupDescriptions[groupingOption]}";
            ShowGroupingResults(resultOfGrouping, typeDescription);
        }

        private static List<TestStructs.QuestionMetadata> GetGroupOfQuestions(QuestionGroupOption groupingOption, List<TestStructs.QuestionMetadata> questionsToGroup)
        {
            List<TestStructs.QuestionMetadata> groupOfQuestions = new List<TestStructs.QuestionMetadata>();
            if (groupingOption.Equals(QuestionGroupOption.WITH_IMAGE))
            {
                groupOfQuestions = questionsToGroup.FindAll(a => a.hasLinkedImage);
            }
            else if (groupingOption.Equals(QuestionGroupOption.ALL_VARIANTS_CORRECT))
            {
                groupOfQuestions = questionsToGroup
                    .FindAll(a => a.variants.Count == a.correctVariantsIndeces.Count);
            }

            return groupOfQuestions;
        }

        private static string GetQuestionsGroupingResults(List<TestStructs.QuestionMetadata> groupOfQuestions)
        {
            if (groupOfQuestions.Count == 0)
            {
                return string.Empty;
            }

            StringBuilder groupingResult = new StringBuilder(string.Empty);
            foreach (TestStructs.QuestionMetadata questionMetadata in groupOfQuestions)
            {
                groupingResult.AppendLine($"Запитання: {questionMetadata.question}; " +
                    $"Всього варіантів: {questionMetadata.variants.Count}; " +
                    $"Правильних варіантів: {questionMetadata.correctVariantsIndeces.Count}; ");
                if (questionMetadata.hasLinkedImage)
                {
                    groupingResult.Append("Містить ілюстрацію\n");
                }
                else
                {
                    groupingResult.Append("Не містить ілюстрацію\n");
                }
            }

            return groupingResult.ToString();
        }

        private static void ShowGroupingResults(string result, string typeDescription)
        {
            if (result == string.Empty)
            {
                MessageBox.Show("Немає що групувати", typeDescription);
                return;
            }

            MessageBox.Show(result, typeDescription);
        }
    }
}
