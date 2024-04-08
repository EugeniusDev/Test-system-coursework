using System.Configuration;
using System.IO;

namespace courseWork_project
{
    /// <summary>
    /// class for manipulation with images
    /// </summary>
    public static class ImageManager
    {
        /// <summary>
        /// Structure for managing info about an image
        /// </summary>
        public struct ImageInfo
        {
            public string imagePath;
            // Index of question to which image is linked to
            public int questionIndex;
        }
        public static string ImagesDirectory { get { return ConfigurationManager.AppSettings["imagesDirPath"]; } }
        /// <summary>
        /// Moves an image from old path to a new one
        /// </summary>
        /// <param name="currentImagePath">Current path of an image</param>
        /// <param name="wantedImageTitle">New filename (path) of an image</param>
        public static void CopyImageToFolder(string currentImagePath, string wantedImageTitle)
        {
            Directory.CreateDirectory(ImagesDirectory);
            string fileExtension = Path.GetExtension(currentImagePath);
            string relativePath = Path.Combine(ImagesDirectory, 
                string.Concat(wantedImageTitle, fileExtension));

            File.Copy(currentImagePath, relativePath, true);
        }
        /// <summary>
        /// Provides all data about all images in database-directory
        /// </summary>
        /// <returns>Tuple from array of relative paths and bool value that shows if array is empty</returns>
        public static (string[] imagePaths, bool imagesExist) GetAllImages()
        {
            Directory.CreateDirectory(ImagesDirectory);
            string[] allImagesPaths = Directory.GetFiles(ImagesDirectory);

            bool imagesExist = allImagesPaths.Length != 0;
            return (allImagesPaths, imagesExist);
        }
        /// <summary>
        /// Renames (moves) all images according to a new test title value
        /// </summary>
        /// <param name="oldTestTitle">Old test title (not transliterated is also allowed)</param>
        /// <param name="newTestTitle">New test title (not transliterated is also allowed)</param>
        public static void RenameAll(string oldTestTitle, string newTestTitle)
        {
            string oldTestTitleTransliterated = DataDecoder.TransliterateAString(oldTestTitle);
            string newTestTitleTransliterated = DataDecoder.TransliterateAString(newTestTitle);

            (string[], bool) allImagesTuple = GetAllImages();
            if (!allImagesTuple.Item2) return;

            foreach (string currentImageRelativePath in allImagesTuple.Item1)
            {
                string imageDirectory = Path.GetFileName(Path.GetDirectoryName(currentImageRelativePath));
                bool containsOldTestTitle = currentImageRelativePath.Contains(oldTestTitleTransliterated);
                string newImageRelativePath = currentImageRelativePath.Replace(oldTestTitleTransliterated, newTestTitleTransliterated);
                bool isInDatabaseDirectory = string.Compare(imageDirectory, ImagesDirectory) == 0;
                if (containsOldTestTitle && isInDatabaseDirectory)
                {
                    if (File.Exists(newImageRelativePath))
                    {
                        File.Delete(newImageRelativePath);
                    }

                    File.Move(currentImageRelativePath, newImageRelativePath);
                }
                else if (containsOldTestTitle)
                {
                    File.Copy(currentImageRelativePath, newImageRelativePath, true);
                }
            }
        }
        /// <summary>
        /// Deletes all images of specified test (deprecated)
        /// </summary>
        /// <param name="testTitle">Title of test to delete related to images (not transliterated is also allowed)</param>
        public static void ImagesCleanup(string testTitle)
        {
            (string[], bool) allImagesTuple = GetAllImages();
            if (!allImagesTuple.Item2) return;

            string transliteratedTestTitle = DataDecoder.TransliterateAString(testTitle);
            foreach (string currentImageRelativePath in allImagesTuple.Item1)
            {
                bool containsTestTitle = currentImageRelativePath.Contains(transliteratedTestTitle);
                if (containsTestTitle)
                {
                    File.Delete(currentImageRelativePath);
                }
            }
        }
    }
}
