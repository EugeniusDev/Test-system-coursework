using courseWork_project.ImageManipulation;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

namespace courseWork_project
{
    public static class ImageManager
    {
        public struct ImageMetadata
        {
            public string path;
            // Index of question to which image is linked to
            public int questionIndex;
        }

        public static readonly ImageMetadata EmptyImageMetadata = new ImageMetadata()
        {
            path = string.Empty,
            questionIndex = -1
        };
        public static string ImagesDirectory { get { return Properties.Settings.Default.imagesDirectory; } }

        public static void CopyToDatabaseDirectoryWithNameOf(this ImageMetadata imageMetadata, string transliteratedTestTitle)
        {
            string newImageName = DataEncoder.GetConventionalImageName(transliteratedTestTitle, imageMetadata);
            string oldImagePath = imageMetadata.path;
            EnsureImageDirectoryExists();
            string relativePath = GetNewRelativePath(oldImagePath, newImageName);
            File.Copy(oldImagePath, relativePath, true);
        }

        public static void TryDeleteImage(ImageMetadata deprecatedImageMetadata)
        {
            if (ImageIsInDirectoryDatabase(deprecatedImageMetadata))
            {
                try
                {
                    File.Delete(deprecatedImageMetadata.path);
                }
                catch
                {
                    // Not critical for application work
                }
            }
        }

        private static void EnsureImageDirectoryExists()
        {
            Directory.CreateDirectory(ImagesDirectory);
        }
        
        private static string GetNewRelativePath(string currentImagePath, string newImageName)
        {
            string fileExtension = Path.GetExtension(currentImagePath);
            string relativePath = Path.Combine(ImagesDirectory,
                string.Concat(newImageName, fileExtension));
            return relativePath;
        }

        public static List<ImageMetadata> GetRelatedImages(this Test test)
        {
            List<string> imageDirectoryFiles = GetImageDirectoryFiles();
            if (imageDirectoryFiles.Count == 0)
            {
                return new List<ImageMetadata>();
            }

            List<ImageMetadata> imagesToReturn = new List<ImageMetadata>();
            foreach (string currentFileName in imageDirectoryFiles)
            {
                if (!IsFilenameRelatedToTest(currentFileName, test))
                {
                    continue;
                }

                ImageMetadata imageMetadata = currentFileName.ParseToImageMetadata();
                if (!imageMetadata.Equals(EmptyImageMetadata))
                {
                    imagesToReturn.Add(imageMetadata);
                }
            }

            return imagesToReturn;
        }

        public static List<string> GetImageDirectoryFiles()
        {
            EnsureImageDirectoryExists();
            return Directory.GetFiles(ImagesDirectory).ToList();
        }

        public static bool IsFilenameRelatedToTest(string fileName, Test test)
        {
            string transliteratedTestTitle = DataDecoder.TransliterateToEnglish(test.TestMetadata.testTitle);
            return fileName.Contains(transliteratedTestTitle);
        }

        public static bool IsImageInCorrectPlace(ImageMetadata imageMetadata, Test test)
        {
            return IsFilenameRelatedToTest(imageMetadata.path, test)
                && ImageIsInDirectoryDatabase(imageMetadata);
        }

        private static bool ImageIsInDirectoryDatabase(ImageMetadata imageMetadata)
        {
            string imageDirectories = Path.GetDirectoryName(imageMetadata.path);
            string firstLevelImageDirectory = Path.GetFileName(imageDirectories
                .Trim(Path.DirectorySeparatorChar));
            return firstLevelImageDirectory.Equals(ImagesDirectory);
        }
        // TODO delete this dead code
        //public static void MoveImagesUsingTitles(string oldTransliteratedTitle, string newTransliteratedTitle)
        //{
        //    List<string> imageDirectoryFiles = GetImageDirectoryFiles();
        //    if (imageDirectoryFiles.Count == 0)
        //    {
        //        return;
        //    }

        //    foreach (string currentImageRelativePath in imageDirectoryFiles)
        //    {
        //        string imageDirectory = Path.GetFileName(Path.GetDirectoryName(currentImageRelativePath));
        //        bool containsOldTestTitle = currentImageRelativePath.Contains(oldTransliteratedTitle);
        //        string newImageRelativePath = currentImageRelativePath.Replace(oldTransliteratedTitle, newTransliteratedTitle);
        //        bool isInDatabaseDirectory = string.Compare(imageDirectory, ImagesDirectory) == 0;
        //        if (containsOldTestTitle && isInDatabaseDirectory)
        //        {
        //            if (File.Exists(newImageRelativePath))
        //            {
        //                File.Delete(newImageRelativePath);
        //            }

        //            File.Move(currentImageRelativePath, newImageRelativePath);
        //        }
        //        else if (containsOldTestTitle)
        //        {
        //            File.Copy(currentImageRelativePath, newImageRelativePath, true);
        //        }
        //    }
        //}

        public static ImageMetadata GetImageForQuestionAtIndex(this Test test, int indexOfQuestion)
        {
            List<ImageMetadata> testImages = test.GetRelatedImages();
            string transliteratedTestTitle = test.TestMetadata.testTitle.TransliterateToEnglish();
            ImageMetadata supposedImage = testImages.Find(img => img.path.Contains($"{transliteratedTestTitle}-{indexOfQuestion}."));
            if (supposedImage.Equals(default(ImageMetadata)))
            {
                return EmptyImageMetadata;
            }

            return supposedImage;
        }

        public static void TryDeleteImages(List<ImageMetadata> images)
        {
            images.ForEach(TryDeleteImage);
        }

        public static BitmapImage GetBitmapImageByMetadata(ImageMetadata imageMetadata)
        {
            BitmapImage foundImageBitmap = new BitmapImage();
            foundImageBitmap.BeginInit();
            foundImageBitmap.UriSource = new Uri(imageMetadata.path, UriKind.RelativeOrAbsolute);
            foundImageBitmap.CacheOption = BitmapCacheOption.OnLoad;
            foundImageBitmap.EndInit();
            return foundImageBitmap;
        }

        public static BitmapImage DefaultBitmapImage()
        {
            Bitmap defaultImage = DefaultImage.default_image;
            // Convert Bitmap to BitmapImage
            BitmapImage bitmapImage = null;
            using (MemoryStream memory = new MemoryStream())
            {
                // Save Bitmap to memory stream
                defaultImage.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);

                // Rewind the stream
                memory.Position = 0;

                bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                // This is important to prevent file locks
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
            }

            return bitmapImage;
        }
    }
}
