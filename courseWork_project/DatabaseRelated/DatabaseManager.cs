using System.IO;

namespace courseWork_project
{
    /// <summary>
    /// Superclass for all classes which have access to databases
    /// </summary>
    public class DatabaseManager
    {
        public string FilePath { get; set; }
        public string DirectoryPath { get; set; }
        public string FullPath { get; set; }

        /// <summary>
        /// Autofill-database-path constructor for operating with test's database
        /// </summary>
        /// <param name="testTitle">Test title, not transliterated is also allowed</param>
        public DatabaseManager(string testTitle)
        {
            UpdateDatabasePath(testTitle);
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
        /// <summary>
        /// Populates class object's properties based on test title
        /// </summary>
        /// <param name="newTestTitle">Test title, not transliterated is also allowed</param>
        public void UpdateDatabasePath(string newTestTitle)
        {
            string transliteratedTitle = DataDecoder.TransliterateAString(newTestTitle);
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
