using System.Collections.Generic;
using System.Configuration;
using System.IO;
using static courseWork_project.TestStructs;

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
        /// <summary>
        /// Записує заданий список у файл рядок за рядком
        /// </summary>
        /// <remarks>Шлях файлу вказується при ініціалізації об'єкту класу або з допомогою UpdateDatabasePath</remarks>
        /// <param name="textToWrite">Список рядків для запису в файл</param>
        public void WriteListInFileByLines(List<string> textToWrite)
        {
            // Якщо переданий список порожній, то метод нічого не робить
            if (textToWrite.Count == 0) return;
            Directory.CreateDirectory(DirectoryPath);
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
        /// <summary>
        /// Appends new data to file with results of test taking
        /// </summary>
        /// <remarks>If such file don't exists, creates it</remarks>
        /// <param name="testInfo">TestInfo structure of current test</param>
        /// <param name="resultToWrite">Result to be written into a file</param>
        public void AppendTestTakingData(TestInfo testInfo, string resultToWrite)
        {
            string transliterTestTitle = DataDecoder.TransliterateAString(testInfo.testTitle);
            // Отримання шляху до списку даних про проходження
            string pathToResultsDirectory = ConfigurationManager.AppSettings["testResultsDirPath"];
            Directory.CreateDirectory(pathToResultsDirectory);
            string pathToResultsFile = $"{transliterTestTitle}.txt";
            string fullPath = Path.Combine(pathToResultsDirectory, pathToResultsFile);
            // Допис нових даних про проходження
            using (StreamWriter sw = File.AppendText(fullPath))
            {
                sw.WriteLine(resultToWrite);
            }
        }
    }
}
