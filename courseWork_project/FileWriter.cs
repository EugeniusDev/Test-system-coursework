using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace courseWork_project
{
    /// <summary>
    /// Клас для збереження даних у файл
    /// </summary>
    /// <remarks>Має 2 конструктори, наслідує клас DatabaseManager</remarks>
    internal class FileWriter : DatabaseManager
    {
        /// <summary>
        /// Конструктор класу з одним параметром - назва тесту
        /// </summary>
        /// <remarks>На основі цього параметра формується шлях до бази даних</remarks>
        /// <param name="testTitle">Назва тесту, допускається нетранслітерована</param>
        public FileWriter(string testTitle) : base(testTitle) { }
        /// <summary>
        /// Конструктор класу з 2-ма параметрами
        /// </summary>
        /// <remarks>Використовується, коли відбувається запис інших даних, а не тесту</remarks>
        /// <param name="directoryPath">Назва директорії, куди треба записати дані</param>
        /// <param name="filePath">Назва файлу, куди треба записати дані (вказуйте розширення файлу)</param>
        public FileWriter(string directoryPath, string filePath) : base(directoryPath, filePath) { }
        // Деструктор
        ~FileWriter()
        {
            Debug.WriteLine("Знищено об'єкт FileWriter");
        }
        /// <summary>
        /// Записує заданий список у файл рядок за рядком
        /// </summary>
        /// <remarks>Шлях файлу вказується при ініціалізації об'єкту класу або з допомогою FormAndSetDatabasePath</remarks>
        /// <param name="textToWrite">Список рядків для запису в файл</param>
        public void WriteListInFileByLines(List<string> textToWrite)
        {
            // Якщо переданий список порожній, то метод нічого не робить
            if (textToWrite.Count == 0) return;
            // Якщо директорії із заданою назвою не існує, така створюється
            if (!PathExists())
            {
                Directory.CreateDirectory(DirectoryPath);
            }
            // Об'єднання назви директорії та файлу для знаходження повного шляху до файлу
            string fullPath = Path.Combine(DirectoryPath, FilePath);
            // Запис переданого списку в файл
            using (StreamWriter writer = new StreamWriter(fullPath))
            {
                foreach (string line in textToWrite)
                {
                    writer.WriteLine(line);
                }
            }
        }
    }
}
