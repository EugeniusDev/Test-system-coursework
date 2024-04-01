using System;
using System.Collections.Generic;
using System.Windows;

namespace courseWork_project
{
    public static class Grouper
    {
        /// <summary>
        /// Метод, що групує тести за вказаним типом
        /// </summary>
        /// <param name="groupingType">Тип групування (відповідно до enum TestGroupTypes)</param>
        /// <param name="transliteratedTitles">Список всіх назв тестів (транслітерованих)</param>
        public static void GroupTests(int groupingType, List<string> transliteratedTitles)
        {
            List<TestStructs.TestInfo> testsToGroup = DataDecoder.GetAllTestInfos(transliteratedTitles);
            List<TestStructs.TestInfo> groupOfTests = new List<TestStructs.TestInfo>();
            string typeDescription = "Група тестів, ";
            // Процес групування
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

            // Формування виводу згрупованої інформації
            string resultOfGrouping = string.Empty;
            foreach (TestStructs.TestInfo currTestInfo in groupOfTests)
            {
                resultOfGrouping = string.Concat(resultOfGrouping, $"\nНазва: {currTestInfo.testTitle}; " +
                    $"Дата: {currTestInfo.lastEditedTime}; " +
                    $"Таймер: {currTestInfo.timerValue} хв\n");
            }
            MessageBox.Show(resultOfGrouping, typeDescription);
        }
        /// <summary>
        /// Метод, що групує запитання тестів за вказаним типом
        /// </summary>
        /// <param name="groupingType">Тип групування (відповідно до enum QuestionGroupTypes)</param>
        /// <param name="transliteratedTitles">Список всіх назв тестів (транслітерованих)</param>
        public static void GroupQuestions(int groupingType, List<string> transliteratedTitles)
        {
            List<TestStructs.Question> questionsToGroup = DataDecoder.GetAllQuestions(transliteratedTitles);
            List<TestStructs.Question> groupOfQuestions = new List<TestStructs.Question>();
            string typeDescription = "Група запитань тесту, ";
            // Процес групування
            switch ((QuestionGroupTypes)groupingType)
            {
                case QuestionGroupTypes.WITH_IMAGE:
                    typeDescription = string.Concat(typeDescription, "до яких додано ілюстрацію");
                    groupOfQuestions = questionsToGroup.FindAll(a => a.hasLinkedImage);
                    break;
                case QuestionGroupTypes.ALL_VARIANTS_CORRECT:
                    typeDescription = string.Concat(typeDescription, "всі варіанти яких правильні");
                    groupOfQuestions = questionsToGroup.FindAll(a => a.variants.Count == a.correctVariantsIndexes.Count);
                    break;
                default:
                    MessageBox.Show("Обрано некоректний тип групування запитань тестів");
                    break;
            }
            // Формування виводу згрупованої інформації
            string resultOfGrouping = string.Empty;
            foreach (TestStructs.Question currQuestion in groupOfQuestions)
            {
                resultOfGrouping = string.Concat(resultOfGrouping, $"\nЗапитання: {currQuestion.question}; " +
                    $"Всього варіантів: {currQuestion.variants.Count}; " +
                    $"Правильних варіантів: {currQuestion.correctVariantsIndexes.Count}; ");
                resultOfGrouping = currQuestion.hasLinkedImage ?
                    string.Concat(resultOfGrouping, "Містить ілюстрацію\n")
                    : string.Concat(resultOfGrouping, "Не містить ілюстрацію\n");
            }
            MessageBox.Show(resultOfGrouping, typeDescription);
        }
    }
}
