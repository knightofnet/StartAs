using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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

            // Manage tbExecPath events
            tbExecPath.TextChanged += (sender, args) =>
            {
                if (!_tbExecPathEventState) return;

                tbExecPath.Text = tbExecPath.Text.TrimPathExt();

            };
            tbExecPath.KeyUp += (sender, args) =>
            {
                if (args.Key == Key.Enter && File.Exists(tbExecPath.Text))
                {
                    AdaptUiAtExecFilepath(tbExecPath.Text);
                }
            };

            _tbExecPathEventState = true;


            // Manage tbCryptFilePath events
            tbCryptFilePath.TextChanged += (sender, args) =>
            {
                if (!_tbExecPathEventState) return;

                tbCryptFilePath.Text = tbCryptFilePath.Text.TrimPathExt();
            };
            tbCryptFilePath.KeyUp += (sender, args) =>
            {
                if (args.Key == Key.Enter && File.Exists(tbCryptFilePath.Text))
                {
                    OpenAuthentFile(tbCryptFilePath.Text);
                }
            };

            // Drop event
            AllowDrop = true;
            Drop += OnDrop;

            gridAuthentLinks.IsEnabled = false;


            tbSecurityPin.IsEnabled = false;
            tbSecurityDate.IsEnabled = false;
            tbSecurityDate.DisplayDateStart = DateTime.Now;

            btnVerifyUser.IsEnabled = false;
            tbUsername.TextChanged += (sender, args) =>
            {
                btnVerifyUser.IsEnabled = tbUsername.Text.Trim().Any();
            };

            chkbPin.Click += (sender, args) =>
            {
                tbSecurityPin.IsEnabled = chkbPin.IsChecked ?? false;
            };
            chkbHaveExpirationDate.Click += (sender, args) =>
            {
                tbSecurityDate.IsEnabled = chkbHaveExpirationDate.IsChecked ?? false;
            };

            lblVersion.Content = String.Format(lblVersion.Content.ToString(), Assembly.GetExecutingAssembly().GetName().Version);
        }

        private void OnDrop(object sender, DragEventArgs args)
        {
            string[] files = (string[])args.Data.GetData(DataFormats.FileDrop);
            if (files == null || !files.Any()) return;
            string file = files[0];

            if (Path.HasExtension(file) && ".crt".Equals(Path.GetExtension(file).ToLower()))
            {
                OpenAuthentFile(file);
            }
            else
            {
                AdaptUiAtExecFilepath(file);
            }
        }

        private void btnChangeExePath_Click(object sender, RoutedEventArgs e)
        {
            string initDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            string strExecPath = tbExecPath.Text.TrimPathExt();
            if (File.Exists(strExecPath) && Path.GetDirectoryName(strExecPath) != null)
            {
                initDirectory = Path.GetDirectoryName(strExecPath);
            }

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = false,
                Filter = Properties.Resources.openForFilePathFilenameFilter,
                InitialDirectory = initDirectory

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

                String msg = Properties.Resources.msgTxtOkSavedNotNew;
                if (_isNewFile)
                {
                    GuiMiscUtils.MsgInfo(Properties.Resources.msgTxtOkSavedNew,
                        Properties.Resources.msgTxtInfo);
                }

                gridAuthentLinks.IsEnabled = true;
                _isNewFile = false;
            }
            catch (Exception ex)
            {
                GuiMiscUtils.MsgError(Properties.Resources.msgTxtErrorCreateAuthentFile,
                    Properties.Resources.msgTxtError);
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
            OpenAuthentFile(authentFilepath);
        }

        private void OpenAuthentFile(string authentFilepath)
        {
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
                gridAuthentLinks.IsEnabled = true;
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

            gridAuthentLinks.IsEnabled = false;

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


        private void btnVerifyUser_Click(object sender, RoutedEventArgs e)
        {
            String tUsername = tbUsername.Text;
            if (string.IsNullOrWhiteSpace(tUsername)) return;

            if (MiscAppUtils.IsUserExist(tUsername))
            {
                GuiMiscUtils.MsgInfo("L'utilisateur existe sur cet ordinateur", "Information");
            }
            else
            {
                GuiMiscUtils.MsgError("L'utilisateur n'existe pas sur cet ordinateur", "Information");
            }
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

        private void hlinkOpenFolderCrt_Click(object sender, RoutedEventArgs e)
        {
            if (_isNewFile) return;
            
            FileUtils.ShowFileInWindowsExplorer(tbExecPath.Text.TrimPathExt());
        }
    }

}
