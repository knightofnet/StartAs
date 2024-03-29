﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Reflection;
using System.ServiceProcess;
using System.Text;

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
        internal const string ArgChar = "-";

        private static Logger _log;

        private static readonly string CommonAppDataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "StartAs");
        private static readonly string LocalAppDataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "StartAs");


        static void Main(string[] args)
        {
            try
            {
                InitLog(LocalAppDataDir);
            }
            catch (UnauthorizedAccessException e)
            {
                InitLog(CommonAppDataDir, StringUtils.RandomString(16)+"-StartAs.log", true);
            }

            try
            {
                _log.Debug("Step 0 - Start");

                if (!Directory.Exists(CommonAppDataDir))
                {
                    Directory.CreateDirectory(CommonAppDataDir);
                }


                AppArgs appArgs = ParseAppArgs(args);
                VerifyServices();

                AuthentFile aFile = UncryptAuthentFile(appArgs);

                if (aFile == null) throw new Exception("Empty aFile");

                _log.Debug("Step 3 - Verify conditions to run");
                VerifBeforeStart(aFile, appArgs.SecuredPinStart, appArgs);

                _log.Debug("Step 4 - Run sub-process");
                Environment.Exit(RunCert(aFile, appArgs));

            }
            catch (Exception ex)
            {
                if (_log != null)
                {
                    _log.Error(ex.Message);
#if DEBUG
                    _log.Error(ex.StackTrace);
#endif
                }
                else
                {
                    Console.WriteLine(ex.Message);
#if DEBUG
                    Console.WriteLine(ex.StackTrace);
#endif
                    File.WriteAllText(Path.Combine(CommonAppDataDir, StringUtils.RandomString(16) + "-error.log"), $"{ex.Message}\n{ex.StackTrace}");

                }

                Environment.Exit(1002);

            }
        }


        private static void InitLog(string dir, string logFilename = "log.log", bool isExit=false)
        {
            try
            {
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
#if DEBUG
                _log = new Logger(Path.Combine(dir, logFilename), Logger.LogLvl.DEBUG,
                    Logger.LogLvl.DEBUG, "1Mo");
#else
                _log = new Logger(Path.Combine(dir, logFilename), Logger.LogLvl.ERROR, Logger.LogLvl.ERROR,
                    "1Mo");
#endif
            }
            catch (Exception ex)
            {
                if (isExit)
                {
                    Console.WriteLine(ex.Message);
#if DEBUG
                    Console.WriteLine(ex.StackTrace);
#endif
                    File.WriteAllText(Path.Combine(CommonAppDataDir, StringUtils.RandomString(16) + "-StartAs-Error.log"),
                        $"{ex.GetType()} - {ex.Message}\n{ex.StackTrace}");

                    Environment.Exit(1001);
                }
                else
                {
                    throw ex;
                }


            }
        }

        private static AuthentFile UncryptAuthentFile(AppArgs appArgs)
        {
            AuthentFile aFile;
            try
            {
                _log.Debug("Step 2 - Uncrypt authentification file");
                aFile = AuthentFileUtils.CryptAuthenDtoFromFile(appArgs.AuthentFilepath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(Properties.Resources.msgExErrorReadingAuthentFile);
                throw new Exception(Properties.Resources.msgExErrorReadingAuthentFile, ex);
            }

            return aFile;
        }


        private static AppArgs ParseAppArgs(string[] args)
        {
            try
            {
                AppArgs retAppArgs;
                if (args.Length == 0)
                {
                    throw new CliParsingException(Properties.Resources.msgCliParsingExceptionZeroArg);
                }

                if (args[0].Trim().StartsWith(ArgChar))
                {
                    CmdArgsParser cmdArgsParser = new CmdArgsParser();
                    retAppArgs = cmdArgsParser.ParseDirect(args);
                }
                else
                {
                    retAppArgs = new AppArgs
                    {
                        AuthentFilepath = args[0],
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

        private static int RunCert(AuthentFile aFile, AppArgs appArgs)
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
                _log.Debug("Prepare run profil");
                psi = PrepareRunWithProfile(aFile, appArgs);
            }
            else
            {
                _log.Debug("Prepare run as");

                psi = new ProcessStartInfo()
                {
                    FileName = aFile.Filepath,
                    UseShellExecute = true,
                    Verb = "runas",
                    WorkingDirectory = aFile.WorkingDirectory,
                    Arguments = aFile.Arguments,
                    WindowStyle = aFile.WindowStyleToLaunch
                };
                _log.Debug($"FileName:{psi.FileName}, " +
                           $"UseShellExecute={psi.UseShellExecute}, " +
                           $"Verb={psi.Verb}, " +
                           $"Arguments={psi.Arguments}, " +
                           $"UseShellExecute={psi.UseShellExecute}, " +
                           $"WindowStyle={psi.WindowStyle}, " +
                           $"WorkingDirectory={psi.WorkingDirectory}");
                mode = 1;
            }

            Process p = new Process();
            p.StartInfo = psi;
            p.Start();
            if (appArgs.WaitForApp || mode == 0)
            {
                p.WaitForExit();
                _log.Debug($"Process ended: {p.ExitCode}");
            }

            if (mode == 0)
            {
                FileInfo fAuthentTemp = new FileInfo(appArgs.TmpAuthentFilepath);
                fAuthentTemp.Delete();

                return p.ExitCode;
            }

            return appArgs.WaitForApp ? p.ExitCode : 0;
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
            FileInfo originalAuthenFile = new FileInfo(appArgs.AuthentFilepath);
            string tmpAuthentFilepath = Path.Combine(CommonAppDataDir, StringUtils.RandomString(16) + originalAuthenFile.Name);

            aFile.IsAskForPinAtStart = true;
            aFile.PinStart = StringUtils.RandomString(16);
            aFile.IsHaveExpirationDate = true;
            aFile.ExpirationDate = DateTime.Now.AddMinutes(1);
            aFile.IsTempAuthentfile = true;

            AuthentFileUtils.CryptAuthenDtoToFile(aFile, tmpAuthentFilepath);
            appArgs.TmpAuthentFilepath = tmpAuthentFilepath;

            // Preparing argument's string, with temporary authentification filepath
            StringBuilder newProcessArgs = new StringBuilder($"{ArgChar}{CmdArgsOptions.OptAuthentFile.ShortOpt} \"{tmpAuthentFilepath}\"");
            // Adding the new, temporary, Pin 
            newProcessArgs.Append(
                $" {ArgChar}{CmdArgsOptions.OptSecuredPinStart.ShortOpt} {StringCipher.Encrypt(aFile.PinStart, aFile.GetSpecialHashCode())}");

            // if user ask for wait option, we transmit it
            if (appArgs.WaitForApp)
            {
                newProcessArgs.Append($" {ArgChar}{CmdArgsOptions.OptWait.ShortOpt}");
            }

            // Finally, we add an argument to tell the next 'StartAsCmd' that it is started with
            // the target profile.
            newProcessArgs.Append($" {ArgChar}{CmdArgsOptions.OptRunnedWithProfile.ShortOpt}");

            _log.Debug(newProcessArgs.ToString());

            string startAsFileName = Assembly.GetExecutingAssembly().Location;
            string startAsCmdDir = Path.GetDirectoryName(startAsFileName);
            if (appArgs.RunNoWinIfPossible && startAsCmdDir != null)
            {
                string locNoWin = Path.Combine(startAsCmdDir, "StartAsNoWin.exe");
                if (File.Exists(locNoWin))
                {
                    startAsFileName = locNoWin;
                }
            }

            ProcessStartInfo psi = new ProcessStartInfo()
            {
                FileName = startAsFileName,
                UseShellExecute = false,
                Arguments = newProcessArgs.ToString().TrimStart(),
                UserName = aFile.Username,
                Password = aFile.PasswordSecured.ToSecureString(),
                WindowStyle = ProcessWindowStyle.Hidden,
                WorkingDirectory = startAsCmdDir ?? Path.GetTempPath()
            };
            _log.Debug($"FileName:{psi.FileName}, " +
                       $"UseShellExecute={psi.UseShellExecute}, " +
                       $"Verb={psi.Verb}, " +
                       $"Arguments={psi.Arguments}, " +
                       $"UseShellExecute={psi.UseShellExecute}, " +
                       $"WindowStyle={psi.WindowStyle}");

            return psi;
        }

        private static void VerifBeforeStart(AuthentFile aFile, string pinStartEncrypted, AppArgs appArgs)
        {

            if (aFile.IsDoSha1VerifAtStart)
            {
                String[] sV = FileIntegrityUtils.CalculateFileIntegrity(new FileInfo(aFile.Filepath));
                if (!aFile.ChecksumSha1.Equals(sV[0]) || !aFile.ChecksumCrc32.Equals(sV[1]))
                {
                    throw new Exception(Properties.Resources.msgExVerifAfIntegrityKo);

                }

            }

            if (aFile.IsHaveExpirationDate)
            {
                if (!aFile.ExpirationDate.HasValue)
                {
                    throw new Exception(Properties.Resources.msgExVerifAfDateExpMissing);
                }
                DateTime now = DateTime.Now;
                if (now.Date.IsBefore(aFile.AuthentFileDateCreated.Date) || now.Date.IsAfter(aFile.ExpirationDate.Value))
                {
                    throw new Exception(Properties.Resources.msgExVerifAfDateExpInvalid);

                }
            }

            if (aFile.IsAskForPinAtStart)
            {
                string pinInput = pinStartEncrypted == null ? null : StringCipher.Decrypt(pinStartEncrypted, aFile.GetSpecialHashCode());
                if (pinInput == null)
                {
                    if (appArgs.RunNoWinIfPossible)
                    {
                        SystemSounds.Beep.Play();
                        Environment.Exit(50);
                    }
                    Console.WriteLine(Properties.Resources.msgEnterPinToStart);
                    pinInput = Console.ReadLine();
                }

                if (!aFile.PinStart.Equals(pinInput))
                {
                    throw new Exception(Properties.Resources.msgExVerifAfWrongPinCode);

                }
            }

        }

        private static void VerifyServices()
        {
            _log.Debug("VerifyServices: Wait for secLogon service requested");
            ServiceController secLogonSvc;
            try
            {
                secLogonSvc = new ServiceController("seclogon");
            }
            catch (Exception ex)
            {
                secLogonSvc = null;
                _log.Debug("VerifyServices: Create ServiceController for secLogon service failed - proceeding...");

                throw ex;
            }

            
                //Sanity check for existance of TV service
                ServiceControllerStatus status = ServiceControllerStatus.Stopped;
                try
                {
                    status = secLogonSvc.Status;
                }
                catch (Exception ex)
                {
                    _log.Debug("VerifyServices: Failed to retrieve secLogon service status");
                    secLogonSvc.Close();
                    secLogonSvc = null;

                    throw ex;
                }


            _log.Debug("VerifyServices: secLogon service found. Checking status...");

            if (secLogonSvc.Status == ServiceControllerStatus.StartPending || secLogonSvc.Status == ServiceControllerStatus.Stopped)
            {
                switch (secLogonSvc.Status)
                {
                    case ServiceControllerStatus.StartPending:
                        _log.Info("VerifyServices: secLogon service start is pending. Waiting...");
                        break;
                    case ServiceControllerStatus.Stopped:
                        _log.Info("VerifyServices: secLogon service is stopped, so we try start it...");
                        try
                        {
                            secLogonSvc.Start();
                        }
                        catch (Exception ex)
                        {
                            _log.Info("secLogon seems to be already starting up.");
                            //throw ex;

                        }

                        break;
                }

                try
                {
                    secLogonSvc.WaitForStatus(ServiceControllerStatus.Running, new TimeSpan(0, 0, 45));
                }
                catch (Exception ex)
                {
                    throw ex;
                }


                if (secLogonSvc.Status == ServiceControllerStatus.Running)
                {
                    _log.Debug("VerifyServices: The secLogon service has started successfully.");
                }
                else
                {
                    _log.Error("VerifyServices: Startup of the secLogon service failed - current status: {0}", secLogonSvc.Status.ToString());
                }
            }
            _log.Debug("VerifyServices: secLogon service is in status {0} - proceeding...", secLogonSvc.Status.ToString());
            secLogonSvc.Close();

        }
    }
}
