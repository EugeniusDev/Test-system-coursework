namespace courseWork_project
{
    /// <summary>
    /// Інтерфейс для всіх класів проекту, яким потрібно передавати дані в GUI
    /// </summary>
    /// <remarks>Містить метод GetListAndPutItInGUI</remarks>
    internal interface IListInGUIPuttable<T>
    {
        /// <summary>
        /// Метод, що передає дані з аргументу в GUI поточного вікна
        /// </summary>
        /// <param name="questionsOrTestsList">Список питань/тестів (List<Test.Question> або List<string></param>
        void GetListAndPutItInGUI(T questionsOrTestsList);
    }
}
