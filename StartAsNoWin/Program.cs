﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StartAsCore.constant;

namespace StartAsNoWin
{
    class Program
    {
        static void Main(string[] args)
        {
            String loc = System.Reflection.Assembly.GetExecutingAssembly().Location;


            FileInfo mainExe = new FileInfo(Path.Combine(Path.GetDirectoryName(loc), "StartAsCmd.exe") );
            if (!mainExe.Exists) Environment.Exit(1);

            ProcessStartInfo pStartInfo = new ProcessStartInfo
            {
                FileName = mainExe.FullName,
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = false,
                Arguments = $"-{CmdArgsOptions.OptRunNoWinIfPossible.ShortOpt} " + Environment.CommandLine
                    .Replace("\"" + System.Reflection.Assembly.GetExecutingAssembly().Location + "\"", "").Trim()
            };

            Process process = new Process { StartInfo = pStartInfo };

            process.Start();
            process.WaitForExit();

            int exitCode = 1;
            try
            {
                exitCode = process.ExitCode;
            }
            catch (Exception)
            {
                exitCode = -1;
            }

            Environment.Exit(exitCode);
        }
    }
}