using System.Collections.Generic;
using static courseWork_project.TestStructs;

namespace courseWork_project
{
    internal static class WindowCaller
    {
        public static void ShowMain()
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }

        public static void ShowHelpCenter()
        {
            HelpCenter_Window helpCenter = new HelpCenter_Window();
            helpCenter.Show();
        }

        public static void ShowTestChangeCreatingMode()
        {
            TestChange_Window testChange_Window = new TestChange_Window();
            testChange_Window.Show();
        }
        // TODO remove "+1" as it is temporary solution before fix
        public static void ShowTestChangeEditingMode(Test testToEdit, int indexOfElementToEdit)
        {
            TestChange_Window testChange_Window = new TestChange_Window(testToEdit, indexOfElementToEdit);
            testChange_Window.Show();
        }

        public static void ShowTestSavingCreatingMode(List<QuestionMetadata> questionsToSave)
        {
            TestSaving_Window testSaving_Window = new TestSaving_Window(questionsToSave);
            testSaving_Window.Show();
        }
        public static void ShowTestSavingEditingMode(Test testToEdit)
        {
            TestSaving_Window testSaving_Window = new TestSaving_Window(testToEdit);
            testSaving_Window.Show();
        }

        public static void ShowNameEntry(Test testToPass)
        {
            NameEntry_Window nameEntry_Window = new NameEntry_Window(testToPass);
            nameEntry_Window.Show();
        }

        public static void ShowTestTaking(Test testToPass, string userName)
        {
            TestTaking_Window testTaking_Window = new TestTaking_Window(testToPass, userName);
            if (testTaking_Window.LoadedSuccessfully)
            {
                testTaking_Window.Show();
            }
        }
    }
}
