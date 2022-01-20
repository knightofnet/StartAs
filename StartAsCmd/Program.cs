using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using AryxDevLibrary.extensions;
using AryxDevLibrary.utils;
using AryxDevLibrary.utils.cliParser;
using StartAsCore.business;
using StartAsCore.constant;
using StartAsCore.dto;
using StartAsCore.utils;

namespace StartAsCmd
{
    class Program
    {
        static void Main(string[] args)
        {
            AppArgs appArgs = null;
            try
            {
                appArgs = ParseAppArgs(args);
            }
            catch (CliParsingException cx)
            {
                Console.WriteLine(cx.Message);
                Console.WriteLine();
                return;
            }

            string currentUser = Environment.UserName;
            AuthentFile aFile = null;

            try
            {
                aFile = AuthentFileUtils.CryptAuthenDtoFromFile(appArgs.FilecertPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur lors de la lecture du fichier crt.");
                Console.WriteLine();
                return;
            }

            if (aFile == null) return;

            if (!VerifBeforeStart(aFile, appArgs.SecuredPinStart))
            {
                Console.WriteLine();
                return;
            }

            RunCert(currentUser, aFile, appArgs);
        }

        private static AppArgs ParseAppArgs(string[] args)
        {
            AppArgs retAppArgs;
            if (args.Length == 0)
            {
                throw new CliParsingException(
                    "Il est nécessaire de démarrer l'application avec au moins un argument, le fichier d'authentification");
            }

            if (args[0].Trim().StartsWith("-"))
            {
                CmdArgsParser cmdArgsParser = new CmdArgsParser();
                retAppArgs = cmdArgsParser.ParseDirect(args);
            }
            else
            {
                retAppArgs = new AppArgs
                {
                    FilecertPath = args[0],
                    SecuredPinStart = args.Length >= 2 ? args[1] : null
                };
            }

            return retAppArgs;
        }

        private static void RunCert(string currentUser, AuthentFile aFile, AppArgs appArgs)
        {
            string certPath = appArgs.FilecertPath;

            ProcessStartInfo psi;
            if (!currentUser.Equals(aFile.Username))
            {
                StringBuilder newProcessArgs = new StringBuilder($"-{CmdArgsOptions.OptAuthentFile.ShortOpt} \"{certPath}\"");
                if (aFile.IsAskForPinAtStart)
                {
                    newProcessArgs.Append(
                        $" -{CmdArgsOptions.OptSecuredPinStart.ShortOpt} {StringCipher.Encrypt(aFile.PinStart, aFile.GetSpecialHashCode())}");
                }

                if (appArgs.WaitForApp)
                {
                    newProcessArgs.Append(
                        $" -{CmdArgsOptions.OptWait.ShortOpt}");
                }

                psi = new ProcessStartInfo()
                {
                    FileName = Assembly.GetExecutingAssembly().Location,
                    UseShellExecute = false,
                    Arguments = newProcessArgs.ToString(),
                    UserName = aFile.Username,
                    Password = aFile.PasswordSecured.ToSecureString(),
                    WindowStyle = ProcessWindowStyle.Hidden
                    
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
                    WindowStyle = aFile.WindowStyleToLaunch,
                };
            }

            Process p = new Process();
            p.StartInfo = psi;
            p.Start();
            if (appArgs.WaitForApp)
            {
                p.WaitForExit();
            }
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

            if (aFile.IsHaveExpirationDate)
            {
                if (!aFile.ExpirationDate.HasValue)
                {
                    throw new Exception("La date d'expiration est manquante");
                }
                DateTime now = DateTime.Now;
                return (now.Date.Equals(aFile.AuthentFileDateCreated.Date) ||
                        now.Date.IsAfter(aFile.AuthentFileDateCreated.Date))
                       && now.IsBefore(aFile.ExpirationDate.Value);
            }


            return true;
        }
    }
}
