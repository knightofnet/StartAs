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

        public static MessageBoxResult MsgInfo(string message, string title)
        {
            return MessageBox.Show(message, title,
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static MessageBoxResult MsgWarnYesNo(string message, string title)
        {
            return MessageBox.Show(message, title,
                MessageBoxButton.YesNo, MessageBoxImage.Warning);
        }

        public static string TrimPath(string path)
        {
            return path.Trim(new[] { ' ', '\"' });
        }

        public static string TrimPathExt(this string path)
        {
            return TrimPath(path);
        }
    }
}
