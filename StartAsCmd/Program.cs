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

            String certPath = null;
            String pinStart = null;
            if (args.Length >= 1)
            {
                certPath = args[0];
            }
            if (args.Length >= 2)
            {
                pinStart = args[1];
            }


            string currentUser = Environment.UserName;
            AuthentFile aFile = null;

            try
            {
                aFile = AuthentFileUtils.CryptAuthenDtoFromFile(certPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur lors de la lecture du fichier crt.");
                Console.WriteLine();
                return;
            }

            if (aFile == null) return;

            if (!VerifBeforeStart(aFile, pinStart))
            {
                Console.WriteLine();
                return;
            }

            RunCert(certPath, currentUser, aFile);
        }



        private static void RunCert(string certPath, string currentUser, AuthentFile aFile)
        {
            ProcessStartInfo psi;
            if (!currentUser.Equals(aFile.Username))
            {
                string pin = aFile.IsAskForPinAtStart ? $" {StringCipher.Encrypt(aFile.PinStart, aFile.GetSpecialHashCode())}" : string.Empty;
                psi = new ProcessStartInfo()
                {
                    FileName = Assembly.GetExecutingAssembly().Location,
                    UseShellExecute = false,
                    Arguments = $"\"{certPath}\"{pin}",
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
                    WindowStyle = aFile.WindowStyleToLaunch
                };
            }

            Process p = new Process();
            p.StartInfo = psi;

            p.Start();
        }

        private static bool VerifBeforeStart(AuthentFile aFile, string pinStartEncrypted)
        {

            if (aFile.IsDoSha1VerifAtStart)
            {
                String[] sV = FileIntegrityUtils.CalculateFileIntegrity(new FileInfo(aFile.Filepath));
                if (!aFile.ChecksumSha1.Equals(sV[0]) || !aFile.ChecksumCrc32.Equals(sV[1]))
                {
                    Console.WriteLine("Integrité du fichier non vérifiée");
                    return false;
                }

            }

            
            if (aFile.IsAskForPinAtStart)
            {
                string pinInput = pinStartEncrypted == null ? null : StringCipher.Decrypt(pinStartEncrypted, aFile.GetSpecialHashCode());
                if (pinInput == null)
                {
                    Console.WriteLine("Entrez le PIN pour démarrer le fichier :");
                    pinInput = Console.ReadLine();
                }

                if (!aFile.PinStart.Equals(pinInput))
                {
                    Console.WriteLine("Code PIN erronné");
                    return false;
                }
            }



            return true;
        }
    }
}
