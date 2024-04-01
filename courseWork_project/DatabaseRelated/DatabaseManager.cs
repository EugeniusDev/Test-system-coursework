using System.IO;

namespace courseWork_project
{
    /// <summary>
    /// Інтерфейс для всіх класів проекту, які мають доступ до баз даних
    /// </summary>
    /// <remarks>Містить методи FormAndSetDatabasePath та PathExists</remarks>
    public class DatabaseManager
    {
        /// <summary>
        /// Конструктор класу з одним параметром - назва тесту
        /// </summary>
        /// <remarks>На основі цього параметра формується шлях до бази даних</remarks>
        /// <param name="testTitle">Назва тесту, допускається нетранслітерована</param>
        public DatabaseManager(string testTitle)
        {
            FormAndSetDatabasePath(testTitle);
        }
        /// <summary>
        /// Конструктор класу з 2-ма параметрами
        /// </summary>
        /// <remarks>Використовується, коли відбувається запис інших даних, а не тесту</remarks>
        /// <param name="directoryPath">Назва директорії, куди треба записати дані</param>
        /// <param name="filePath">Назва файлу, куди треба записати дані (вказуйте розширення файлу)</param>
        public DatabaseManager(string directoryPath, string filePath)
        {
            DirectoryPath = directoryPath;
            FilePath = filePath;
            FullPath = System.IO.Path.Combine(DirectoryPath, FilePath);
        }

        // Відповідні до полів класу властивості
        public string FilePath { get; set; }
        public string DirectoryPath { get; set; }
        public string FullPath { get; set; }

        /// <summary>
        /// Формує поля класу, використовуючи назву тесту
        /// </summary>
        /// <param name="currentTestTitle">Назва тесту, допускається нетранслітерована</param>
        public void FormAndSetDatabasePath(string currentTestTitle)
        {
            string transliteratedTitle = DataDecoder.TransliterateAString(currentTestTitle);
            DirectoryPath = transliteratedTitle;
            FilePath = $"{transliteratedTitle}.txt";
            FullPath = System.IO.Path.Combine(DirectoryPath, FilePath);
        }
        /// <summary>
        /// Перевірка на наявність директорії із заданою назвою
        /// </summary>
        /// <remarks>Назва директорії задається при ініціалізації об'єкта класу або за допомогою FormAndSetDatabasePath</remarks>
        /// <returns>true, якщо директорія існує; false, якщо ні</returns>
        public virtual bool PathExists()
        {
            return Directory.Exists(DirectoryPath);
        }
    }
}
