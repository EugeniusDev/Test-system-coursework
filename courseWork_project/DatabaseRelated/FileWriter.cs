using System.Collections.Generic;
using System.IO;
using static courseWork_project.TestStructs;

namespace courseWork_project
{
    /// <summary>
    /// Class for writing data into files
    /// </summary>
    /// <remarks>Has 2 constructors, inherits DatabaseManager class</remarks>
    internal class FileWriter : DatabaseManager
    {
        public FileWriter() : base() { }
        public FileWriter(string testTitle) : base(testTitle) { }
        public FileWriter(string directoryPath, string filePath) : base(directoryPath, filePath) { }
        /// <summary>
        /// Writes given list into file line by line
        /// </summary>
        /// <param name="textToWrite">List of strings to write into file</param>
        public void WriteListInFileByLines(List<string> textToWrite)
        {
            if (textToWrite.Count == 0) return;

            Directory.CreateDirectory(DirectoryPath);
            string fullPath = Path.Combine(DirectoryPath, FilePath);

            using (StreamWriter writer = new StreamWriter(fullPath))
            {
                foreach (string line in textToWrite)
                {
                    writer.WriteLine(line);
                }
            }
        }
        /// <summary>
        /// Appends new data to file with results of test taking
        /// </summary>
        /// <remarks>If such file don't exists, creates it</remarks>
        /// <param name="testMetadata">TestMetadata structure of current test</param>
        /// <param name="resultToWrite">Result to be written into a file</param>
        public void AppendNewTestPassingDataToDatabase(TestMetadata testMetadata, string resultToWrite)
        {
            string pathToResultsDirectory = Properties.Settings.Default.testResultsDirectory;
            Directory.CreateDirectory(pathToResultsDirectory);
            string pathToResultsFile = $"{testMetadata.testTitle.TransliterateToEnglish()}.txt";
            string fullPath = Path.Combine(pathToResultsDirectory, pathToResultsFile);

            using (StreamWriter sw = File.AppendText(fullPath))
            {
                sw.WriteLine(resultToWrite);
            }
        }
    }
}
