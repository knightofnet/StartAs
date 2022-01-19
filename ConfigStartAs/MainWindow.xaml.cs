using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AryxDevLibrary.utils;
using Microsoft.Win32;
using StartAsCore.dto;
using StartAsCore.utils;
using Path = System.IO.Path;

namespace ConfigStartAs
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly string AppDataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "StartAs");
        private bool _tbExecPathEventState;

        public MainWindow()
        {
            InitializeComponent();
            tbExecPath.TextChanged += (sender, args) =>
            {
                if (!_tbExecPathEventState) return;

                if (!String.IsNullOrEmpty(tbExecPath.Text) && tbExecPath.Text.StartsWith("\"") &&
                    tbExecPath.Text.EndsWith("\""))
                {
                    tbExecPath.Text = tbExecPath.Text.Substring(1, tbExecPath.Text.Length - 2);
                }

                if (File.Exists(tbExecPath.Text))
                {
                    AdaptUiAtExecFilepath(tbExecPath.Text);
                }
            };

            tbSecurityPin.IsEnabled = false;
            tbSecurityDate.IsEnabled = false;
            tbSecurityDate.DisplayDateStart = DateTime.Now;
            


            chkbPin.Click += (sender, args) =>
            {
                tbSecurityPin.IsEnabled = chkbPin.IsChecked ?? false;
            };
            chkbHaveExpirationDate.Click += (sender, args) =>
            {
                tbSecurityDate.IsEnabled = chkbHaveExpirationDate.IsChecked ?? false;
            };
        }

        private void btnChangeExePath_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = false,
                Filter = "Application|*.exe|Tous les fichiers|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)

            };

            if (openFileDialog.ShowDialog() != true) return;

            AdaptUiAtExecFilepath(openFileDialog.FileName);
        }

        private void AdaptUiAtExecFilepath(string execFilepath)
        {
            tbExecPath.Text = execFilepath;
            tbWdir.Text = Path.GetDirectoryName(execFilepath);
            tbCryptFilePath.Text = Path.Combine(AppDataDir,
                Path.GetFileNameWithoutExtension(execFilepath) + ".crt");
        }

        private void btnCreateCryptFile_Click(object sender, RoutedEventArgs e)
        {
            FileInfo execPath = new FileInfo(tbExecPath.Text);
            if (!execPath.Exists)
            {
                MessageBox.Show($"Le fichier '{execPath.FullName}' n'existe pas ou n'est pas accessible.", "Erreur",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                tbExecPath.Focus();
                return;
            }

            AuthentFile aFile = new AuthentFile
            {
                Filepath = execPath.FullName,
                Arguments = tbArgs.Text,
                WorkingDirectory = tbWdir.Text,
                AuthentFileDateCreated = DateTime.Now,
                Username = tbUsername.Text,
                PasswordSecured = tbPwd.Password,
                WindowStyleToLaunch = (ProcessWindowStyle)((ComboBoxItem)cbWindowStart.SelectedItem).Tag,
                IsDoSha1VerifAtStart = chkbVerif.IsChecked ?? false,
                IsAskForPinAtStart = chkbPin.IsChecked ?? false,
                PinStart = tbSecurityPin.Text,
                IsHaveExpirationDate = chkbHaveExpirationDate.IsChecked ?? false,
                ExpirationDate = tbSecurityDate.SelectedDate
            };


            try
            {
                AuthentFileUtils.CreateFile(aFile, tbCryptFilePath.Text);
            } catch (Exception ex)
            {
                MessageBox.Show("Erreur lors de la création du fichier d'authentification.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);

            }

   



        }

        private void btnSaveCrptFile_Click(object sender, RoutedEventArgs e)
        {
            string authentFilepath = tbCryptFilePath.Text;
            if (!String.IsNullOrEmpty(authentFilepath) && authentFilepath.StartsWith("\"") &&
                authentFilepath.EndsWith("\""))
            {
                authentFilepath = authentFilepath.Substring(1, authentFilepath.Length - 2);
            }

            AuthentFile aFile = null;
            if (File.Exists(authentFilepath))
            {
                try
                {
                    aFile = AuthentFileUtils.CryptAuthenDtoFromFile(authentFilepath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erreur lors de la lecture du fichier d'authentification.\n"+ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    tbCryptFilePath.Focus();
                    return;
                }
            }

            if (aFile == null) return;

            AdaptUiFromAuthentFile(aFile);
        }

        private void AdaptUiFromAuthentFile(AuthentFile aFile)
        {
            _tbExecPathEventState = false;
            tbSecurityPin.IsEnabled = false;
            tbSecurityDate.IsEnabled = false;

            tbExecPath.Text = aFile.Filepath;
            tbArgs.Text = aFile.Arguments;
            tbWdir.Text = aFile.WorkingDirectory;
            tbUsername.Text = aFile.Username;
            tbPwd.Password = aFile.PasswordSecured;

            chkbPin.IsChecked = aFile.IsAskForPinAtStart;
            tbSecurityPin.Text = aFile.IsAskForPinAtStart ? aFile.PinStart.ToString() : string.Empty;
            tbSecurityPin.IsEnabled = aFile.IsAskForPinAtStart;

            chkbHaveExpirationDate.IsChecked = aFile.IsHaveExpirationDate;
            tbSecurityDate.SelectedDate = aFile.IsHaveExpirationDate ? aFile.ExpirationDate : null;

            chkbVerif.IsChecked = aFile.IsDoSha1VerifAtStart;

            _tbExecPathEventState = true;
        }
    }
}
