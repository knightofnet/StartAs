using System;
using System.DirectoryServices;

using System.Linq;
using System.Security.Principal;

namespace StartAsCore.utils
{
    public static class MiscAppUtils
    {

        public static SecurityIdentifier GetComputerSid()
        {
            return new SecurityIdentifier((byte[])new DirectoryEntry($"WinNT://{Environment.MachineName},Computer").Children.Cast<DirectoryEntry>().First().InvokeGet("objectSID"), 0).AccountDomainSid;
        }


        public static bool IsUserExist(string username)
        {
            string adsPath = $@"WinNT://{Environment.MachineName}";
            using (DirectoryEntry de = new DirectoryEntry(adsPath))
            {
                try
                {
                    return de.Children != null && de.Children.Find(username) != null;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
        }
    }
}
