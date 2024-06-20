using System.Collections.Generic;
using static courseWork_project.TestStructs;

namespace courseWork_project
{
    internal static class WindowCaller
    {
        public static void ShowMain()
        {
            MainMenu_Window mainMenu = new MainMenu_Window();
            mainMenu.Show();
        }

        public static void ShowHelpCenter()
        {
            UserManuals_Window userManual = new UserManuals_Window();
            userManual.Show();
        }

        public static void ShowTestChangeCreatingMode()
        {
            TestEdit_Window testEdit = new TestEdit_Window();
            testEdit.Show();
        }

        public static void ShowTestChangeEditingMode(Test testToEdit, int indexOfElementToEdit)
        {
            TestEdit_Window testEdit = new TestEdit_Window(testToEdit, indexOfElementToEdit);
            testEdit.Show();
        }

        public static void ShowTestSavingCreatingMode(List<QuestionMetadata> questionsToSave)
        {
            TestSave_Window testSave = new TestSave_Window(questionsToSave);
            testSave.Show();
        }
        public static void ShowTestSavingEditingMode(Test testToEdit)
        {
            TestSave_Window testSave = new TestSave_Window(testToEdit);
            testSave.Show();
        }

        public static void ShowUsernamePrompt(Test testToPass)
        {
            PromptUsername_Window usernameWindow = new PromptUsername_Window(testToPass);
            usernameWindow.Show();
        }

        public static void ShowTestTaking(Test testToPass, string userName)
        {
            TestPass_Window testPass = new TestPass_Window(testToPass, userName);
            if (testPass.LoadedSuccessfully)
            {
                testPass.Show();
            }
        }
    }
}
