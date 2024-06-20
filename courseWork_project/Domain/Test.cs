using System.Collections.Generic;
using static courseWork_project.TestStructs;

namespace courseWork_project
{
    public class Test
    {
        public TestMetadata TestMetadata { get; private set; }
        public List<QuestionMetadata> QuestionMetadatas { get; private set; }
        public Test(TestMetadata testMetadata, List<QuestionMetadata> questionMetadatas)
        {
            TestMetadata = testMetadata;
            QuestionMetadatas = questionMetadatas;
        }
    }
}
