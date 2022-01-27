using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using AryxDevLibrary.extensions;
using AryxDevLibrary.utils;
using AryxDevLibrary.utils.cliParser;
using AryxDevLibrary.utils.logger;
using StartAsCore.business;
using StartAsCore.constant;
using StartAsCore.dto;
using StartAsCore.utils;

namespace StartAsCmd
{
    class Program
    {
#if DEBUG
        private static readonly Logger Log = new Logger("log.log", Logger.LogLvl.DEBUG, Logger.LogLvl.DEBUG, "1Mo");
#else
        private static readonly Logger Log = new Logger("log.log", Logger.LogLvl.ERROR, Logger.LogLvl.ERROR, "1Mo");
#endif

        private static readonly string AppDataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "StartAs");


        static void Main(string[] args)
        {

            try
            {
                if (!Directory.Exists(AppDataDir))
                {
                    Directory.CreateDirectory(AppDataDir);
                }

                AppArgs appArgs = null;
                try
                {
                    Log.Debug("ReadsArgs");
                    appArgs = ParseAppArgs(args);
                }
                catch (CliParsingException cx)
                {
                    Console.WriteLine(cx.Message);
                    Console.WriteLine();
                    throw cx;

                }

                string currentUser = Environment.UserName;
                AuthentFile aFile = null;

                try
                {
                    Log.Debug("Uncrypt");
                    aFile = AuthentFileUtils.CryptAuthenDtoFromFile(appArgs.FilecertPath);

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erreur lors de la lecture du fichier crt.");
                    Console.WriteLine();
                    throw ex;

                }

                Log.Debug($"{aFile}");

                if (aFile == null) throw new Exception("Empty aFile");

                if (!VerifBeforeStart(aFile, appArgs.SecuredPinStart))
                {
                    Console.WriteLine();
                    Log.Error("Error when verifying");
                    throw new Exception("Error when verifying");

                }

                Log.Debug("RunCert");
                RunCert(currentUser, aFile, appArgs);

            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
#if DEBUG
                Log.Error(ex.StackTrace);
#endif
                Environment.Exit(1);

            }
        }

        private static AppArgs ParseAppArgs(string[] args)
        {
            try {
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
            catch (CliParsingException cx)
            {
                Console.WriteLine(cx.Message);
                Console.WriteLine();
                throw cx;

            }
        }

        private static void RunCert(string currentUser, AuthentFile aFile, AppArgs appArgs)
        {
            /*
             Here, we will prepare the launch of a process. Two steps: either StartAsCmd must be
             started with the target profile (mode = 0), or the target application must be started with 
            elevated privileges (runas, mode = 1). 
            */

            int mode = 0;

            ProcessStartInfo psi;
            if (!appArgs.RunnedWithProfile)
            {
                Log.Debug("Prepare run profil");
                psi = PrepareRunWithProfile(aFile, appArgs);
            }
            else
            {
                Log.Debug("Prepare run as");

                psi = new ProcessStartInfo()
                {
                    FileName = aFile.Filepath,
                    UseShellExecute = true,
                    Verb = "runas",
                    WorkingDirectory = aFile.WorkingDirectory,
                    Arguments = aFile.Arguments,
                    WindowStyle = aFile.WindowStyleToLaunch,
                };

                mode = 1;
            }

            Process p = new Process();
            p.StartInfo = psi;
            p.Start();
            if (appArgs.WaitForApp || mode == 0)
            {
                p.WaitForExit();
            }

            if (mode == 0)
            {
                FileInfo fCert = new FileInfo(appArgs.TmpFileCert);
                fCert.Delete();
            }
        }

        /// <summary>
        /// Prepare the information to re-run StartAsCmd with target profile.
        /// </summary>
        /// <param name="aFile">Authentification's informations</param>
        /// <param name="appArgs">Application options</param>
        /// <returns></returns>
        private static ProcessStartInfo PrepareRunWithProfile(AuthentFile aFile, AppArgs appArgs)
        {
            /*
             * We will prepare a string of arguments and launch options (ProcessStartInfo) to
             * restart StartAsCmd in the right profile.
             *
             * The authentication file used up to this point may be located in a folder that the
             * next profile could not access: to avoid this problem, we will create a temporary
             * authentication file (with a pin and an expiration date) in the "ProgramData/StartAs"
             * folder. This folder was shared by all users, the next user will be able to access
             * the data of the authentication file.
             *
             * If we are in this method, it means that the verification of the authentication file
             * has been done; we can therefore override the parameters of the authentication file
             * for its temporary alternative.
             *
             */

            // Creating temporary authentification file
            FileInfo certPath = new FileInfo(appArgs.FilecertPath);
            string tmpCertFile = Path.Combine(AppDataDir, StringUtils.RandomString(16) + certPath.Name);

            aFile.IsAskForPinAtStart = true;
            aFile.PinStart = StringUtils.RandomString(16);
            aFile.IsHaveExpirationDate = true;
            aFile.ExpirationDate = DateTime.Now.AddMinutes(1);
            aFile.IsTempCertfile = true;

            AuthentFileUtils.CryptAuthenDtoToFile(aFile, tmpCertFile);
            appArgs.TmpFileCert = tmpCertFile;

            // Preparing argument's string, with temporary authentification filepath
            StringBuilder newProcessArgs = new StringBuilder($"-{CmdArgsOptions.OptAuthentFile.ShortOpt} \"{tmpCertFile}\"");
            // Adding the new, temporary, Pin 
            newProcessArgs.Append(
                $" -{CmdArgsOptions.OptSecuredPinStart.ShortOpt} {StringCipher.Encrypt(aFile.PinStart, aFile.GetSpecialHashCode())}");

            // if user ask for wait option, we transmit it
            if (appArgs.WaitForApp)
            {
                newProcessArgs.Append($" -{CmdArgsOptions.OptWait.ShortOpt}");
            }

            // Finally, we add an argument to tell the next 'StartAsCmd' that it is started with
            // the target profile.
            newProcessArgs.Append($" -{CmdArgsOptions.OptRunnedWithProfile.ShortOpt}");

            Log.Debug(newProcessArgs.ToString());

            ProcessStartInfo psi = new ProcessStartInfo()
            {
                FileName = Assembly.GetExecutingAssembly().Location,
                UseShellExecute = false,
                Arguments = newProcessArgs.ToString().TrimStart(),
                UserName = aFile.Username,
                Password = aFile.PasswordSecured.ToSecureString(),
                WindowStyle = ProcessWindowStyle.Hidden,
                WorkingDirectory = Directory.GetCurrentDirectory()
            };
            return psi;
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
