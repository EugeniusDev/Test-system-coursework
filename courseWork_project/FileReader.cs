using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace courseWork_project
{
    /// <summary>
    /// Клас для зчитування даних з файлу
    /// </summary>
    /// <remarks>Має 2 конструктори, наслідує клас DatabaseManager</remarks>
    internal class FileReader : DatabaseManager
    {
        /// <summary>
        /// Конструктор класу з одним параметром - назва тесту
        /// </summary>
        /// <remarks>На основі цього параметра формується шлях до бази даних</remarks>
        /// <param name="testTitle">Назва тесту, допускається нетранслітерована</param>
        public FileReader(string testTitle) : base(testTitle) { }
        /// <summary>
        /// Конструктор класу з 2-ма параметрами
        /// </summary>
        /// <remarks>Використовується, коли відбувається читка інших даних, а не тесту</remarks>
        /// <param name="directoryPath">Назва директорії, звідки треба зчитати дані</param>
        /// <param name="filePath">Назва файлу, звідки треба зчитати дані (вказуйте розширення файлу)</param>
        public FileReader(string directoryPath, string filePath) : base(directoryPath, filePath) { }
        // Деструктор
        ~FileReader()
        {
            Debug.WriteLine("Знищено об'єкт FileReader");
        }
        /// <summary>
        /// Зчитує питання тесту з файлу рядок за рядком
        /// </summary>
        /// <remarks>Шлях файлу вказується при ініціалізації об'єкту класу або з допомогою FormAndSetDatabasePath</remarks>
        /// <returns>Список зчитаних рядків, що формують дані питань тесту; або порожній список, якщо файлу не існує</returns>
        public List<string> ReadAndReturnQuestionLines()
        {
            List<string> lines = new List<string>();
            if (!PathExists()) return lines;
            using (StreamReader streamReader = new StreamReader(FullPath))
            {
                bool firstIteration = true;
                // Допоки не буде досягнуто кінця файлу
                while (!streamReader.EndOfStream)
                {
                    string currLine = streamReader.ReadLine();
                    // При першій ітерації зчитування не відбувається, оскільки перший рядок - інформація про тест
                    if (firstIteration)
                    {
                        firstIteration = false;
                        continue;
                    }
                    // Кількість питань не має перевищувати 10
                    if (lines.Count < 10 && !string.IsNullOrEmpty(currLine))
                        lines.Add(currLine);
                }
            }
            return lines;
        }
        /// <summary>
        /// Зчитує інформацію про тест з файлу
        /// </summary>
        /// <remarks>Шлях файлу вказується при ініціалізації об'єкту класу або з допомогою FormAndSetDatabasePath</remarks>
        /// <returns>Перший рядок файлу або пустий рядок, якщо файлу не існує чи він порожній</returns>
        public string ReadAndReturnTestInfo()
        {
            if (!PathExists()) return string.Empty;
            using (StreamReader streamReader = new StreamReader(FullPath))
            {
                // Якщо не досягнуто кінця файлу (тобто якщо він не порожній)
                if (!streamReader.EndOfStream)
                {
                    return streamReader.ReadLine();
                }
            }
            return string.Empty;
        }
        /// <summary>
        /// Зчитує дані з файлу рядок за рядком
        /// </summary>
        /// <remarks>Шлях файлу вказується при ініціалізації об'єкту класу</remarks>
        /// <returns>Список зчитаних рядків; або порожній список, якщо файлу не існує</returns
        public List<string> ReadAndReturnLines()
        {
            List<string> lines = new List<string>();
            if (!PathExists()) return lines;
            using (StreamReader streamReader = new StreamReader(FullPath))
            {
                // Допоки не буде досягнуто кінця файлу
                while (!streamReader.EndOfStream)
                {
                    string currLine = streamReader.ReadLine();
                    if(!string.IsNullOrEmpty(currLine))
                        lines.Add(currLine);
                }
            }
            return lines;
        }
        /// <summary>
        /// Формує список назв тестів, бази даних яких існують
        /// </summary>
        /// <remarks>Модифікує поля об'єкта при роботі з кожним елементом списку</remarks>
        /// <param name="testTitlesList">Список назв тестів, допускаються нетранслітеровані</param>
        /// <returns>Список транслітерованих назв тестів, бази даних яких існують. Якщо таких немає, то порожній список</returns>
        private List<string> FormListOfTestsWithExistingDatabases(List<string> testTitlesList)
        {
            List<string> listToForm = new List<string>();
            foreach (string testTitle in testTitlesList)
            {
                // Зміна шляху на шлях до бази даних поточного тесту
                FormAndSetDatabasePath(testTitle);
                if (PathExists())
                    listToForm.Add(testTitle);
            }
            return listToForm;
        }
        /// <summary>
        /// Оновлює список назв тестів, бази даних яких існують та записує його у потрібний файл
        /// </summary>
        /// <remarks>Порядково зчитує файл та формує список використовуючи інші методи FileReader. 
        /// Записує новий список у файл, шлях до якого вказується при ініціалізації об'єкту класу</remarks>
        /// <returns>Список нетранслітерованих назв тестів, бази даних яких існують</returns>
        public List<string> RefreshTheListOfTests()
        {
            // Тимчасове збереження початкових значень полів
            string tempDirPath = DirectoryPath;
            string tempFilePath = FilePath;
            // Список транслітерованих назв тестів, бази даних яких існують
            List<string> existingTestsTitles = FormListOfTestsWithExistingDatabases(ReadAndReturnLines());
            // Повернення полям початкових значень (бо FormListOfTestsWithExistingDatabases їх змінює)
            DirectoryPath = tempDirPath;
            FilePath = tempFilePath;
            FullPath = System.IO.Path.Combine(tempDirPath, tempFilePath);
            // Записує оновлений список транслітерованих назв тестів у потрібний файл
            FileWriter fileWriter = new FileWriter(DirectoryPath, FilePath);
            fileWriter.WriteListInFileByLines(existingTestsTitles);
            // Повертає оновлений список транслітерованих назв тестів
            return existingTestsTitles;
        }
        /// <summary>
        /// Перевірка на наявність шляху за заданими полями
        /// </summary>
        /// <remarks>Значення полів задаються при ініціалізації об'єкта класу або за допомогою FormAndSetDatabasePath</remarks>
        /// <returns>true, якщо шлях існує; false, якщо ні</returns>
        public override bool PathExists()
        {
            return Directory.Exists(DirectoryPath) && File.Exists(FullPath);
        }
    }
}
