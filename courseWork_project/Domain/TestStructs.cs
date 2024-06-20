using System;
using System.Collections.Generic;

namespace courseWork_project
{
    public static class TestStructs
    {
        public struct QuestionMetadata
        {
            public string question;
            public List<string> variants;
            public List<int> correctVariantsIndeces;
            public string linkedImagePath;
        }

        public struct TestMetadata
        {
            public string testTitle;
            public DateTime lastEditedTime;
            public int timerValueInMinutes;
        }
        public readonly static TestMetadata EmptyTestMetadata = new TestMetadata()
        {
            testTitle = string.Empty,
            lastEditedTime = DateTime.Now,
            // No restrictions in time by default
            timerValueInMinutes = 0
        };
    }
}
