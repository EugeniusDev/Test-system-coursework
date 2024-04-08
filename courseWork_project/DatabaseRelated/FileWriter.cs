using System.Collections.Generic;
using System.Configuration;
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
        /// <summary>
        /// Autofill-database-path constructor for operating with test's database
        /// </summary>
        /// <param name="testTitle">Test title, not transliterated is also allowed</param>
        public FileWriter(string testTitle) : base(testTitle) { }
        /// <summary>
        /// Constructor for working with specified paths
        /// </summary>
        /// <param name="directoryPath">Path to directory that holds file data to be written into</param>
        /// <param name="filePath">Name of file to write into (specify extension as well)</param>
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
        /// <param name="testInfo">TestInfo structure of current test</param>
        /// <param name="resultToWrite">Result to be written into a file</param>
        public void AppendTestTakingData(TestInfo testInfo, string resultToWrite)
        {
            string transliterTestTitle = DataDecoder.TransliterateAString(testInfo.testTitle);

            string pathToResultsDirectory = ConfigurationManager.AppSettings["testResultsDirPath"];
            Directory.CreateDirectory(pathToResultsDirectory);
            string pathToResultsFile = $"{transliterTestTitle}.txt";
            string fullPath = Path.Combine(pathToResultsDirectory, pathToResultsFile);

            using (StreamWriter sw = File.AppendText(fullPath))
            {
                sw.WriteLine(resultToWrite);
            }
        }
    }
}
