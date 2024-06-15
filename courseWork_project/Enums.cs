namespace courseWork_project
{
    #region Grouping related
    /// <summary>
    /// Enum for types of tests grouping
    /// </summary>
    public enum TestGroupOption
    {
        TIMER_EXISTANCE,
        EDITED_TODAY,
        CONTROL_WORK
    }
    /// <summary>
    /// Enum for types of QuestionMetadatas grouping
    /// </summary>
    public enum QuestionGroupOption
    {
        WITH_IMAGE,
        ALL_VARIANTS_CORRECT
    }
    #endregion

    #region Sorting related
    /// <summary>
    /// Enum for types of tests sorting
    /// </summary>
    public enum TestSortOption
    {
        BY_DATE,
        BY_TIMER,
        BY_QUESTIONS_COUNT,
        BY_TITLE
    }
    /// <summary>
    /// Enum for types of QuestionMetadatas sorting
    /// </summary>
    public enum QuestionSortOption
    {
        BY_VARIANTS_COUNT,
        BY_CORRECT_COUNT,
        BY_QUESTION_LENGTH,
        BY_QUESTION_TITLE
    }
    #endregion

    #region HelpCenter_Window related
    /// <summary>
    /// Enum for MainWindow related manuals
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
    /// Enum for test passing related manuals
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
    /// Enum for TestChange_Window related manuals
    /// </summary>
    public enum TestChangeManuals
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
    /// Enum for TestSaving_Window related manuals
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
}
