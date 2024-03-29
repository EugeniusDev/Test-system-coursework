using System.Collections.Generic;
using System.Windows;

namespace courseWork_project
{
    public abstract class Sorter
    {
        /// <summary>
        /// Enum для визначення типу сортування тестів
        /// </summary>
        private enum TestSortTypes
        {
            BY_DATE,
            BY_TIMER,
            BY_QUESTIONS_COUNT,
            BY_TITLE
        }
        /// <summary>
        /// Enum для визначення типу сортування запитань тестів
        /// </summary>
        private enum QuestionSortTypes
        {
            BY_VARIANTS_COUNT,
            BY_CORRECT_COUNT,
            BY_QUESTION_LENGTH,
            BY_QUESTION_TITLE
        }
        /// <summary>
        /// Метод, що сортує тести за вказаним типом
        /// </summary>
        /// <param name="typeOfSort">Тип сортування (відповідно до enum TestSortTypes)</param>
        /// <param name="transliteratedTitles">Список всіх назв тестів (транслітерованих)</param>
        public static void SortTests(int typeOfSort, List<string> transliteratedTitles)
        {
            List<Test.TestInfo> testsToSort = DataDecoder.GetAllTestInfos(transliteratedTitles);
            string typeDescription = "Резулат сортування тестів за ";
            // Процес сортування
            switch ((TestSortTypes)typeOfSort)
            {
                case TestSortTypes.BY_DATE:
                    typeDescription = string.Concat(typeDescription, "датою останнього редагування");
                    testsToSort.Sort((a, b) => b.lastEditedTime.CompareTo(a.lastEditedTime));
                    break;
                case TestSortTypes.BY_TIMER:
                    typeDescription = string.Concat(typeDescription, "значенням таймера");
                    testsToSort.Sort((a, b) => a.timerValue.CompareTo(b.timerValue));
                    break;
                case TestSortTypes.BY_QUESTIONS_COUNT:
                    typeDescription = string.Concat(typeDescription, "кількістю запитань");
                    testsToSort.Sort((a, b) =>
                        // Кількість запитань першого порівнюваного тесту
                        DataDecoder.FormQuestionsList(a.testTitle).Count
                        .CompareTo(
                            // Кількість запитань другого порівнюваного тесту
                            DataDecoder.FormQuestionsList(b.testTitle).Count)
                        );
                    break;
                case TestSortTypes.BY_TITLE:
                    typeDescription = string.Concat(typeDescription, "назвою (лексикографічний порядок)");
                    testsToSort.Sort((a, b) => a.testTitle.CompareTo(b.testTitle));
                    break;
                default:
                    MessageBox.Show("Обрано некоректний тип сортування тестів");
                    break;
            }

            // Формування виводу просортованої інформації
            string resultOfSort = string.Empty;
            foreach(Test.TestInfo currTestInfo in testsToSort)
            {
                resultOfSort = string.Concat(resultOfSort, $"\nНазва: {currTestInfo.testTitle}; " +
                    $"Дата: {currTestInfo.lastEditedTime}; " +
                    $"Таймер: {currTestInfo.timerValue} хв; " +
                    $"Кількість запитань: {DataDecoder.FormQuestionsList(currTestInfo.testTitle).Count}\n");
            }
            MessageBox.Show(resultOfSort, typeDescription);
        }
        /// <summary>
        /// Метод, що сортує запитання тестів за вказаним типом
        /// </summary>
        /// <param name="typeOfSort">Тип сортування (відповідно до enum QuestionSortTypes)</param>
        /// <param name="transliteratedTitles">Список всіх назв тестів (транслітерованих)</param>
        public static void SortQuestions(int typeOfSort, List<string> transliteratedTitles)
        {
            List<Test.Question> questionsToSort = DataDecoder.GetAllQuestions(transliteratedTitles);
            string typeDescription = "Резулат сортування запитань тестів за ";
            // Процес сортування
            switch ((QuestionSortTypes)typeOfSort)
            {
                case QuestionSortTypes.BY_VARIANTS_COUNT:
                    typeDescription = string.Concat(typeDescription, "кількістю варіантів відповідей");
                    questionsToSort.Sort((a, b) => a.variants.Count.CompareTo(b.variants.Count));
                    break;
                case QuestionSortTypes.BY_CORRECT_COUNT:
                    typeDescription = string.Concat(typeDescription, "кількістю правильних відповідей");
                    questionsToSort.Sort((a, b) => a.correctVariantsIndexes.Count.CompareTo(b.correctVariantsIndexes.Count));
                    break;
                case QuestionSortTypes.BY_QUESTION_LENGTH:
                    typeDescription = string.Concat(typeDescription, "довжиною запитання");
                    questionsToSort.Sort((a, b) => a.question.Length.CompareTo(b.question.Length));
                    break;
                case QuestionSortTypes.BY_QUESTION_TITLE:
                    typeDescription = string.Concat(typeDescription, "запитанням (лексикографічний порядок)");
                    questionsToSort.Sort((a, b) => a.question.CompareTo(b.question));
                    break;
                default:
                    MessageBox.Show("Обрано некоректний тип сортування запитань тестів");
                    break;
            }
            // Формування виводу просортованої інформації
            string resultOfSort = string.Empty;
            foreach (Test.Question currQuestion in questionsToSort)
            {
                resultOfSort = string.Concat(resultOfSort, $"\nЗапитання: {currQuestion.question}; " +
                    $"Всього варіантів: {currQuestion.variants.Count}; " +
                    $"Правильних варіантів: {currQuestion.correctVariantsIndexes.Count}\n");
            }
            MessageBox.Show(resultOfSort, typeDescription);
        }
    }
}
