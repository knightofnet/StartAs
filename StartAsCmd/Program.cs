using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using AryxDevLibrary.utils;
using StartAsCore.dto;
using StartAsCore.utils;

namespace StartAsCmd
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Le premier argument doit être un fichier crt");
                Console.WriteLine();
                return;
            }

            string currentUser = Environment.UserName;
            AuthentFile aFile = null;
            ProcessStartInfo psi;
            try
            {
                aFile = AuthentFileUtils.CryptAuthenDtoFromFile(args[0]);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur lors de la lecture du fichier crt.");
                Console.WriteLine();
                return;
            }

            if (aFile == null) return;

            if (!currentUser.Equals(aFile.Username))
            {
                psi = new ProcessStartInfo()
                {
                    FileName = Assembly.GetExecutingAssembly().Location,
                    UseShellExecute = false,
                    Arguments = $"\"{args[0]}\"",
                    UserName = aFile.Username,
                    PasswordInClearText = aFile.PasswordSecured
                };
            }
            else
            {
                psi = new ProcessStartInfo()
                {
                    FileName = aFile.Filepath,
                    UseShellExecute = true,
                    Verb = "runas",
                    WorkingDirectory = aFile.WorkingDirectory,
                    Arguments = aFile.Arguments,
                    WindowStyle = aFile.WindowStyle

                };
            }

            Process p = new Process();
            p.StartInfo = psi;

            p.Start();

        }
    }
}
