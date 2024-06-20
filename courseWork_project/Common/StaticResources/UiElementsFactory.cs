using System.Windows;
using System.Windows.Controls;

namespace courseWork_project.GuiManipulation
{
    internal static class UiElementsFactory
    {
        public static TextBox MakeVariantTextbox(string variantText)
        {
            TextBox textBox = new TextBox
            {
                Text = variantText,
                Foreground = ColorBrushes.Black,
                Background = ColorBrushes.VariantBackground,
                FontSize = 24,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
                TextAlignment = TextAlignment.Center,
                TextWrapping = TextWrapping.NoWrap,
                ToolTip = "Варіант відповіді",
                Margin = new Thickness(4),
                MinWidth = 200,
                MaxWidth = 270
            };
            textBox.GotFocus += VariantTextbox_GotFocus;
            textBox.LostFocus += VariantTextbox_LostFocus;

            return textBox;
        }

        private static void VariantTextbox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox;
            if (!IsSenderTextbox(sender, out textBox))
            {
                return;
            }

            bool fieldContainsDefaultText = string.Compare(textBox.Text, 
                "Введіть варіант відповіді") == 0;
            if (fieldContainsDefaultText)
            {
                textBox.Text = string.Empty;
            }
        }

        private static void VariantTextbox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox;
            if (!IsSenderTextbox(sender, out textBox))
            {
                return;
            }

            bool fieldIsEmpty = string.IsNullOrWhiteSpace(textBox.Text);
            if (fieldIsEmpty)
            {
                textBox.Text = "Введіть варіант відповіді";
            }
        }

        private static bool IsSenderTextbox(object sender, out TextBox textBox)
        {
            textBox = sender as TextBox;
            return textBox != null;
        }

        public static CheckBox MakeVariantCheckbox(bool isVariantCorrect)
        {
            return new CheckBox
            {
                IsChecked = isVariantCorrect,
                HorizontalAlignment = HorizontalAlignment.Center,
                FontStyle = FontStyles.Oblique,
                Content = "Правильний",
                ToolTip = "Позначити варіант як правильний",
                Margin = new Thickness(4)
            };
        }

        public static DockPanel MakeVariantDockpanel(TextBox textBox, CheckBox checkBox)
        {
            DockPanel dockPanel = new DockPanel();
            DockPanel.SetDock(textBox, Dock.Left);
            dockPanel.Children.Add(textBox);
            DockPanel.SetDock(checkBox, Dock.Right);
            dockPanel.Children.Add(checkBox);

            return dockPanel;
        }

        public static Button MakeVariantButton(string variantText)
        {
            return new Button
            {
                Content = variantText,
                Foreground = ColorBrushes.Black,
                Background = ColorBrushes.VariantBackground,
                FontSize = 24,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                ToolTip = "Клацніть, щоб обрати цей варіант",
                Margin = new Thickness(3),
                MinWidth = 200,
                MaxWidth = 260
            };
        }
    }
}
