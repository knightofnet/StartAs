using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AryxDevLibrary.utils;
using AryxDevLibrary.utils.logger;
using StartAsCore.constant;

namespace StartAsNoWin
{
    class Program
    {
        private static readonly string CommonAppDataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "StartAs");

        private static readonly string LocalAppDataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "StartAs");


        private static Logger _log;

        static void Main(string[] args)
        {
            try
            {
                InitLog(LocalAppDataDir, "StartAsNoWin.log");
            }
            catch (UnauthorizedAccessException e)
            {
                InitLog(CommonAppDataDir, StringUtils.RandomString(16) + "-StartAsNoWin.log", true);
            }
            _log.Debug("Start");
            String loc = System.Reflection.Assembly.GetExecutingAssembly().Location;
            _log.Debug($"Exeloc: {loc}");

            FileInfo mainExe = new FileInfo(Path.Combine(Path.GetDirectoryName(loc), "StartAsCmd.exe") );
            if (!mainExe.Exists) Environment.Exit(1002);

            int exitCode = 1000;
            try
            {
                exitCode = 500;
                _log.Error("Prepare args");

                ProcessStartInfo pStartInfo = new ProcessStartInfo
                {
                    FileName = mainExe.FullName,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = false,
                    Arguments = $"-{CmdArgsOptions.OptRunNoWinIfPossible.ShortOpt} " + Environment.CommandLine
                        .Replace("\"" + Assembly.GetExecutingAssembly().Location + "\"", "").Trim()
                };

                exitCode = 501;
                _log.Error("Args prepared");

                Process process = new Process { StartInfo = pStartInfo };
                process.Start();

                exitCode = 502;
                _log.Error("Process started");
                process.WaitForExit();

               
                try
                {
                    exitCode = process.ExitCode;
                    _log.Error($"ExitCode: {exitCode}");
                }
                catch (Exception)
                {
                    _log.Error("Cant get exitCode");
                    exitCode = 1001;
                }

            }

            catch (Exception e)
            {
                _log.Error(e.Message);
                _log.Error(e.StackTrace);
            }

            Environment.Exit(exitCode);
        }

        private static void InitLog(string dir, string logFilename = "StartAsNoWin.log", bool isExit = false)
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
    }
}
