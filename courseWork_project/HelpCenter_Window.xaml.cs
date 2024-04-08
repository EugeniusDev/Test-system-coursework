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
        /// <summary>
        /// Current instructions stored in this variable
        /// </summary>
        string infoToWrite = string.Empty;
        /// <summary>
        /// Parameterless constructor for HelpCenter_Window
        /// </summary>
        public HelpCenter_Window()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Handling pressed Escape key
        /// </summary>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
            {
                Close();
            }
        }
        /// <summary>
        /// Handling choosing option from MainWindowCombobox
        /// </summary>
        private void MainWindowCombobox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            switch((MainWindowManuals)MainWindowCombobox.SelectedIndex)
            {
                case MainWindowManuals.TEST_CREATING:
                    infoToWrite = "Щоб створити новий тест, клацніть на кнопку \"Створити тест\" у верхній частині головної сторінки. Ця дія відкриє вікно створення тесту.";
                    break;
                case MainWindowManuals.QUESTION_SEARCH:
                    infoToWrite = "Для пошуку запитання тесту введіть запитання, що хочете знайти в поле з підписом \"Пошук запитання тесту\". " +
                        "Потім натисніть кнопку \"Знайти\" поруч з цим полем.";
                    break;
                case MainWindowManuals.ANSWER_SEARCH:
                    infoToWrite = "Для пошуку варіанту відповіді введіть варіант відповіді, що хочете знайти в поле з підписом \"Пошук відповіді тесту\". " +
                        "Потім натисніть кнопку \"Знайти\" поруч з цим полем.";
                    break;
                case MainWindowManuals.TEST_GROUP:
                    infoToWrite = "Для групування тестів клацніть на випадаюче меню, яке підписане текстом \"Групування тестів\" над ним. " +
                        "В меню оберіть опцію, за якою бажаєте згрупувати тести.";
                    break;
                case MainWindowManuals.QUESTION_GROUP:
                    infoToWrite = "Для групування запитань клацніть на випадаюче меню, яке підписане текстом \"Групування запитань\" над ним. " +
                        "В меню оберіть опцію, за якою бажаєте згрупувати запитання.";
                    break;
                case MainWindowManuals.TEST_SORT:
                    infoToWrite = "Для сортування тестів клацніть на випадаюче меню, яке підписане текстом \"Сортування тестів\" над ним. " +
                        "В меню оберіть опцію, за якою бажаєте посортувати тести.";
                    break;
                case MainWindowManuals.QUESTION_SORT:
                    infoToWrite = "Для сортування запитань клацніть на випадаюче меню, яке підписане текстом \"Сортування запитань\" над ним. " +
                        "В меню оберіть опцію, за якою бажаєте посортувати запитання.";
                    break;
                case MainWindowManuals.TEST_PASSING:
                    infoToWrite = "Для проходження обраного тесту клацніть на кнопку \"Пройти\" в списку тестів навпроти " +
                        "бажаного до проходження тесту. Ця дія відкриє вікно проходження тесту.";
                    break;
                case MainWindowManuals.TEST_EDITING:
                    infoToWrite = "Для редагування обраного тесту клацніть на кнопку \"Редагувати\" в списку тестів навпроти " +
                        "бажаного до редагування тесту. Ця дія відкриє вікно збереження тесту.";
                    break;
                case MainWindowManuals.RESULTS_VIEWING:
                    infoToWrite = "Для перегляду результатів проходження обраного тесту клацніть на кнопку \"Переглянути\" в " +
                        "списку тестів навпроти бажаного до перегляду результатів проходження тесту.";
                    break;
                case MainWindowManuals.TEST_DELETING:
                    infoToWrite = "Для видалення обраного тесту клацніть на кнопку \"Видалити\" в списку тестів навпроти " +
                        "бажаного до видалення тесту. Далі необхідно підтвердити бажання видалення обраного тесту.";
                    break;
                default:
                    infoToWrite = "Обрано неправильний варіант.";
                    break;
            }
            InfoText.Text = infoToWrite;
        }
        /// <summary>
        /// Handling choosing option from TestPassingCombobox
        /// </summary>
        private void TestPassingCombobox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            switch((TestPassingManuals)TestPassingCombobox.SelectedIndex)
            {
                case TestPassingManuals.TEST_BEGIN:
                    infoToWrite = "Як розпочати тест?\nВведіть Ваше ім'я в поле \"Введіть ім'я тут\", клацніть на кнопку \"Розпочати тест\". " +
                        "Після прочитання інформації про тест натисніть \"ОК\". Ця дія розпочне спробу проходження тесту.";
                    break;
                case TestPassingManuals.QUESTION_CHOOSE:
                    infoToWrite = "Для того, щоб обрати варіант відповіді, клацніть на кнопку з текстом варіанту відповіді, " +
                        "що здається Вам правильною. Після цього правильні відповіді відобразяться зеленим кольором. " +
                        "Якщо ви обрали неправильний варіант, кнопка змінить колір на червоний.";
                    break;
                case TestPassingManuals.QUESTION_NEXT:
                    infoToWrite = "Щоб перейти до наступного запитання тесту, клацніть на кнопку \"Наступне запитання\", " +
                        "що знаходиться в правому нижньому куті вікна проходження тесту.";
                    break;
                case TestPassingManuals.BACK_TO_MAIN:
                    infoToWrite = "Щоб повернутися до головної сторінки, клацніть на кнопку \"До головної сторінки\", що знаходиться в лівому верхньому куті вікна " +
                        "проходження тесту. Зауважте, що ця дія призведе до втрати даних проходження тесту. При підтвердження бажання повернення " +
                        "відкриється головна сторінка. При відмові потрібно буде заново проходити тест.";
                    break;
                case TestPassingManuals.TEST_END:
                    infoToWrite = "Щоб завершити спробу проходження тесту, клацніть на кнопку \"Завершити проходження тесту\", " +
                        "що знаходиться в центрі нижньої частини вікна проходження тесту.";
                    break;
                default:
                    infoToWrite = "Обрано неправильний варіант.";
                    break;
            }
            InfoText.Text = infoToWrite;
        }
        /// <summary>
        /// Handling choosing option from CreationEditingCombobox
        /// </summary>
        private void CreationEditingCombobox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            switch((TestChangeManuls)CreationEditingCombobox.SelectedIndex)
            {
                case TestChangeManuls.QUESTION_ENTRY:
                    infoToWrite = "Для задання чи редагування запитання користуйтеся полем \"Введіть Ваше запитання\".";
                    break;
                case TestChangeManuls.VARIANT_ADD:
                    infoToWrite = "Щоб додати новий варіант відповіді, клацніть на кнопку \"Додати варіант відповіді\"," +
                        " після чого заповніть поле, що з'явилось в центральній частині вікна.";
                    break;
                case TestChangeManuls.VARIANT_MARK:
                    infoToWrite = "Щоб позначити варіант як правильний (або забрати це позначення)," +
                        " клацніть на квадрат, що знаходиться справа від варіанту відповіді та підписаний текстом \"Правильний\".";
                    break;
                case TestChangeManuls.VARIANT_DELETE:
                    infoToWrite = "Щоб видалити останній варіант відповіді, клацніть на кнопку \"Видалити останній варіант\".";
                    break;
                case TestChangeManuls.QUESTION_IMAGE:
                    infoToWrite = "Щоб додати/змінити ілюстрацію до запитання тесту, клацніть на кнопку \"Клацніть, щоб змінити картинку\". " +
                        "Відкриється діалогове вікно. Оберіть картинку з допомогою нього, і тоді вона відобразиться поверх кнопки. " +
                        "Для видалення ілюстрації з поточного запитання, клацніть на кнопку \"Видалити картинку\".";
                    break;
                case TestChangeManuls.QUESTION_NEXT:
                    infoToWrite = "Щоб перейти до наступного запитання тесту (або щоб створити нове), клацніть на кнопку \"Наступне запитання\", " +
                        "що знаходиться в правому нижньому куті вікна проходження тесту.";
                    break;
                case TestChangeManuls.QUESTION_PREVIOUS:
                    infoToWrite = "Щоб перейти до попереднього запитання тесту, клацніть на кнопку \"Попереднє запитання\", " +
                        "що знаходиться в лівому нижньому куті вікна проходження тесту.";
                    break;
                case TestChangeManuls.TO_SAVING:
                    infoToWrite = "Клацніть на кнопку \"Закінчити та зберегти тест\". Ця дія" +
                        " відкриє вікно збереження тесту. За потреби, до редагування тесту можна повернутись з вікна збереження тесту.";
                    break;
                case TestChangeManuls.BACK_TO_MAIN:
                    infoToWrite = "Щоб повернутися до головної сторінки, клацніть на кнопку \"До головної сторінки\", що знаходиться в лівому верхньому куті вікна " +
                        "проходження тесту і підтвердьте бажання переходу до головної сторінки. Зауважте, що ця дія призведе до втрати даних тесту.";
                    break;
                default:
                    infoToWrite = "Обрано неправильний варіант.";
                    break;
            }
            InfoText.Text = infoToWrite;
        }
        /// <summary>
        /// Handling choosing option from TestSavingCombobox
        /// </summary>
        private void TestSavingCombobox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            switch((TestSavingManuals)TestSavingCombobox.SelectedIndex)
            {
                case TestSavingManuals.TITLE_ENTRY:
                    infoToWrite = "Для введення чи редагування назви тесту використовуйте поле \"Введіть назву тесту\"";
                    break;
                case TestSavingManuals.TIMER_ENTRY:
                    infoToWrite = "Для введення чи редагування обмеження в часі проходження тесту використовуйте поле, " +
                        "що підписане текстом \"Обмеження в часі (у хв.)\" над ним. Зауважте, що значення повинне бути цілим числом. " +
                        "За замовчуванням значення обмеження дорівнює 0, що означає, що час проходження тесту необмежений.";
                    break;
                case TestSavingManuals.QUESTION_EDITING:
                    infoToWrite = "Для редагування обраного запитання клацніть на кнопку \"Редагувати\" в списку запитань навпроти " +
                        "бажаного до редагування запитання. Ця дія відкриє вікно редагування тесту на обраному запитанні.";
                    break;
                case TestSavingManuals.QUESTION_DELETING:
                    infoToWrite = "Для видалення обраного запитання клацніть на кнопку \"Видалити\" в списку запитань навпроти " +
                        "бажаного до видалення запитання. Ця дія відкриє вікно видалення тесту на обраному запитанні.";
                    break;
                case TestSavingManuals.TEST_SAVING:
                    infoToWrite = "Щоб зберегти тест, клацніть кнопку \"Зберегти тест та повернутись до головної сторінки\". Ця дія збереже тест, " +
                        "сповістить Вас про вдале записання даних тесту та відкриє головну сторінку. За потреби, до редагування " +
                        "тесту можна повернутись з головної сторінки.";
                    break;
                default:
                    infoToWrite = "Обрано неправильний варіант.";
                    break;
            }
            InfoText.Text = infoToWrite;
        }
    }
}
