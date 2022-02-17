using System.Linq;
using System.Windows;

namespace ConfigStartAs.view
{
    /// <summary>
    /// Logique d'interaction pour PasswordBoxView.xaml
    /// </summary>
    public partial class PasswordBoxView : Window
    {

        public MessageBoxResult Result { get; set; }

        public string Password { get; set; }

        public PasswordBoxView(string username)
        {
            InitializeComponent();
            lblUsername.Content = username;

            Result = MessageBoxResult.Cancel;

            Loaded += (sender, args) =>
            {
                passwordBox.Focus();
            };
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if (passwordBox.Password.Any())
            {
                Password = passwordBox.Password;
                Result = MessageBoxResult.OK;
                Close();
            }
            else
            {
                MessageBox.Show(Properties.Resources.msgTxtWarnNoPwd,
                    Properties.Resources.lblWinPwdPromptTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
