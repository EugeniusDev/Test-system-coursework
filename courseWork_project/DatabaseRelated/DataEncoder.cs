using System.Collections.Generic;
using System.Linq;

namespace courseWork_project
{
    /// <summary>
    /// Class to encode info from structures into strings
    /// </summary>
    public static class DataEncoder
    {
        /// <summary>
        /// Forms a list of strings from TestInfo structure
        /// </summary>
        /// <remarks>Uses ₴ as separator operator</remarks>
        /// <param name="testInfo">TestInfo structure</param>
        /// <param name="questionsListToDecode">List of question structures</param>
        /// <returns>List of string infos</returns>
        public static List<string> EncodeAndReturnLines(TestStructs.TestInfo testInfo, List<TestStructs.Question> questionsListToDecode)
        {
            List<string> stringListToReturn = new List<string>
            {
                $"{testInfo.testTitle}₴{testInfo.lastEditedTime}₴{testInfo.timerValue}"
            };
            string tempStringToForm = string.Empty;
            foreach (TestStructs.Question question in questionsListToDecode)
            {
                tempStringToForm = RemoveSplitCharacters(question.question);
                tempStringToForm = string.Concat(tempStringToForm, "₴");
                tempStringToForm = string.Concat(tempStringToForm, string.Join("₴", question.variants.Select(variant => RemoveSplitCharacters(variant))));
                tempStringToForm = string.Concat(tempStringToForm, "₴");
                tempStringToForm = string.Concat(tempStringToForm, string.Join("₴", question.correctVariantsIndeces.Select(index => index.ToString())));
                tempStringToForm = string.Concat(tempStringToForm, $"₴{question.hasLinkedImage}");
                stringListToReturn.Add(tempStringToForm);
            }
            return stringListToReturn;
        }
        /// <summary>
        /// Replaces "₴" by "грн"
        /// </summary>
        /// <remarks>Used for correct encoding of structures</remarks>
        /// <param name="stringToRemoveFrom">String to check for separator symbol</param>
        /// <returns>Modified or same string</returns>
        private static string RemoveSplitCharacters(string stringToRemoveFrom)
        {
            return stringToRemoveFrom.Contains("₴") ? stringToRemoveFrom.Replace("₴", "грн") : stringToRemoveFrom;
        }
    }
}