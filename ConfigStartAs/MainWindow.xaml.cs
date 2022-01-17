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
                WindowStyle =  (ProcessWindowStyle)((ComboBoxItem)cbWindowStart.SelectionBoxItem).Tag
                
            };

            if (!AuthentFileUtils.CryptAuthenDtoToFile(aFile, tbCryptFilePath.Text, out var error))
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

            tbExecPath.Text = aFile.Filepath;
            tbArgs.Text = aFile.Arguments;
            tbWdir.Text = aFile.WorkingDirectory;
            tbUsername.Text = aFile.Username;
            tbPwd.Password = aFile.PasswordSecured;

            _tbExecPathEventState = true;
        }
    }
}
