﻿using System.Collections.Generic;
using System.Windows;

namespace courseWork_project
{
    public static class Sorter
    {
        /// <summary>
        /// Sorts tests in a given manner
        /// </summary>
        /// <param name="typeOfSort">Type of sorting (according to enum TestSortTypes)</param>
        /// <param name="transliteratedTitles">List of transliterated test titles</param>
        public static void SortTests(int typeOfSort, List<string> transliteratedTitles)
        {
            List<TestStructs.TestMetadata> testsToSort = DataDecoder.GetAllTestMetadatasByTitles(transliteratedTitles);
            string typeDescription = "Резулат сортування тестів за ";
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
                        DataDecoder.GetQuestionMetadatasByTitle(a.testTitle).Count
                        .CompareTo(DataDecoder.GetQuestionMetadatasByTitle(b.testTitle).Count)
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
            // Forming the output of sorted stuff
            string resultOfSort = string.Empty;
            foreach(TestStructs.TestMetadata currentTestMetadata in testsToSort)
            {
                resultOfSort = string.Concat(resultOfSort, $"\nНазва: {currentTestMetadata.testTitle}; " +
                    $"Дата: {currentTestMetadata.lastEditedTime}; " +
                    $"Таймер: {currentTestMetadata.timerValue} хв; " +
                    $"Кількість запитань: {DataDecoder.GetQuestionMetadatasByTitle(currentTestMetadata.testTitle).Count}\n");
            }

            ShowSortingResults(resultOfSort, typeDescription);
        }
        /// <summary>
        /// Sorts QuestionMetadatas in a given manner
        /// </summary>
        /// <param name="typeOfSort">Type of sorting (according to enum QuestionSortTypes)</param>
        /// <param name="transliteratedTitles">List of transliterated test titles</param>
        public static void SortQuestions(int typeOfSort, List<string> transliteratedTitles)
        {
            List<TestStructs.QuestionMetadata> questionsToSort = DataDecoder.GetAllQuestionsByTestTitles(transliteratedTitles);
            string typeDescription = "Резулат сортування запитань тестів за ";
            switch ((QuestionSortTypes)typeOfSort)
            {
                case QuestionSortTypes.BY_VARIANTS_COUNT:
                    typeDescription = string.Concat(typeDescription, "кількістю варіантів відповідей");
                    questionsToSort.Sort((a, b) => a.variants.Count.CompareTo(b.variants.Count));
                    break;
                case QuestionSortTypes.BY_CORRECT_COUNT:
                    typeDescription = string.Concat(typeDescription, "кількістю правильних відповідей");
                    questionsToSort.Sort((a, b) => a.correctVariantsIndeces.Count.CompareTo(b.correctVariantsIndeces.Count));
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
            // Forming the output of sorted stuff
            string resultOfSort = string.Empty;
            foreach (TestStructs.QuestionMetadata currQuestion in questionsToSort)
            {
                resultOfSort = string.Concat(resultOfSort, $"\nЗапитання: {currQuestion.question}; " +
                    $"Всього варіантів: {currQuestion.variants.Count}; " +
                    $"Правильних варіантів: {currQuestion.correctVariantsIndeces.Count}\n");
            }

            ShowSortingResults(resultOfSort, typeDescription);
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