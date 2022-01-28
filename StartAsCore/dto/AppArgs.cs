using System.IO;

namespace StartAsCore.dto
{
    public class AppArgs
    {
        public string AuthentFilepath { get; set; }
        public string SecuredPinStart { get; set; }
        public bool WaitForApp { get; set; }
        public bool RunnedWithProfile { get; set; }
        public string TmpAuthentFilepath { get; set; }
    }
}
