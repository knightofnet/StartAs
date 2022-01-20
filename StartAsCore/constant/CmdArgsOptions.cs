using AryxDevLibrary.utils.cliParser;

namespace StartAsCore.constant
{
    public static class CmdArgsOptions
    {
        public static readonly Option OptAuthentFile = new Option()
        {
            ShortOpt = "f",
            LongOpt = "authent-file",
            Description = "Authentification file",
            HasArgs = true,
            Name = "OptAuthentFile",
            IsMandatory = true
        };

        public static readonly Option OptWait = new Option()
        {
            ShortOpt = "w",
            LongOpt = "wait",
            Description = "Wait app to end",
            HasArgs = false,
            Name = "OptWait",
            IsMandatory = false
        };

        public static readonly Option OptSecuredPinStart = new Option()
        {
            ShortOpt = "p",
            LongOpt = "secured-pin-start",
            Description = "Pin to start (secured one, used to relaunch app)",
            HasArgs = true,
            Name = "OptSecuredPinStart",
            IsMandatory = false
        };


        
    }
}
