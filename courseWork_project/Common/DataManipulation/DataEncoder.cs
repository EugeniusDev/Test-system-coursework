﻿using System.Collections.Generic;
using System.Linq;
using static courseWork_project.TestStructs;

namespace courseWork_project
{
    public static class DataEncoder
    {
        private static readonly char separator = Properties.Settings.Default.dataSeparator;

        public static List<string> EncodeToLines(this Test testToEncode)
        {
            List<string> encodedTest = new List<string>
            {
                EncodeTestMetadata(testToEncode.TestMetadata)
            };
            foreach (QuestionMetadata questionMetadata in testToEncode.QuestionMetadatas)
            {
                encodedTest.Add(EncodeQuestionMetadata(questionMetadata));
            }

            return encodedTest;
        }

        private static string EncodeTestMetadata(TestMetadata testMetadata)
        {
            return $"{testMetadata.testTitle}{separator}" +
                $"{testMetadata.lastEditedTime}{separator}" +
                $"{testMetadata.timerValueInMinutes}";
        }

        private static string EncodeQuestionMetadata(QuestionMetadata questionMetadata)
        {
            string encodedMetadata = RemoveSplitCharacter(questionMetadata.question);
            encodedMetadata = string.Concat(encodedMetadata, separator);
            encodedMetadata = string.Concat(encodedMetadata, string.Join(separator.ToString(), 
                questionMetadata.variants.Select(variant => variant.RemoveSplitCharacter())));
            encodedMetadata = string.Concat(encodedMetadata, separator);
            encodedMetadata = string.Concat(encodedMetadata, string.Join(separator.ToString(), 
                questionMetadata.correctVariantsIndeces.Select(index => index.ToString())));
            encodedMetadata = string.Concat(encodedMetadata, $"{separator}{questionMetadata.linkedImagePath}");
            
            return encodedMetadata;
        }

        private static string RemoveSplitCharacter(this string stringToRemoveFrom)
        {
            return stringToRemoveFrom.Contains(separator) ? 
                stringToRemoveFrom.Replace(separator.ToString(), "") : stringToRemoveFrom;
        }
    }
}