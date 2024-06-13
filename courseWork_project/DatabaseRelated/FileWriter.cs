using System.Collections.Generic;
using System.IO;
using static courseWork_project.TestStructs;

namespace courseWork_project
{
    internal class FileWriter : DatabaseManager
    {
        private static readonly string resultsDirectoryName = Properties.Settings.Default.testResultsDirectory;
        public FileWriter() : base() { }
        public FileWriter(string testTitle) : base(testTitle) { }
        public FileWriter(string directoryPath, string filePath) : base(directoryPath, filePath) { }

        public void WriteListLineByLine(List<string> listToWrite)
        {
            if (listToWrite.Count == 0)
            {
                return;
            }

            Directory.CreateDirectory(DirectoryName);

            WriteListInPath(listToWrite);
        }

        private void WriteListInPath(List<string> listToWrite)
        {
            string fullPath = GetFullPath(DirectoryName, FileName);
            using (StreamWriter writer = new StreamWriter(fullPath))
            {
                foreach (string line in listToWrite)
                {
                    writer.WriteLine(line);
                }
            }
        }

        public void AppendNewTestPassingData(TestMetadata testMetadata, string resultToWrite)
        {
            string resultsFileName = $"{testMetadata.testTitle.TransliterateToEnglish()}.txt";
            DirectoryName = resultsDirectoryName;
            FileName = resultsFileName;

            Directory.CreateDirectory(DirectoryName);

            AppendLineToFile(resultToWrite);
        }

        public void AppendLineToFile(string line)
        {
            string fullPath = GetFullPath(DirectoryName, FileName);
            using (StreamWriter sw = File.AppendText(fullPath))
            {
                sw.WriteLine(line);
            }
        }

        private static string GetFullPath(string directoryName, string fileName)
        {
            return Path.Combine(directoryName, fileName);
        }
    }
}
