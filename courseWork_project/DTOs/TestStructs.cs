using System;
using System.Collections.Generic;

namespace courseWork_project
{
    /// <summary>
    /// Class-container for test-related structures
    /// </summary>
    public static class TestStructs
    {
        public struct QuestionMetadata
        {
            public string question;
            public List<string> variants;
            public List<int> correctVariantsIndeces;
            public bool hasLinkedImage;
        }
        public readonly static QuestionMetadata EmptyQuestionMetadata = new QuestionMetadata
        {
            variants = new List<string>(),
            correctVariantsIndeces = new List<int>()
        };

        public struct TestMetadata
        {
            public string testTitle;
            public DateTime lastEditedTime;

            // 0, if no restrictions in time
            public int timerValue;
        }

        /// <summary>
        /// TestMetadata with sample data
        /// </summary>
        public readonly static TestMetadata EmptyTestMetadata = new TestMetadata()
        {
            testTitle = "empty",
            lastEditedTime = DateTime.Now,
            timerValue = 0
        };
    }
}
