using System;
using System.Collections.Generic;

namespace courseWork_project
{
    /// <summary>
    /// Клас для визначення структур даних з інформацією про тест
    /// </summary>
    public abstract class Test
    {
        /// <summary>
        /// Структура кожного питання
        /// </summary>
        public struct Question
        {
            /// <summary>
            /// Текст запитання
            /// </summary>
            public string question;
            /// <summary>
            /// Список тексту варіантів відповідей
            /// </summary>
            public List<string> variants;
            /// <summary>
            /// Список індексів правильних варіантів відповідей
            /// </summary>
            public List<int> correctVariantsIndexes;
            /// <summary>
            /// Чи містить запитання ілюстрацію?
            /// </summary>
            public bool hasLinkedImage;
        }
        /// <summary>
        /// Структура загальної інформації про тест
        /// </summary>
        /// <remarks>Назва, дата останнього редагування, значення таймера</remarks>
        public struct TestInfo
        {
            /// <summary>
            /// Назва тесту
            /// </summary>
            public string testTitle;
            /// <summary>
            /// Час останнього редагування
            /// </summary>
            public DateTime lastEditedTime;
            /// <summary>
            /// Значення таймера
            /// </summary>
            /// <remarks>Використовується для обмеження проходження тесту в часі</remarks>
            public int timerValue;
        }
    }
}
