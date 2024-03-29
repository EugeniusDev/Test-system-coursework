﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace courseWork_project
{
    /// <summary>
    /// Клас, методи якого розкодовують інформацію з бази даних та формують потрібну структуру даних
    /// </summary>
    public abstract class DataDecoder
    {
        /// <summary>
        /// Словник транслітерування для використання рядків в якості шляхів до баз даних
        /// </summary>
        /// <remarks>Містить символи, що потребують заміни та самі символи заміни</remarks>
        private static Dictionary<char, string> transliterationTable = new Dictionary<char, string>
        {
            {'а', "a"}, {'б', "b"}, {'в', "v"}, {'г', "g"}, {'д', "d"}, {'е', "e"},
            {'є', "ye"}, {'ж', "zh"}, {'з', "z"}, {'и', "y"}, {'і', "i"}, {'ї', "yi"},
            {'й', "y"}, {'к', "k"}, {'л', "l"}, {'м', "m"}, {'н', "n"}, {'о', "o"},
            {'п', "p"}, {'р', "r"}, {'с', "s"}, {'т', "t"}, {'у', "u"}, {'ф', "f"},
            {'х', "kh"}, {'ц', "ts"}, {'ч', "ch"}, {'ш', "sh"}, {'щ', "shch"}, {'ь', ""},
            {'ю', "yu"}, {'я', "ya"}, {'ґ', "g"}, {' ', "_"}, {'<', "_"}, {'>', "_"}, {':', "_"},
            {'\"', "_"}, {'/', "_"}, {'\\', "_"}, {'|', "_"}, {'?', "_"}, {'*', "_"}
        };
        /// <summary>
        /// Отримує рядки з даними про питання, формує список структур даних цих питань
        /// </summary>
        /// <remarks>Використовує FileReader для зчитування даних з бази</remarks>
        /// <param name="testTitle">Назва тесту, допускається нетранслітерована</param>
        /// <returns>Список структур даних з питань шуканого тесту або пустий список, якщо дані відсутні</returns>
        public static List<Test.Question> FormQuestionsList(string testTitle)
        {
            List<Test.Question> formedQuestionsList = new List<Test.Question>();
            FileReader reader = new FileReader(testTitle);
            List<string> textInLines = reader.ReadAndReturnQuestionLines();
            int correctAnswerIndex;
            foreach (string line in textInLines)
            {
                // Створення нової структури, яка буде додана у список
                Test.Question tempQuestion = new Test.Question();
                tempQuestion.variants = new List<string>();
                tempQuestion.correctVariantsIndexes = new List<int>();
                // Розділення поточного рядка та розподілення його частин по полях структури питання
                string[] splitLine = line.Split(new char[] { '₴' }, StringSplitOptions.RemoveEmptyEntries);
                tempQuestion.question = splitLine[0];
                for (int i = 1; i < splitLine.Length-1; i++)
                {
                    // Якщо поточна частина рядка може бути конвертована в int додаємо її до correctVariantsIndexes
                    if (int.TryParse(splitLine[i], out correctAnswerIndex) && splitLine[i].Length == 1)
                    {
                        tempQuestion.correctVariantsIndexes.Add(correctAnswerIndex);
                    }
                    // Інакше поки кількість варіантів не перевищує 8, додаємо поточну частину рядка в ролі варіанта
                    else if(tempQuestion.variants.Count < 8)
                    {
                        tempQuestion.variants.Add(splitLine[i]);
                    }
                }
                // Конвертування інформації про під'єднану ілюстрацію з останнього розбитого елемента
                tempQuestion.hasLinkedImage = bool.Parse(splitLine[splitLine.Length-1]);
                formedQuestionsList.Add(tempQuestion);
            }
            return formedQuestionsList;
        }
        /// <summary>
        /// Отримує перший рядок з бази даних тесту, формує структуру інформації про тест
        /// </summary>
        /// <remarks>Використовує FileReader для зчитування даних з бази</remarks>
        /// <param name="testTitle">Назва тесту, допускається нетранслітерована</param>
        /// <returns>Зповнену структуру Test.TestInfo або порожню у випадку відсутності даних</returns>
        public static Test.TestInfo GetTestInfo(string testTitle)
        {
            try
            {
                FileReader reader = new FileReader(testTitle);
                string[] stringToSplit = reader.ReadAndReturnTestInfo().Split(new char[] { '₴' }, StringSplitOptions.RemoveEmptyEntries);
                // Якщо зчитаний рядок містить недостатньо інформації, кидаємо помилку
                if (stringToSplit.Length < 3) throw new FormatException();
                // Створення структури Test.TestInfo та її заповнення відповідними даними
                Test.TestInfo currentTestInfo;
                currentTestInfo.testTitle = stringToSplit[0];
                currentTestInfo.lastEditedTime = DateTime.Parse(stringToSplit[1]);
                int timerValue = 0;
                bool timerIsSet = int.TryParse(stringToSplit[2], out timerValue);
                currentTestInfo.timerValue = timerValue;
                return currentTestInfo;
            }
            catch (FormatException)
            {
                MessageBox.Show("Помилка! Дані з бази даних некоректні!");
                // Створення "порожньої" структури Test.TestInfo
                Test.TestInfo nullTestInfo;
                nullTestInfo.testTitle = "null";
                nullTestInfo.lastEditedTime = DateTime.Now;
                nullTestInfo.timerValue = 0;
                return nullTestInfo;
            }
        }
        /// <summary>
        /// Транслітерує кириличні символи та замінює інші символи для оптимізації шляхів до баз даних
        /// </summary>
        /// <remarks>Використовує transliterationTable для заміни символів</remarks>
        /// <param name="inputString">Рядок, який потрібно транслітерувати</param>
        /// <returns>Оптимізований для використання як шляху до баз даних рядок</returns>
        public static string TransliterateAString(string inputString)
        {
            string transliteratedString = string.Empty;
            foreach(char c in inputString.ToLower())
            {
                // Якщо символ є у словнику, замінюємо його, інакше залишаємо як є
                transliteratedString = transliterationTable.ContainsKey(c) ? 
                    string.Concat(transliteratedString, transliterationTable[c])
                    : transliteratedString = string.Concat(transliteratedString, c);
            }
            return transliteratedString;
        }
        /// <summary>
        /// Метод, що повертає всі запитання всіх існуючих тестів
        /// </summary>
        /// <remarks>Використовується для сорту та групування запитань</remarks>
        /// <returns>Список всіх запитань</returns>
        /// <param name="transliteratedTitles">Список всіх назв тестів (транслітерованих)</param>
        public static List<Test.Question> GetAllQuestions(List<string> transliteratedTitles)
        {
            List<Test.Question> listToReturn = new List<Test.Question>();
            foreach (string transliteratedTitle in transliteratedTitles)
            {
                List<Test.Question> tempQuestionsList = FormQuestionsList(transliteratedTitle);
                listToReturn.AddRange(tempQuestionsList);
            }
            return listToReturn;
        }
        /// <summary>
        /// Метод, що повертає всі загальні дані тестів
        /// </summary>
        /// <remarks>Використовується для сорту та групування тестів</remarks>
        /// <returns>Список всіх даних тестів</returns>
        /// <param name="transliteratedTitles">Список всіх назв тестів (транслітерованих)</param>
        public static List<Test.TestInfo> GetAllTestInfos(List<string> transliteratedTitles)
        {
            List<Test.TestInfo> listToReturn = new List<Test.TestInfo>();
            foreach (string transliteratedTitle in transliteratedTitles)
            {
                Test.TestInfo currentTestInfo = GetTestInfo(transliteratedTitle);
                listToReturn.Add(currentTestInfo);
            }
            return listToReturn;
        }
        /// <summary>
        /// Видаляє директорію бази даних заданого тесту
        /// </summary>
        /// <remarks>Видаляє і директорію, і саму базу даних в ній</remarks>
        /// <param name="testTitle">Назва тесту, допускається нетранслітерована</param>
        public static void EraseFolder(string testTitle)
        {
            FileReader reader = new FileReader(testTitle);
            if (reader.PathExists())
            {
                Directory.Delete(reader.DirectoryPath, true);
            }
        }
    }
}
