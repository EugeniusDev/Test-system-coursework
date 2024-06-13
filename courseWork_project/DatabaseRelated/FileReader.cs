using System.Collections.Generic;
using System.IO;

namespace courseWork_project
{
    internal class FileReader : DatabaseManager
    {
        public FileReader() : base() { }
        public FileReader(string testTitle) : base(testTitle) { }
        public FileReader(string directoryPath, string filePath) : base(directoryPath, filePath) { }

        public List<string> GetFileContentInLines()
        {
            if (!FullPathExists())
            {
                CreateFullPath();
                return new List<string>();
            }

            return ReadLinesFromFile();
        }

        private List<string> ReadLinesFromFile()
        {
            List<string> lines = new List<string>();
            using (StreamReader streamReader = new StreamReader(FullPath))
            {
                while (!streamReader.EndOfStream)
                {
                    string currentLine = streamReader.ReadLine();

                    if (!string.IsNullOrEmpty(currentLine))
                    {
                        lines.Add(currentLine);
                    }
                }
            }

            return lines;
        }

        public List<string> GetExistingTestTitles()
        {
            List<string> testTitlesList = GetFileContentInLines();
            List<string> existingDistinctTestTitles = new List<string>();
            foreach (string testTitle in testTitlesList)
            {
                UpdateDatabasePathByTitle(testTitle);

                if (FullPathExists() && !existingDistinctTestTitles.Contains(testTitle))
                {
                    existingDistinctTestTitles.Add(testTitle);
                }
            }

            return existingDistinctTestTitles;
        }

        public bool FullPathExists()
        {
            return DirectoryExists() && File.Exists(FullPath);
        }

        public bool DirectoryExists()
        {
            return Directory.Exists(DirectoryName);
        }

        private void CreateFullPath()
        {
            Directory.CreateDirectory(DirectoryName);
            if (!File.Exists(FullPath))
            {
                File.Create(FullPath).Close();
            }
        }
    }
}
