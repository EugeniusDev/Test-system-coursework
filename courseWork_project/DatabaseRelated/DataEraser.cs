using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace courseWork_project.DatabaseRelated
{
    public static class DataEraser
    {
        public static void EraseTestCreatingMode(TestStructs.TestMetadata testMetadata)
        {
            EraseTestFolderByTitle(testMetadata.testTitle);
            EraseTestPassingDataByTitle(testMetadata.testTitle);
        }

        public static void EraseTestEditingMode(TestStructs.TestMetadata testMetadata, List<ImageManager.ImageMetadata> imageMetadatas)
        {
            EraseTestFolderByTitle(testMetadata.testTitle);
            EraseTestPassingDataByTitle(testMetadata.testTitle);
            imageMetadatas.ForEach(img => EraseImage(img));
        }

        /// <summary>
        /// Deletes a directory-database of given test
        /// </summary>
        /// <remarks>Deletes directory recursively, including all files in it</remarks>
        /// <param name="testTitle">Test title, not transliterated is also allowed</param>
        public static void EraseTestFolderByTitle(string testTitle)
        {
            FileReader reader = new FileReader(testTitle);
            if (reader.FullPathExists())
            {
                Directory.Delete(reader.DirectoryPath, true);
            }
        }
        /// <summary>
        /// Deletes an image from directory-database
        /// </summary>
        /// <param name="imageMetadata">Structure with image's info</param>
        public static void EraseImage(ImageManager.ImageMetadata imageMetadata)
        {
            if (File.Exists(imageMetadata.imagePath))
            {
                Task.Delay(100).ContinueWith(_ =>
                {
                    try
                    {
                        File.Delete(imageMetadata.imagePath);
                    }
                    catch
                    {
                        // Not critical for application work
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }
        /// <summary>
        /// Deletes given test's passing data
        /// </summary>
        /// <param name="testTitle">Test title, not transliterated is also allowed</param>
        public static void EraseTestPassingDataByTitle(string testTitle)
        {
            string pathOfTestsDirectory = Properties.Settings.Default.testResultsDirectory;
            string pathOfFile = $"{testTitle.TransliterateToEnglish()}.txt";
            string fullPath = Path.Combine(pathOfTestsDirectory, pathOfFile);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
    }
}
