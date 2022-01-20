using System;
using System.Diagnostics;

namespace StartAsCore.dto
{
    [Serializable]
    public class AuthentFile
    {

        public string Filepath { get; set; }

        public string Arguments { get; set; }

        public string WorkingDirectory { get; set; }

        public ProcessWindowStyle WindowStyleToLaunch { get; set; }

        public string Username { get; set; }

        public string PasswordSecured { get; set; }

        public DateTime AuthentFileDateCreated { get; set; }

        public bool IsDoSha1VerifAtStart { get; set; }

        public string ChecksumSha1 { get; set; }

        public string ChecksumCrc32 { get; set; }

        public bool IsAskForPinAtStart { get; set; }

        public string PinStart { get; set; }
        public long FilepathLength { get; set; }
        public bool IsHaveExpirationDate { get; set; }
        public DateTime? ExpirationDate { get; set; }

        public string GetSpecialHashCode()
        {
            return  $"{Filepath}#{Username}#{PasswordSecured}#{PinStart}".GetHashCode().ToString();
        }
    }
}
