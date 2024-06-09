using System;
using System.Collections.Generic;

namespace courseWork_project
{
    /// <summary>
    /// Class-container for test-related structures
    /// </summary>
    public static partial class TestStructs
    {
        /// <summary>
        /// Structure of question of a test
        /// </summary>
        public struct QuestionMetadata
        {
            public string question;
            public List<string> variants;
            public List<int> correctVariantsIndeces;
            public bool hasLinkedImage;
        }
        /// <summary>
        /// General test's info structure
        /// </summary>
        public struct TestMetadata
        {
            public string testTitle;
            public DateTime lastEditedTime;
            /// <summary>
            /// Tiemr value
            /// </summary>
            /// <remarks>0, if no restrictions in time</remarks>
            public int timerValue;
        }
    }
}
