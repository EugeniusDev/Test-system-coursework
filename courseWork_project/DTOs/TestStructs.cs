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
        public struct Question
        {
            /// <summary>
            /// Question itself
            /// </summary>
            public string question;
            /// <summary>
            /// List of answer variants
            /// </summary>
            public List<string> variants;
            /// <summary>
            /// List of correct answer variants indeces
            /// </summary>
            public List<int> correctVariantsIndeces;
            /// <summary>
            /// This question has linked image?
            /// </summary>
            public bool hasLinkedImage;
        }
        /// <summary>
        /// General test's info structure
        /// </summary>
        public struct TestInfo
        {
            /// <summary>
            /// Test title
            /// </summary>
            public string testTitle;
            /// <summary>
            /// Time of last editing of a test
            /// </summary>
            public DateTime lastEditedTime;
            /// <summary>
            /// Tiemr value
            /// </summary>
            /// <remarks>0, if no restrictions in time</remarks>
            public int timerValue;
        }
    }
}
