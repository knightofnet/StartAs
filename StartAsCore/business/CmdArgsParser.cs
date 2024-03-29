﻿using System.Collections.Generic;
using AryxDevLibrary.utils.cliParser;
using StartAsCore.constant;
using StartAsCore.dto;

namespace StartAsCore.business
{
    public class CmdArgsParser : CliParser<AppArgs>
    {
        public CmdArgsParser()
        {
            AddOption(CmdArgsOptions.OptAuthentFile);
            AddOption(CmdArgsOptions.OptWait);
            AddOption(CmdArgsOptions.OptSecuredPinStart);
            AddOption(CmdArgsOptions.OptRunNoWinIfPossible);
            AddOption(CmdArgsOptions.OptRunnedWithProfile);
        }
        public override AppArgs ParseDirect(string[] args)
        {
            return Parse(args, ParseTrt);
        }

        private AppArgs ParseTrt(Dictionary<string, Option> arg)
        {
            AppArgs retAppArgs = new AppArgs();

            retAppArgs.AuthentFilepath = GetSingleOptionValue(CmdArgsOptions.OptAuthentFile, arg);
            retAppArgs.WaitForApp = HasOption(CmdArgsOptions.OptWait, arg);
            retAppArgs.RunnedWithProfile = HasOption(CmdArgsOptions.OptRunnedWithProfile, arg);
            retAppArgs.RunNoWinIfPossible = HasOption(CmdArgsOptions.OptRunNoWinIfPossible, arg);

            if (HasOption(CmdArgsOptions.OptSecuredPinStart, arg))
            {
                retAppArgs.SecuredPinStart = GetSingleOptionValue(CmdArgsOptions.OptSecuredPinStart, arg);
            }

            return retAppArgs;
        }
    }
}
