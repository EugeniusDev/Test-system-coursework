﻿using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace courseWork_project
{
    public static class WindowExtensionMethods
    {
        public static void OpenHelpCenterOnF1(this KeyEventArgs keyEventArgs)
        {
            if (keyEventArgs.Key == Key.F1)
            {
                WindowCaller.ShowHelpCenter();
            }
        }

        public static bool TryGetClosingConfirmation(this CancelEventArgs cancelEventArgs, string additionalPrependMessage = "")
        {
            MessageBoxResult result = MessageBox.Show($"{additionalPrependMessage}Ви справді хочете закрити вікно?",
                "Підтвердження закриття вікна", 
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result.Equals(MessageBoxResult.No))
            {
                // Cancelling closing process
                cancelEventArgs.Cancel = true;

                return false;
            }

            return true;
        }

        public static void CloseWindowAndDisableConfirmationPrompt(this Window windowToClose, ref bool confirmationRequiredVariable)
        {
            confirmationRequiredVariable = false;
            windowToClose.Close();
        }
    }
}
