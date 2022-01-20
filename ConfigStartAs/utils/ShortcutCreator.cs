using System;
using System.IO;
using System.Windows;
using Shell32;

namespace ConfigStartAs.utils
{
    internal class ShortcutCreator
    {


        public string ShortcutName { get; private set; }

        public ShortcutCreator(string targetOfShortcut, string nameOfLnkFile, string whereToSaveShortcut)
        {

            ShortcutTarget = new FileInfo(targetOfShortcut);

            DirectoryToCreateShortcut = new DirectoryInfo(whereToSaveShortcut);
            ShortcutName = nameOfLnkFile;

        }

        public bool ErrorDuringCreation { get; private set; }
        public FileInfo ShortcutTarget { get; private set; }
       
        public DirectoryInfo DirectoryToCreateShortcut { get; private set; }

        public string Args { get; set; }
        public string IconFile { get;  set; }


        public bool CreateShortcut()
        {
            try
            {
                if (!CheckAlreadyExistsShortcut())
                {
                    return false;
                }


                String shortcutFullPath = Path.Combine(DirectoryToCreateShortcut.FullName, ShortcutName + ".lnk");

                File.WriteAllBytes(shortcutFullPath, new byte[0]);

                Shell shl = new Shell();

                Folder dir = shl.NameSpace(DirectoryToCreateShortcut.FullName);
                FolderItem itm = dir.Items().Item(ShortcutName + ".lnk");
                ShellLinkObject lnk = (ShellLinkObject)itm.GetLink;
                // Set the .lnk file properties
                lnk.Path = ShortcutTarget.FullName;
                lnk.Description = "";
                lnk.Arguments = Args;
                lnk.WorkingDirectory = Path.GetDirectoryName(ShortcutTarget.FullName);
                lnk.SetIconLocation(IconFile, 0);
                lnk.Save(shortcutFullPath);



                return true;
            }
            catch (Exception ex)
            {
                ErrorDuringCreation = true;
                return false;
            }
        }







        private bool CheckAlreadyExistsShortcut()
        {
            String path = Path.Combine(DirectoryToCreateShortcut.FullName, ShortcutName + ".lnk");
            if (File.Exists(path))
            {
                MessageBoxResult res =
                    MessageBox.Show(
                        String.Format(
                            "Un raccourci nommé '{0}' existe déjà dans le dossier '{1}'. Voulez-vous le remplacer ?",
                            ShortcutName, DirectoryToCreateShortcut.FullName), "Question", MessageBoxButton.YesNo,
                        MessageBoxImage.Question);
                if (res == MessageBoxResult.Yes)
                {
                    return true;
                }
            }
            else
            {
                return true;
            }

            return false;
        }






    }
}
