using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartAsCore.dto
{
    [Serializable]
    public class AuthentFile
    {

        public string Filepath { get; set; }

        public string Arguments { get; set; }

        public string WorkingDirectory { get; set; }

        public ProcessWindowStyle WindowStyle { get; set; }

        public string Username { get; set; }

        public string PasswordSecured { get; set; }

        public DateTime AuthentFileDateCreated { get; set; }



    }
}
