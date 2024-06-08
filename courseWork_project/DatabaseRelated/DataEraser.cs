using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;

namespace courseWork_project.DatabaseRelated
{
    public static class DataEraser
    {
        /// <summary>
        /// Erases all data related with current test
        /// </summary>
        /// <param name="testInfo">TestInfo structure</param>
        /// <param name="creatingMode">Is test in creating mode or in edit mode</param>
        /// <param name="imagesList">List of ImageInfo structures with data about images</param>
        public static void EraseCurrentTestData(TestStructs.TestInfo testInfo, bool creatingMode, List<ImageManager.ImageInfo> imagesList)
        {
            EraseTestFolder(testInfo.testTitle);
            ErasePassingData(testInfo.testTitle);
            if (!creatingMode)
            {
                imagesList.ForEach(img => EraseImage(img));
            }
        }

        /// <summary>
        /// Deletes a directory-database of given test
        /// </summary>
        /// <remarks>Deletes directory recursively, including all files in it</remarks>
        /// <param name="testTitle">Test title, not transliterated is also allowed</param>
        public static void EraseTestFolder(string testTitle)
        {
            FileReader reader = new FileReader(testTitle);
            if (reader.PathExists())
            {
                Directory.Delete(reader.DirectoryPath, true);
            }
        }
        /// <summary>
        /// Deletes an image from directory-database
        /// </summary>
        /// <param name="imageInfo">Structure with image's info</param>
        public static void EraseImage(ImageManager.ImageInfo imageInfo)
        {
            if (File.Exists(imageInfo.imagePath))
            {
                // Attempting to delete file after a delay of 100 milliseconds
                Task.Delay(100).ContinueWith(_ =>
                {
                    try
                    {
                        File.Delete(imageInfo.imagePath);
                    }
                    catch
                    {
                        // Ignoring the problems and continue on living :)
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }
        /// <summary>
        /// Deletes given test's passing data
        /// </summary>
        /// <param name="testTitle">Test title, not transliterated is also allowed</param>
        public static void ErasePassingData(string testTitle)
        {
            string pathOfTestsDirectory = ConfigurationManager.AppSettings["testResultsDirPath"];
            string pathOfFile = $"{testTitle.TransliterateToEnglish()}.txt";
            string fullPath = Path.Combine(pathOfTestsDirectory, pathOfFile);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
    }
}
