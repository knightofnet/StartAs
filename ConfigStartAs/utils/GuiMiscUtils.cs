using System.Windows;

namespace ConfigStartAs.utils
{
    internal static class GuiMiscUtils
    {

        internal static MessageBoxResult MsgError(string message, string title)
        {
            return MessageBox.Show(message, title,
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
