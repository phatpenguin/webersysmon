using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaskManagerProj
{
    class clsProcess
    {
        private string strProcessName;

        private int intNbrProcesses;

        private int intProcessID;

        private string strMemoryUsage;

        public string ProcessName
        {
            get { return strProcessName; }
            set { strProcessName = value; }
        }

        public int NbrProcesses
        {
            get { return intNbrProcesses; }
            set { intNbrProcesses = value; }
        }

        public int ProcessID
        {
            get { return intProcessID; }
            set { intProcessID = value; }
        }

        public string MemoryUsage
        {
            get { return strMemoryUsage; }
            set {strMemoryUsage = value; }
        }

         public override string ToString()
        {
            return strProcessName;
        }

    }
}
