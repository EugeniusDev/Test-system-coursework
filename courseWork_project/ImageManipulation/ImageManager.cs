using courseWork_project.ImageManipulation;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace courseWork_project
{
    public static class ImageManager
    {
        public static readonly string DefaultPath = string.Empty;

        public static bool IsLinkedImageDefault(TestStructs.QuestionMetadata question)
        {
            return question.linkedImagePath.Equals(DefaultPath);
        }

        public static BitmapImage GetBitmapImageByPath(string imagePath)
        {
            BitmapImage foundImageBitmap = new BitmapImage();
            foundImageBitmap.BeginInit();
            try
            {
                foundImageBitmap.UriSource = new Uri(imagePath, UriKind.RelativeOrAbsolute);
            }
            catch
            {
                return DefaultBitmapImage();
                throw new ArgumentException($"{imagePath} is not valid");
            }
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
