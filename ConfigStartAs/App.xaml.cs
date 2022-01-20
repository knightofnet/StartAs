using System.Globalization;
using System.Windows;

namespace ConfigStartAs
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {



        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            //base.OnStartup(e);

            ConfigStartAs.Properties.Resources.Culture = new CultureInfo("en-US");

            MainWindow m = new MainWindow();
            m.ShowDialog();


        }
    }
}
