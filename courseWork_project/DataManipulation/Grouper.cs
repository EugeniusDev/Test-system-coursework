using System;
using System.Collections.Generic;
using System.Windows;

namespace courseWork_project
{
    public static class Grouper
    {
        /// <summary>
        /// Groups tests by specified type
        /// </summary>
        /// <param name="groupingType">Type of grouping (according to enum TestGroupTypes)</param>
        /// <param name="transliteratedTitles">List of transliterated test titles</param>
        public static void GroupTests(int groupingType, List<string> transliteratedTitles)
        {
            List<TestStructs.TestInfo> testsToGroup = DataDecoder.GetAllExistingTestInfosByTitles(transliteratedTitles);
            List<TestStructs.TestInfo> groupOfTests = new List<TestStructs.TestInfo>();
            string typeDescription = "Група тестів, ";
            switch ((TestGroupTypes)groupingType)
            {
                case TestGroupTypes.TIMER_EXISTANCE:
                    typeDescription = string.Concat(typeDescription, "час проходження яких обмежений");
                    groupOfTests = testsToGroup.FindAll(a => a.timerValue != 0);
                    break;
                case TestGroupTypes.EDITED_TODAY:
                    typeDescription = string.Concat(typeDescription, "що були відредаговані сьогодні");
                    groupOfTests = testsToGroup.FindAll(a => a.lastEditedTime.Date == DateTime.Today);
                    break;
                case TestGroupTypes.CONTROL_WORK:
                    typeDescription = string.Concat(typeDescription, "що є контрольними роботами");
                    groupOfTests = testsToGroup.FindAll(a => a.testTitle.ToLower().Contains("контрольна робота"));
                    break;
                default:
                    MessageBox.Show("Обрано некоректний тип групування тестів");
                    break;
            }
            // Forming output of grouped stuff
            string resultOfGrouping = string.Empty;
            foreach (TestStructs.TestInfo currTestInfo in groupOfTests)
            {
                resultOfGrouping = string.Concat(resultOfGrouping, $"\nНазва: {currTestInfo.testTitle}; " +
                    $"Дата: {currTestInfo.lastEditedTime}; " +
                    $"Таймер: {currTestInfo.timerValue} хв\n");
            }

            ShowGroupingResults(resultOfGrouping, typeDescription);
        }
        /// <summary>
        /// Groups Questions by specified type
        /// </summary>
        /// <param name="groupingType">Type of grouping (according to enum QuestionGroupTypes)</param>
        /// <param name="transliteratedTitles">List of transliterated test titles</param>
        public static void GroupQuestions(int groupingType, List<string> transliteratedTitles)
        {
            List<TestStructs.Question> questionsToGroup = DataDecoder.GetAllExistingQuestionsByTestTitles(transliteratedTitles);
            List<TestStructs.Question> groupOfQuestions = new List<TestStructs.Question>();
            string typeDescription = "Група запитань тесту, ";
            switch ((QuestionGroupTypes)groupingType)
            {
                case QuestionGroupTypes.WITH_IMAGE:
                    typeDescription = string.Concat(typeDescription, "до яких додано ілюстрацію");
                    groupOfQuestions = questionsToGroup.FindAll(a => a.hasLinkedImage);
                    break;
                case QuestionGroupTypes.ALL_VARIANTS_CORRECT:
                    typeDescription = string.Concat(typeDescription, "всі варіанти яких правильні");
                    groupOfQuestions = questionsToGroup.FindAll(a => a.variants.Count == a.correctVariantsIndeces.Count);
                    break;
                default:
                    MessageBox.Show("Обрано некоректний тип групування запитань тестів");
                    break;
            }
            // Forming output of grouped stuff
            string resultOfGrouping = string.Empty;
            foreach (TestStructs.Question currQuestion in groupOfQuestions)
            {
                resultOfGrouping = string.Concat(resultOfGrouping, $"\nЗапитання: {currQuestion.question}; " +
                    $"Всього варіантів: {currQuestion.variants.Count}; " +
                    $"Правильних варіантів: {currQuestion.correctVariantsIndeces.Count}; ");
                resultOfGrouping = currQuestion.hasLinkedImage ?
                    string.Concat(resultOfGrouping, "Містить ілюстрацію\n")
                    : string.Concat(resultOfGrouping, "Не містить ілюстрацію\n");
            }

            ShowGroupingResults(resultOfGrouping, typeDescription);
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
