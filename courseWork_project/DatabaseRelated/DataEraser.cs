using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading;
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
        /// Видаляє директорію бази даних заданого тесту
        /// </summary>
        /// <remarks>Видаляє і директорію, і саму базу даних в ній</remarks>
        /// <param name="testTitle">Назва тесту, допускається нетранслітерована</param>
        public static void EraseTestFolder(string testTitle)
        {
            FileReader reader = new FileReader(testTitle);
            if (reader.PathExists())
            {
                Directory.Delete(reader.DirectoryPath, true);
            }
        }
        /// <summary>
        /// Видаляє картинку з папки-бази даних
        /// </summary>
        /// <param name="imageInfo">Структура даних про картинку</param>
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
                        // Ігноруємо проблеми і живемо далі
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }
        /// <summary>
        /// Видаляє дані проходження тесту
        /// </summary>
        /// <param name="testTitle">Назва тесту, допускається нетранслітерована</param>
        public static void ErasePassingData(string testTitle)
        {
            string pathOfTestsDirectory = ConfigurationManager.AppSettings["testResultsDirPath"];
            string pathOfFile = $"{DataDecoder.TransliterateAString(testTitle)}.txt";
            string fullPath = Path.Combine(pathOfTestsDirectory, pathOfFile);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
    }
}
