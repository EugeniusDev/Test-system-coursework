using System.Collections.Generic;
using System.IO;

namespace courseWork_project
{
    /// <summary>
    /// Class for reading data out of files
    /// </summary>
    /// <remarks>Has 2 constructors, inherits DatabaseManager class</remarks>
    internal class FileReader : DatabaseManager
    {
        public FileReader() : base() { }
        public FileReader(string testTitle) : base(testTitle) { }
        public FileReader(string directoryPath, string filePath) : base(directoryPath, filePath) { }

        /// <summary>
        /// Reads test's Questions line by line
        /// </summary>
        /// <remarks>Path to database can be changed with help of UpdateDatabasePathUsingTitle</remarks>
        /// <returns>List of strings with Questions data; empty or filled</returns>
        public List<string> GetQuestionLines()
        {
            List<string> lines = new List<string>();
            if (!PathExists()) return lines;
            using (StreamReader streamReader = new StreamReader(FullPath))
            {
                bool firstIteration = true;
                while (!streamReader.EndOfStream)
                {
                    string currLine = streamReader.ReadLine();
                    // Ignoring first line as it contains data not about Questions but about test in general
                    if (firstIteration)
                    {
                        firstIteration = false;
                        continue;
                    }
                    // Questions count cannot be greater than 10 according to coursework task
                    if (lines.Count < 10 && !string.IsNullOrEmpty(currLine))
                        lines.Add(currLine);
                }
            }
            return lines;
        }
        /// <summary>
        /// Reads general test info from a file
        /// </summary>
        /// <remarks>Path to database can be changed with help of UpdateDatabasePathUsingTitle</remarks>
        /// <returns>First line from a file or empty string</returns>
        public string GetTestInfo()
        {
            if (!PathExists()) return string.Empty;
            using (StreamReader streamReader = new StreamReader(FullPath))
            {
                // Якщо не досягнуто кінця файлу (тобто якщо він не порожній)
                if (!streamReader.EndOfStream)
                {
                    return streamReader.ReadLine();
                }
            }
            return string.Empty;
        }
        /// <summary>
        /// Reads all the data from file line by line
        /// </summary>
        /// <remarks>Path to a file is set on object initialization</remarks>
        /// <returns>Lines list; empty or filled</returns
        public List<string> ReadAndReturnLines()
        {
            List<string> lines = new List<string>();
            if (!CreatePathIfNotExists()) return lines;
            try
            {
                using (StreamReader streamReader = new StreamReader(FullPath))
                {
                    while (!streamReader.EndOfStream)
                    {
                        string currLine = streamReader.ReadLine();
                        if (!string.IsNullOrEmpty(currLine))
                            lines.Add(currLine);
                    }
                }
            }
            catch
            {
                // Returning empty list anyway, so catch can be ignored
            }

            return lines;
        }
        /// <summary>
        /// Gets list of titles of tests with existing databases
        /// </summary>
        /// <remarks>Changes paths while working</remarks>
        /// <param name="testTitlesList">List of test titles, not transliterated also allowed</param>
        /// <returns>List of titles of transliterated tests with existing databases. Empty or filled</returns>
        private List<string> FormListOfExistingTests(List<string> testTitlesList)
        {
            List<string> listToForm = new List<string>();
            foreach (string testTitle in testTitlesList)
            {
                // Changing path to a current test's file-database
                UpdateDatabasePathUsingTitle(testTitle);

                if (PathExists() && !listToForm.Contains(testTitle))
                {
                    listToForm.Add(testTitle);
                }
            }

            return listToForm;
        }
        /// <summary>
        /// Updates list of titles of tests with existing databases
        /// </summary>
        /// <remarks>Reads file line by line and forms a list using other methods of the class. 
        /// Rewrites a new list to a respective file</remarks>
        /// <returns>List of titles of tests with existing databases</returns>
        public List<string> UpdateListOfExistingTestsPaths()
        {
            // Saving initial paths as they are changed in the process
            string tempDirPath = DirectoryPath;
            string tempFilePath = FilePath;

            List<string> existingTestsTitles = FormListOfExistingTests(ReadAndReturnLines());
            // Restoring initial path values
            DirectoryPath = tempDirPath;
            FilePath = tempFilePath;
            FullPath = Path.Combine(tempDirPath, tempFilePath);
            // Writing updates list into a respective file
            FileWriter fileWriter = new FileWriter(DirectoryPath, FilePath);
            fileWriter.WriteListInFileByLines(existingTestsTitles);

            return existingTestsTitles;
        }
        /// <summary>
        /// Checking full path existance
        /// </summary>
        /// <remarks>Path values can be changed with help of UpdateDatabasePathUsingTitle</remarks>
        /// <returns>true if path exists; false if not</returns>
        public bool PathExists()
        {
            return Directory.Exists(DirectoryPath) && File.Exists(FullPath);
        }
        /// <summary>
        /// Checking full path existance and its creating in case of if not existing
        /// </summary>
        /// <remarks>Path values can be changed with help of UpdateDatabasePathUsingTitle</remarks>
        /// <returns>true if path exists; false if not</returns>
        public override bool CreatePathIfNotExists()
        {
            if (!Directory.Exists(DirectoryPath))
            {
                Directory.CreateDirectory(DirectoryPath);
                if (!File.Exists(FullPath))
                {
                    File.Create(FullPath).Close();
                }

                return false;
            }

            return true;
        }
    }
}
