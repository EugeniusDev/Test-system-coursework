using System.Configuration;
using System.IO;

namespace courseWork_project
{
    /// <summary>
    /// Клас для маніпуляцій над ілюстраціями
    /// </summary>
    public static class ImageManager
    {
        /// <summary>
        /// Структура для збереження та маніпуляції даними про додану/змінену ілюстрацію
        /// </summary>
        public struct ImageInfo
        {
            // Шлях до картинки
            public string imagePath;
            // Індекс запитання, до якого додано ілюстрацію
            public int questionIndex;
        }
        // Містить шлях до папки з картинками
        public static string ImagesDirectory { get { return ConfigurationManager.AppSettings["imagesDirPath"]; } }
        /// <summary>
        /// Переміщує картинку з наданого шляху в бажаний з її перейменуванням
        /// </summary>
        /// <param name="currentImagePath">Наданий шлях до картинки</param>
        /// <param name="wantedImageTitle">Нове ім'я картинки</param>
        public static void CopyImageToFolder(string currentImagePath, string wantedImageTitle)
        {
            string fileExtension = Path.GetExtension(currentImagePath);
            string relativePath = Path.Combine(ImagesDirectory, wantedImageTitle + fileExtension);

            string absolutePathToMoveOn = Path.GetFullPath(relativePath);

            File.Copy(currentImagePath, absolutePathToMoveOn, true);
        }
        /// <summary>
        /// Повертає масив відносних шляхів до картинок та булеве значення на позначення існування картинок в директорії
        /// </summary>
        /// <returns>Кортеж з масиву відносних шляхів та булевого значення</returns>
        public static (string[] imagePaths, bool imagesExist) GetAllImages()
        {
            // Отримуємо всі наявні картинки
            string[] allImagesPaths = Directory.GetFiles(ImagesDirectory);
            // Залежно від наявності картинок присвоюємо булеве значення
            bool imagesExist = allImagesPaths.Length != 0;
            return (allImagesPaths, imagesExist);
        }
        /// <summary>
        /// Перейменовує всі картинки при оновленні назви тесту
        /// </summary>
        /// <param name="oldTestTitle">Стара назва тесту (допускається нетранслітерована)</param>
        /// <param name="newTestTitle">Нова назва тесту (допускається нетранслітерована)</param>
        public static void RenameAll(string oldTestTitle, string newTestTitle)
        {
            // Транслітеруємо надані назви тесту
            string oldTestTitleTransliterated = DataDecoder.TransliterateAString(oldTestTitle);
            string newTestTitleTransliterated = DataDecoder.TransliterateAString(newTestTitle);

            (string[], bool) allImagesTuple = GetAllImages();
            // Зупинка функції в разі відсутності картинок
            if (!allImagesTuple.Item2) return;

            foreach (string currentImageRelativePath in allImagesTuple.Item1)
            {
                string fullPathToImage = Path.GetFullPath(currentImageRelativePath);
                // Отримання тільки назви директорії завдяки звертанню до неї як до крайнього файлу
                string imageDirectory = Path.GetFileName(Path.GetDirectoryName(fullPathToImage));
                // Якщо картинка містить стару назву тесту в назві та вже переміщена в потрібну папку
                bool containsOldTestTitle = currentImageRelativePath.Contains(oldTestTitleTransliterated)
                    && string.Compare(imageDirectory, ImagesDirectory) == 0;
                if (containsOldTestTitle)
                {
                    string newImageRelativePath = currentImageRelativePath.Replace(oldTestTitleTransliterated, newTestTitleTransliterated);
                    string newImageAbsolutePath = Path.GetFullPath(newImageRelativePath);
                    // Копіювання чи переміщення картинки залежно від її наявності на заданому шляху
                    if (File.Exists(newImageAbsolutePath))
                    {
                        File.Copy(fullPathToImage, newImageAbsolutePath, true);
                    }
                    else
                    {
                        File.Move(fullPathToImage, newImageAbsolutePath);
                    }
                }
            }
        }
        /// <summary>
        /// Видаляє всі картинки тесту з вказаною назвою
        /// </summary>
        /// <remarks>Працює лише з директорією-базою даних</remarks>
        /// <param name="testTitle">Назва тесту, що видаляється (допускається нетранслітерована)</param>
        public static void ImagesCleanup(string testTitle)
        {
            (string[], bool) allImagesTuple = GetAllImages();
            // Зупинка функції в разі відсутності картинок
            if (!allImagesTuple.Item2) return;

            string transliteratedTestTitle = DataDecoder.TransliterateAString(testTitle);
            foreach (string currentImageRelativePath in allImagesTuple.Item1)
            {
                string fullPathToImage = Path.GetFullPath(currentImageRelativePath);
                // Отримання тільки назви директорії завдяки звертанню до неї як до крайнього файлу
                string imageDirectory = Path.GetFileName(Path.GetDirectoryName(fullPathToImage));
                // Якщо картинка містить назву тесту в назві та вже переміщена в потрібну папку
                bool containsTestTitle = currentImageRelativePath.Contains(transliteratedTestTitle)
                    && string.Compare(imageDirectory, ImagesDirectory) == 0;
                if (containsTestTitle)
                {
                    File.Delete(currentImageRelativePath);
                }
            }
        }
    }
}
