using System.Collections.Generic;

namespace courseWork_project
{
    public class Test
    {
        public List<TestStructs.QuestionMetadata> QuestionMetadatas { get; private set; }
        public TestStructs.TestMetadata TestMetadata { get; private set; }

        public Test(List<TestStructs.QuestionMetadata> questionMetadatas, TestStructs.TestMetadata testMetadata)
        {
            QuestionMetadatas = questionMetadatas;
            TestMetadata = testMetadata;
        }
    }
}
