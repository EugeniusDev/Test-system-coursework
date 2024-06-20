using System.Collections.Generic;
using static courseWork_project.TestStructs;

namespace courseWork_project
{
    internal static class WindowCaller
    {
        public static void ShowMain()
        {
            MainMenu_Window mainWindow = new MainMenu_Window();
            mainWindow.Show();
        }

        public static void ShowHelpCenter()
        {
            UserManuals_Window helpCenter = new UserManuals_Window();
            helpCenter.Show();
        }

        public static void ShowTestChangeCreatingMode()
        {
            TestEdit_Window testEditWindow = new TestEdit_Window();
            testEditWindow.Show();
        }
        // TODO remove "+1" as it is temporary solution before fix
        public static void ShowTestChangeEditingMode(Test testToEdit, int indexOfElementToEdit)
        {
            TestEdit_Window testEditWindow = new TestEdit_Window(testToEdit, indexOfElementToEdit);
            testEditWindow.Show();
        }

        public static void ShowTestSavingCreatingMode(List<QuestionMetadata> questionsToSave)
        {
            TestSave_Window testSaving_Window = new TestSave_Window(questionsToSave);
            testSaving_Window.Show();
        }
        public static void ShowTestSavingEditingMode(Test testToEdit)
        {
            TestSave_Window testSaving_Window = new TestSave_Window(testToEdit);
            testSaving_Window.Show();
        }

        public static void ShowNameEntry(Test testToPass)
        {
            NameEntry_Window nameEntry_Window = new NameEntry_Window(testToPass);
            nameEntry_Window.Show();
        }

        public static void ShowTestTaking(Test testToPass, string userName)
        {
            TestPass_Window testTaking_Window = new TestPass_Window(testToPass, userName);
            if (testTaking_Window.LoadedSuccessfully)
            {
                testTaking_Window.Show();
            }
        }
    }
}
