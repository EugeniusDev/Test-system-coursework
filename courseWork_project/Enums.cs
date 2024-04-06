namespace courseWork_project
{
    #region Grouping related
    /// <summary>
    /// Enum для визначення типу групування тестів
    /// </summary>
    public enum TestGroupTypes
    {
        TIMER_EXISTANCE,
        EDITED_TODAY,
        CONTROL_WORK
    }
    /// <summary>
    /// Enum для визначення типу групування запитань тестів
    /// </summary>
    public enum QuestionGroupTypes
    {
        WITH_IMAGE,
        ALL_VARIANTS_CORRECT
    }
    #endregion

    #region HelpCenter_Window related
    /// <summary>
    /// Enum для організації посібників, що стосуються вікна MainWindow
    /// </summary>
    public enum MainWindowManuals
    {
        TEST_CREATING,
        QUESTION_SEARCH,
        ANSWER_SEARCH,
        TEST_GROUP,
        QUESTION_GROUP,
        TEST_SORT,
        QUESTION_SORT,
        TEST_PASSING,
        TEST_EDITING,
        RESULTS_VIEWING,
        TEST_DELETING
    }
    /// <summary>
    /// Enum для організації посібників, що стосуються проходження тесту
    /// </summary>
    public enum TestPassingManuals
    {
        TEST_BEGIN,
        QUESTION_CHOOSE,
        QUESTION_NEXT,
        BACK_TO_MAIN,
        TEST_END
    }
    /// <summary>
    /// Enum для організації посібників, що стосуються вікна TestChange
    /// </summary>
    public enum TestChangeManuls
    {
        QUESTION_ENTRY,
        VARIANT_ADD,
        VARIANT_MARK,
        VARIANT_DELETE,
        QUESTION_IMAGE,
        QUESTION_NEXT,
        QUESTION_PREVIOUS,
        TO_SAVING,
        BACK_TO_MAIN
    }
    /// <summary>
    /// Enum для організації посібників, що стосуються вікна TestSaving
    /// </summary>
    public enum TestSavingManuals
    {
        TITLE_ENTRY,
        TIMER_ENTRY,
        QUESTION_EDITING,
        QUESTION_DELETING,
        TEST_SAVING
    }
    #endregion

    #region Sorting related
    /// <summary>
    /// Enum для визначення типу сортування тестів
    /// </summary>
    public enum TestSortTypes
    {
        BY_DATE,
        BY_TIMER,
        BY_QUESTIONS_COUNT,
        BY_TITLE
    }
    /// <summary>
    /// Enum для визначення типу сортування запитань тестів
    /// </summary>
    public enum QuestionSortTypes
    {
        BY_VARIANTS_COUNT,
        BY_CORRECT_COUNT,
        BY_QUESTION_LENGTH,
        BY_QUESTION_TITLE
    }
    #endregion

}
