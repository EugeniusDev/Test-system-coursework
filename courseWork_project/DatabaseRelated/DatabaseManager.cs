using System.Configuration;
using System.IO;

namespace courseWork_project
{
    /// <summary>
    /// Superclass for all classes which have access to databases
    /// </summary>
    internal class DatabaseManager
    {
        private static readonly string transliteratedTestTitlesDirectoryPath = ConfigurationManager.AppSettings["testTitlesDirPath"];
        private static readonly string transliteratedTestTitlesFileName = ConfigurationManager.AppSettings["testTitlesFileName"];

        public string FilePath { get; set; }
        public string DirectoryPath { get; set; }
        public string FullPath { get; set; }
        
        /// <summary>
        /// Getting access to all tests
        /// </summary>
        public DatabaseManager() : this(transliteratedTestTitlesDirectoryPath, transliteratedTestTitlesFileName) { }
        
        /// <summary>
        /// Operating with certain test
        /// </summary>
        /// <param name="testTitle">Test title, not transliterated is also allowed</param>
        public DatabaseManager(string testTitle)
        {
            UpdateDatabasePathUsingTitle(testTitle);
        }
        /// <summary>
        /// Constructor for working with specified paths
        /// </summary>
        /// <param name="directoryPath">Path to directory that holds wanted file</param>
        /// <param name="filePath">Name of wanted file (specify extension as well)</param>
        public DatabaseManager(string directoryPath, string filePath)
        {
            DirectoryPath = directoryPath;
            FilePath = filePath;
            FullPath = Path.Combine(DirectoryPath, FilePath);
        }

        public void UpdateDatabasePathUsingTitle(string newTestTitle)
        {
            string transliteratedTitle = newTestTitle.TransliterateToEnglish();
            DirectoryPath = transliteratedTitle;
            FilePath = $"{transliteratedTitle}.txt";
            FullPath = Path.Combine(DirectoryPath, FilePath);
        }


        /// <summary>
        /// Check of existance of directory with specified name. Creating it if needed
        /// </summary>
        /// <returns>true, if directory already exists; false if not</returns>
        public virtual bool CreatePathIfNotExists()
        {
            if (!Directory.Exists(DirectoryPath))
            {
                Directory.CreateDirectory(DirectoryPath);

                return false;
            }

            return true;
        }
    }
}
