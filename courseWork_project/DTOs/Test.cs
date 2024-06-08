using System.Collections.Generic;

namespace courseWork_project
{
    public class Test
    {
        public List<TestStructs.Question> Questions { get; private set; }
        public TestStructs.TestInfo TestInfo { get; private set; }

        public Test(List<TestStructs.Question> questions, TestStructs.TestInfo testInfo)
        {
            Questions = questions;
            TestInfo = testInfo;
        }
    }
}
