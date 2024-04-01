using System.Collections.Generic;
using System.Linq;

namespace courseWork_project
{
    /// <summary>
    /// Клас, методи якого закодовують інформацію з структур в рядок для запису в базу даних
    /// </summary>
    public static class DataEncoder
    {
        /// <summary>
        /// Отримує структури з даними про тест, формує список рядків цих даних
        /// </summary>
        /// <remarks>Використовує ₴ як оператор розділення</remarks>
        /// <param name="testInfo">Структура, що містить інформацію про тест</param>
        /// <param name="questionsListToDecode">Список структур, що містять інформацію про запитання</param>
        /// <returns>Список рядків даних</returns>
        public static List<string> EncodeAndReturnLines(TestStructs.TestInfo testInfo, List<TestStructs.Question> questionsListToDecode)
        {
            List<string> stringListToReturn = new List<string>
            {
                $"{testInfo.testTitle}₴{testInfo.lastEditedTime}₴{testInfo.timerValue}"
            };
            string tempStringToForm = string.Empty;
            foreach (TestStructs.Question question in questionsListToDecode)
            {
                // Кожен рядок проходить перевірку на символ "₴"
                tempStringToForm = RemoveSplitCharacters(question.question);
                tempStringToForm = string.Concat(tempStringToForm, "₴");
                tempStringToForm = string.Concat(tempStringToForm, string.Join("₴", question.variants.Select(variant => RemoveSplitCharacters(variant))));
                tempStringToForm = string.Concat(tempStringToForm, "₴");
                tempStringToForm = string.Concat(tempStringToForm, string.Join("₴", question.correctVariantsIndexes.Select(index => index.ToString())));
                tempStringToForm = string.Concat(tempStringToForm, $"₴{question.hasLinkedImage}");
                stringListToReturn.Add(tempStringToForm);
            }
            return stringListToReturn;
        }
        /// <summary>
        /// Метод, що при наявності в рядку символу "₴" заміняє його на "грн"
        /// </summary>
        /// <remarks>Використовується для подальшого коректного розділення рядка на структури даних</remarks>
        /// <param name="stringToRemoveFrom">Рядок, який потрібно перевірити</param>
        /// <returns>Модифікований рядок або той самий, якщо він не містить "₴"</returns>
        private static string RemoveSplitCharacters(string stringToRemoveFrom)
        {
            return stringToRemoveFrom.Contains("₴") ? stringToRemoveFrom.Replace("₴", "грн") : stringToRemoveFrom;
        }
    }
}