using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace courseWork_project
{
    /// <summary>
    /// Interaction logic for HelpCenter_Window.xaml. This window plays a role of user manual
    /// </summary>
    /// <remarks>HelpCenter_Window (user manual) is called from any other window</remarks>
    public partial class HelpCenter_Window : Window
    {
        private static readonly Dictionary<MainWindowManuals, string> mainWindowManualMessages =
            new Dictionary<MainWindowManuals, string>()
        {
            { MainWindowManuals.TEST_CREATING, "Щоб створити новий тест, клацніть на кнопку " +
                    "\"Створити тест\" у верхній частині головної сторінки. Ця дія відкриє " +
                    "вікно створення тесту." },
            { MainWindowManuals.QUESTION_SEARCH, "Для пошуку запитання тесту введіть запитання," +
                    " що хочете знайти в поле з підписом \"Пошук запитання тесту\". " +
                    "Потім натисніть кнопку \"Знайти\" поруч з цим полем."},
            { MainWindowManuals.ANSWER_SEARCH, "Для пошуку варіанту відповіді введіть варіант " +
                    "відповіді, що хочете знайти в поле з підписом \"Пошук відповіді тесту\". " +
                    "Потім натисніть кнопку \"Знайти\" поруч з цим полем."},
            { MainWindowManuals.TEST_GROUP, "Для групування тестів клацніть на випадаюче меню, " +
                    "яке підписане текстом \"Групування тестів\" над ним. " +
                    "В меню оберіть опцію, за якою бажаєте згрупувати тести."},
            { MainWindowManuals.QUESTION_GROUP, "Для групування запитань клацніть на випадаюче " +
                    "меню, яке підписане текстом \"Групування запитань\" над ним. " +
                    "В меню оберіть опцію, за якою бажаєте згрупувати запитання."},
            { MainWindowManuals.TEST_SORT, "Для сортування тестів клацніть на випадаюче меню, " +
                    "яке підписане текстом \"Сортування тестів\" над ним. " +
                    "В меню оберіть опцію, за якою бажаєте посортувати тести."},
            { MainWindowManuals.QUESTION_SORT, "Для сортування запитань клацніть на випадаюче меню, " +
                    "яке підписане текстом \"Сортування запитань\" над ним. " +
                    "В меню оберіть опцію, за якою бажаєте посортувати запитання."},
            { MainWindowManuals.TEST_PASSING, "Для проходження обраного тесту клацніть на кнопку " +
                    "\"Пройти\" в списку тестів навпроти " +
                    "бажаного до проходження тесту. Ця дія відкриє вікно проходження тесту."},
            { MainWindowManuals.TEST_EDITING, "Для редагування обраного тесту клацніть на кнопку " +
                    "\"Редагувати\" в списку тестів навпроти " +
                    "бажаного до редагування тесту. Ця дія відкриє вікно збереження тесту."},
            { MainWindowManuals.RESULTS_VIEWING, "Для перегляду результатів проходження обраного " +
                    "тесту клацніть на кнопку \"Переглянути\" в " +
                    "списку тестів навпроти бажаного до перегляду результатів проходження тесту." },
            { MainWindowManuals.TEST_DELETING, "Для видалення обраного тесту клацніть на кнопку " +
                    "\"Видалити\" в списку тестів навпроти бажаного до видалення тесту. Далі необхідно " +
                    "підтвердити бажання видалення обраного тесту."}
        };
        private static readonly Dictionary<TestPassingManuals, string> testPassingManualMessages =
            new Dictionary<TestPassingManuals, string>()
            {
                { TestPassingManuals.TEST_BEGIN, "Як розпочати тест?\nВведіть Ваше ім'я в поле " +
                    "\"Введіть ім'я тут\", клацніть на кнопку \"Розпочати тест\". " +
                    "Після прочитання інформації про тест натисніть \"ОК\". Ця дія розпочне " +
                    "спробу проходження тесту." },
                { TestPassingManuals.QUESTION_CHOOSE, "Для того, щоб обрати варіант відповіді, " +
                    "клацніть на кнопку з текстом варіанту відповіді, " +
                    "що здається Вам правильною. Після цього правильні відповіді відобразяться " +
                    "зеленим кольором. Якщо ви обрали неправильний варіант, кнопка змінить колір " +
                    "на червоний."},
                { TestPassingManuals.QUESTION_NEXT, "Щоб перейти до наступного запитання тесту, " +
                    "клацніть на кнопку \"Наступне запитання\", що знаходиться в правому нижньому " +
                    "куті вікна проходження тесту."},
                { TestPassingManuals.BACK_TO_MAIN, "Щоб повернутися до головної сторінки, клацніть на " +
                    "кнопку \"До головної сторінки\", що знаходиться в лівому верхньому куті вікна " +
                    "проходження тесту. Зауважте, що ця дія призведе до втрати даних проходження" +
                    " тесту. При підтвердження бажання повернення відкриється головна сторінка. " +
                    "При відмові потрібно буде заново проходити тест."},
                { TestPassingManuals.TEST_END, "Щоб завершити спробу проходження тесту, клацніть на " +
                    "кнопку \"Завершити проходження тесту\", що знаходиться в центрі нижньої частини " +
                    "вікна проходження тесту."}
            };
        private static readonly Dictionary<TestChangeManuals, string> testChangeManualMessages =
            new Dictionary<TestChangeManuals, string>()
            {
                { TestChangeManuals.QUESTION_ENTRY, "Для задання чи редагування запитання користуйтеся " +
                    "полем \"Введіть Ваше запитання\"." },
                { TestChangeManuals.VARIANT_ADD, "Щоб додати новий варіант відповіді, клацніть на " +
                    "кнопку \"Додати варіант відповіді\", після чого заповніть поле, що з'явилось " +
                    "в центральній частині вікна." },
                { TestChangeManuals.VARIANT_MARK, "Щоб позначити варіант як правильний (або забрати " +
                    "це позначення), клацніть на квадрат, що знаходиться справа від варіанту " +
                    "відповіді та підписаний текстом \"Правильний\"." },
                { TestChangeManuals.VARIANT_DELETE, "Щоб видалити останній варіант відповіді, " +
                    "клацніть на кнопку \"Видалити останній варіант\"." },
                { TestChangeManuals.QUESTION_IMAGE, "Щоб додати/змінити ілюстрацію до запитання " +
                    "тесту, клацніть на кнопку \"Клацніть, щоб змінити картинку\". Відкриється " +
                    "діалогове вікно. Оберіть картинку з допомогою нього, і тоді вона відобразиться" +
                    " поверх кнопки. Для видалення ілюстрації з поточного запитання, клацніть на " +
                    "кнопку \"Видалити картинку\"." },
                { TestChangeManuals.QUESTION_NEXT, "Щоб перейти до наступного запитання тесту (або" +
                    " щоб створити нове), клацніть на кнопку \"Наступне запитання\", " +
                    "що знаходиться в правому нижньому куті вікна проходження тесту." },
                { TestChangeManuals.QUESTION_PREVIOUS, "Щоб перейти до попереднього запитання тесту," +
                    " клацніть на кнопку \"Попереднє запитання\", що знаходиться в лівому нижньому " +
                    "куті вікна проходження тесту." },
                { TestChangeManuals.TO_SAVING, "Клацніть на кнопку \"Закінчити та зберегти тест\"." +
                    " Ця дія відкриє вікно збереження тесту. За потреби, до редагування тесту можна " +
                    "повернутись з вікна збереження тесту." },
                { TestChangeManuals.BACK_TO_MAIN, "Щоб повернутися до головної сторінки, клацніть " +
                    "на кнопку \"До головної сторінки\", що знаходиться в лівому верхньому куті" +
                    " вікна проходження тесту і підтвердьте бажання переходу до головної сторінки." +
                    " Зауважте, що ця дія призведе до втрати даних тесту." }
            };
        private static readonly Dictionary<TestSavingManuals, string> testSavingManualMessages =
            new Dictionary<TestSavingManuals, string>()

            {
                { TestSavingManuals.TITLE_ENTRY, "Для введення чи редагування назви тесту " +
                    "використовуйте поле \"Введіть назву тесту\"" },
                { TestSavingManuals.TIMER_ENTRY, "Для введення чи редагування обмеження в часі " +
                    "проходження тесту використовуйте поле, що підписане текстом \"Обмеження в часі" +
                    " (у хв.)\" над ним. Зауважте, що значення повинне бути цілим числом. " +
                    "За замовчуванням значення обмеження дорівнює 0, що означає, що час проходження" +
                    " тесту необмежений."},
                { TestSavingManuals.QUESTION_EDITING, "Для редагування обраного запитання клацніть" +
                    " на кнопку \"Редагувати\" в списку запитань навпроти " +
                    "бажаного до редагування запитання. Ця дія відкриє вікно редагування тесту " +
                    "на обраному запитанні."},
                { TestSavingManuals.QUESTION_DELETING, "Для видалення обраного запитання клацніть " +
                    "на кнопку \"Видалити\" в списку запитань навпроти " +
                    "бажаного до видалення запитання. Ця дія відкриє вікно видалення тесту на " +
                    "обраному запитанні."},
                { TestSavingManuals.TEST_SAVING, "Щоб зберегти тест, клацніть кнопку \"Зберегти тест" +
                    " та повернутись до головної сторінки\". Ця дія збереже тест, " +
                    "сповістить Вас про вдале записання даних тесту та відкриє головну сторінку. " +
                    "За потреби, до редагування тесту можна повернутись з головної сторінки."}
            };

        public HelpCenter_Window()
        {
            InitializeComponent();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
            {
                Close();
            }
        }

        private void MainWindowCombobox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            UpdateInstructions(mainWindowManualMessages[(MainWindowManuals)
                MainWindowCombobox.SelectedIndex]);
        }
        private void TestPassingCombobox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            UpdateInstructions(testPassingManualMessages[(TestPassingManuals)
                TestPassingCombobox.SelectedIndex]);
        }
        private void CreationEditingCombobox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            UpdateInstructions(testChangeManualMessages[(TestChangeManuals)
                CreationEditingCombobox.SelectedIndex]);
        }
        private void TestSavingCombobox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            UpdateInstructions(testSavingManualMessages[(TestSavingManuals)
                TestSavingCombobox.SelectedIndex]);
        }

        private void UpdateInstructions(string text)
        {
            InfoText.Text = text;
        }
    }
}
