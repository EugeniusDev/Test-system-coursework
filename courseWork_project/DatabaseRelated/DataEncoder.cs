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
        /// Forms a list of strings from TestMetadata structure
        /// </summary>
        /// <remarks>Uses ₴ as separator operator</remarks>
        /// <param name="testMetadata">TestMetadata structure</param>
        /// <param name="questionsListToDecode">List of questionMetadata structures</param>
        /// <returns>List of string infos</returns>
        public static List<string> EncodeAndReturnLines(TestStructs.TestMetadata testMetadata, List<TestStructs.QuestionMetadata> questionsListToDecode)
        {
            List<string> stringListToReturn = new List<string>
            {
                $"{testMetadata.testTitle}₴{testMetadata.lastEditedTime}₴{testMetadata.timerValue}"
            };
            string tempStringToForm = string.Empty;
            foreach (TestStructs.QuestionMetadata questionMetadata in questionsListToDecode)
            {
                tempStringToForm = ReplaceSplitCharacterWithRepresentation(questionMetadata.question);
                tempStringToForm = string.Concat(tempStringToForm, "₴");
                tempStringToForm = string.Concat(tempStringToForm, string.Join("₴", questionMetadata.variants.Select(variant => ReplaceSplitCharacterWithRepresentation(variant))));
                tempStringToForm = string.Concat(tempStringToForm, "₴");
                tempStringToForm = string.Concat(tempStringToForm, string.Join("₴", questionMetadata.correctVariantsIndeces.Select(index => index.ToString())));
                tempStringToForm = string.Concat(tempStringToForm, $"₴{questionMetadata.hasLinkedImage}");
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
        private static string ReplaceSplitCharacterWithRepresentation(this string stringToRemoveFrom)
        {
            return stringToRemoveFrom.Contains("₴") ? stringToRemoveFrom.Replace("₴", "грн") : stringToRemoveFrom;
        }
    }
}