using System.Collections.Generic;
using System.IO;

namespace courseWork_project
{
    /// <summary>
    /// Superclass for all classes which have access to databases
    /// </summary>
    internal class DatabaseManager
    {
        private static readonly string transliteratedTestTitlesDirectoryPath = Properties.Settings.Default.testTitlesDirectory;
        private static readonly string transliteratedTestTitlesFileName = Properties.Settings.Default.testTitlesFilename;

        public string FileName { get; set; }
        public string DirectoryName { get; set; }
        public string FullPath { get; set; }
        
        /// <summary>
        /// Getting access to all existing tests
        /// </summary>
        public DatabaseManager() : this(transliteratedTestTitlesDirectoryPath, transliteratedTestTitlesFileName) { }
        
        /// <summary>
        /// Operating with certain test
        /// </summary>
        public DatabaseManager(string testTitle)
        {
            UpdateDatabasePathByTitle(testTitle);
        }

        public DatabaseManager(string directoryPath, string filePath)
        {
            DirectoryName = directoryPath;
            FileName = filePath;
            FullPath = Path.Combine(DirectoryName, FileName);
        }

        public void UpdateDatabasePathByTitle(string newTestTitle)
        {
            string transliteratedTitle = newTestTitle.TransliterateToEnglish();
            SetDirectoryName(transliteratedTitle);
            FileName = $"{transliteratedTitle}.txt";
            FullPath = Path.Combine(DirectoryName, FileName);
        }

        public void SetDirectoryName(string newDirectoryName)
        {
            DirectoryName = newDirectoryName;
        }
    }
}
