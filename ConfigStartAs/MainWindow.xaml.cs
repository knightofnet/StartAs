using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using AryxDevLibrary.utils;
using ConfigStartAs.utils;
using ConfigStartAs.view;
using Microsoft.Win32;
using StartAsCore.constant;
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
        private bool _isNewFile = true;


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
                Filter = Properties.Resources.openForFilePathFilenameFilter,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)

            };

            if (openFileDialog.ShowDialog() != true) return;

            AdaptUiAtExecFilepath(openFileDialog.FileName);
        }



        private void btnCreateCryptFile_Click(object sender, RoutedEventArgs e)
        {
            if (!VerifForm())
            {
                return;
            }

            FileInfo execPath = new FileInfo(tbExecPath.Text.Trim('"'));
            if (!execPath.Exists)
            {
                MessageBox.Show(
                    string.Format(Properties.Resources.msgTxtFileNotExist, execPath.FullName), Properties.Resources.msgTxtError,
                    MessageBoxButton.OK, MessageBoxImage.Error);
                tbExecPath.Focus();
                return;
            }

            AuthentFile aFile = new AuthentFile
            {
                Filepath = execPath.FullName,
                Arguments = tbArgs.Text,
                WorkingDirectory = tbWdir.Text.Trim('"'),
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
                string parentDirCrtFile = Path.GetDirectoryName(tbCryptFilePath.Text.Trim('"'));
                if (_isNewFile && parentDirCrtFile != null && !Directory.Exists(parentDirCrtFile))
                {
                    Directory.CreateDirectory(parentDirCrtFile);
                }
                AuthentFileUtils.CreateFile(aFile, tbCryptFilePath.Text.Trim('"'));

                _isNewFile = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(Properties.Resources.msgTxtErrorCreateAuthentFile, Properties.Resources.msgTxtError, MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void btnNewCrptFile_Click(object sender, RoutedEventArgs e)
        {
            string authentFilepath = tbCryptFilePath.Text.Trim('"');
            if (!_isNewFile)
            {
                MessageBoxResult result = MessageBox.Show(
                    string.Format(
                        Properties.Resources.msgTxtChangesPending,
                        authentFilepath), Properties.Resources.msgTxtQuestion,
                    MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No
                );
                if (result == MessageBoxResult.No)
                {
                    return;
                }
            }

            ResetForms();
            _isNewFile = true;
        }

        private void btnOpenCrptFile_Click(object sender, RoutedEventArgs e)
        {
            string authentFilepath = tbCryptFilePath.Text.Trim('"');
            if (!_isNewFile)
            {
                MessageBoxResult result = MessageBox.Show(
                    string.Format(
                        Properties.Resources.msgTxtChangesPending,
                        authentFilepath), Properties.Resources.msgTxtQuestion,
                    MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No
                );
                if (result == MessageBoxResult.No)
                {
                    return;
                }
            }

            if (String.IsNullOrWhiteSpace(authentFilepath))
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Multiselect = false,
                    Filter = Properties.Resources.openForAuthFileFilter,
                    InitialDirectory = AppDataDir
                };

                if (openFileDialog.ShowDialog() != true) return;
                authentFilepath = openFileDialog.FileName;
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
                    GuiMiscUtils.MsgError(Properties.Resources.msgTxtErrorReadingAuthFile + ".\n" + ex.Message,
                        Properties.Resources.msgTxtError);
                    tbCryptFilePath.Focus();
                    return;
                }
            }

            if (aFile == null) return;

            bool isCanOpenFile = true;
            if (aFile.PasswordSecured.Any())
            {
                isCanOpenFile = false;
                PasswordBoxView pwdView = new PasswordBoxView(aFile.Username);
                pwdView.ShowDialog();
                if (pwdView.Result == MessageBoxResult.OK)
                {
                    isCanOpenFile = pwdView.Password.Equals(aFile.PasswordSecured);
                    if (!isCanOpenFile)
                    {
                        GuiMiscUtils.MsgError(
                            Properties.Resources.msgTxtErrorOpenAuthFilePwd,
                            Properties.Resources.msgTxtError);
                    }
                }
                else
                {
                    GuiMiscUtils.MsgError(
                        Properties.Resources.msgTxtErrorOpenAuthFileGen,
                        Properties.Resources.msgTxtError);
                }
            }

            if (isCanOpenFile)
            {
                AdaptUiFromAuthentFile(aFile);
                tbCryptFilePath.Text = authentFilepath;
                _isNewFile = false;
            }

        }

        private void AdaptUiAtExecFilepath(string execFilepath)
        {
            tbExecPath.Text = execFilepath;
            tbWdir.Text = Path.GetDirectoryName(execFilepath);

            if (StringUtils.IsNullOrWhiteSpace(tbCryptFilePath.Text) || !File.Exists(tbCryptFilePath.Text))
            {
                tbCryptFilePath.Text = Path.Combine(AppDataDir,
                    Path.GetFileNameWithoutExtension(execFilepath) + ".crt");
            }

            SetExeIcon(execFilepath);
        }

        private void AdaptUiFromAuthentFile(AuthentFile aFile)
        {
            _tbExecPathEventState = false;
            tbSecurityPin.IsEnabled = false;
            tbSecurityDate.IsEnabled = false;

            tbExecPath.Text = aFile.Filepath;
            SetExeIcon(aFile.Filepath);
            tbArgs.Text = aFile.Arguments;
            tbWdir.Text = aFile.WorkingDirectory;
            tbUsername.Text = aFile.Username;
            tbPwd.Password = aFile.PasswordSecured;

            chkbPin.IsChecked = aFile.IsAskForPinAtStart;
            tbSecurityPin.Text = aFile.IsAskForPinAtStart ? aFile.PinStart : string.Empty;
            tbSecurityPin.IsEnabled = aFile.IsAskForPinAtStart;

            chkbHaveExpirationDate.IsChecked = aFile.IsHaveExpirationDate;
            tbSecurityDate.SelectedDate = aFile.IsHaveExpirationDate ? aFile.ExpirationDate : null;

            chkbVerif.IsChecked = aFile.IsDoSha1VerifAtStart;

            _tbExecPathEventState = true;
        }

        private void SetExeIcon(string execFilepath)
        {
            if (execFilepath == null)
            {
                exeIcon.Source = null;
                return;
            }
            if (File.Exists(execFilepath))
            {
                using (System.Drawing.Icon icon = System.Drawing.Icon.ExtractAssociatedIcon(execFilepath))
                {
                    exeIcon.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                        icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                }
            }
        }

        private void ResetForms()
        {
            _tbExecPathEventState = false;

            tbSecurityPin.IsEnabled = false;
            tbSecurityDate.IsEnabled = false;

            tbExecPath.Clear();
            SetExeIcon(null);
            tbArgs.Clear();
            tbWdir.Clear();
            tbUsername.Clear();
            tbPwd.Password = string.Empty;

            chkbPin.IsChecked = false;
            tbSecurityPin.Clear();
            tbSecurityPin.IsEnabled = false;

            chkbHaveExpirationDate.IsChecked = false;
            tbSecurityDate.SelectedDate = null;

            chkbVerif.IsChecked = false;

            tbCryptFilePath.Clear();

            _tbExecPathEventState = true;
        }

        private bool VerifForm()
        {
            string tmpA = tbExecPath.Text;
            if (!string.IsNullOrWhiteSpace(tmpA))
            {
                if (!File.Exists(tmpA))
                {
                    GuiMiscUtils.MsgError(string.Format(Properties.Resources.msgTxtFileNotExist, tmpA), Properties.Resources.msgTxtError);
                    tbExecPath.Focus();
                    return false;
                }
            }
            else
            {
                GuiMiscUtils.MsgError(Properties.Resources.strNoFileExec, Properties.Resources.msgTxtError);
                tbExecPath.Focus();
                return false;
            }

            tmpA = tbWdir.Text;
            if (!string.IsNullOrWhiteSpace(tmpA))
            {
                if (!Directory.Exists(tmpA))
                {
                    GuiMiscUtils.MsgError(string.Format(Properties.Resources.msgTxtDirNotExist, tmpA), Properties.Resources.msgTxtError);
                    tbWdir.Focus();
                    return false;
                }
            }

            return true;
        }

        private void hlinkCreateShortcut_Click(object sender, RoutedEventArgs e)
        {
            if (_isNewFile) return;

            ShortcutCreator shortcutCreator = new ShortcutCreator(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "StartAsCmd.exe"),
                $"Raccourci vers {Path.GetFileNameWithoutExtension(tbExecPath.Text)}",
                Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)
                );
            shortcutCreator.Args = $"-{CmdArgsOptions.OptAuthentFile.ShortOpt} \"{tbCryptFilePath.Text}\"";
            shortcutCreator.IconFile = $"{tbExecPath.Text}";

            shortcutCreator.CreateShortcut();
        }
    }
}
