using System.Collections.Generic;
using System.IO;

namespace courseWork_project.DatabaseRelated
{
    public static class DataEraser
    {
        public static void EraseTestDatabases(TestStructs.TestMetadata testMetadata)
        {
            EraseTestFolderByTitle(testMetadata.testTitle);
            EraseTestPassingDataByTitle(testMetadata.testTitle);
        }

        public static void EraseTestFolderByTitle(string testTitle)
        {
            FileReader reader = new FileReader(testTitle);
            if (reader.FullPathExists())
            {
                Directory.Delete(reader.DirectoryName, true);
            }
        }

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

        public static void EraseTestByTestItem(TestItem selectedItem)
        {
            Test testToDelete = DataDecoder.GetTestByTestItem(selectedItem);
            List<ImageManager.ImageMetadata> imagesToDelete = testToDelete.GetRelatedImages();

            EraseTestDatabases(testToDelete.TestMetadata);
            ImageManager.TryDeleteImages(imagesToDelete);
        }

    }
}
