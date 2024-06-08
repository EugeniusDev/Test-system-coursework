using System;

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

        public static void ShowTestChange()
        {
            TestChange_Window testChange_Window = new TestChange_Window();
            testChange_Window.Show();
        }
        // TODO same for overload
        //public static void ShowTestChange()
        //{
        //    TestChange_Window testChange_Window = new TestChange_Window();
        //    testChange_Window.Show();
        //}

        public static void ShowTestSaving(Test testToPass, string userName)
        {
            throw new NotImplementedException();
        }
        // TODO same for overload
        //public static void ShowTestSaving(Test testToPass, string userName)
        //{
        //    throw new NotImplementedException();
        //}

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
