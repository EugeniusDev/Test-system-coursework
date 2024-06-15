using System.Collections.Generic;
using System.Windows;
using System;
using static courseWork_project.TestStructs;

namespace courseWork_project.DataManipulation
{
    public static class Searcher
    {
        #region Question search
        public static void ShowQuestionSearchingResults(string wantedValue, List<string> transliteratedTestTitles)
        {
            string searchOutput = "Запитання не знайдено. Перевірте правильність написання та спробуйте ще раз";
            foreach (string testTitle in transliteratedTestTitles)
            {
                var supposedQuestion = FindQuestionFromTestByValue(testTitle, wantedValue);

                bool isQuestionFound = supposedQuestion.question != null;
                if (isQuestionFound)
                {
                    searchOutput = FormQuestionSearchOutput(testTitle, supposedQuestion);
                    break;
                }
            }

            MessageBox.Show(searchOutput, "Результат пошуку запитання тесту");
        }

        public static QuestionMetadata FindQuestionFromTestByValue(string testTitle,
            string wantedQuestion)
        {
            List<QuestionMetadata> questions = DataDecoder
                .GetQuestionMetadatasByTitle(testTitle);
            return questions.Find(a => a.question.ToLower().Equals(wantedQuestion.ToLower()));
        }

        public static string FormQuestionSearchOutput(string testTitle,
            QuestionMetadata questionMetadata)
        {
            return $"Введене запитання знайдено в тесті \"{testTitle}\":\n"
                + $"Запитання: {questionMetadata.question}; "
                + $"Всього варіантів: {questionMetadata.variants.Count}; "
                + $"Правильних варіантів: {questionMetadata.correctVariantsIndeces.Count}\n";
        }
        #endregion
        #region Variant search
        public static void ShowVariantSearchingResults(string wantedValue, List<string> transliteratedTestTitles)
        {
            foreach (string testTitle in transliteratedTestTitles)
            {
                List<QuestionMetadata> currentTestQuestions = DataDecoder
                    .GetQuestionMetadatasByTitle(testTitle);
                foreach (QuestionMetadata questionMetadata in currentTestQuestions)
                {
                    if (IsWantedVariantFound(questionMetadata, wantedValue))
                    {
                        MessageBox.Show(FormVariantSearchOutput(testTitle, questionMetadata),
                            "Результат пошуку запитання тесту");
                        return;
                    }
                }
            }

            MessageBox.Show("Варіант не знайдено. Перевірте правильність написання та спробуйте ще раз",
                "Результат пошуку запитання тесту");
        }

        public static bool IsWantedVariantFound(QuestionMetadata questionMetadata, string wantedVariant)
        {
            string supposedVariant = questionMetadata.variants.Find(v => 
                v.ToLower().Equals(wantedVariant.ToLower()));
            return supposedVariant != null;
        }

        public static string FormVariantSearchOutput(string testTitle, QuestionMetadata questionMetadata)
        {
            return $"Введений варіант знайдено в тесті \"{testTitle}\",\n"
                + $"в запитанні \"{questionMetadata.question}\"";
        }
        #endregion

        public static void ValidateInputForField(string fieldToCheck, string fieldName)
        {
            bool fieldIsEmptyOrDefault = string.IsNullOrWhiteSpace(fieldToCheck)
                || string.Compare(fieldToCheck, $"Пошук {fieldName} тесту") == 0;
            if (fieldIsEmptyOrDefault)
            {
                throw new ArgumentException("Будь ласка, заповніть поле пошуку запитання тесту");
            }
        }
    }
}
