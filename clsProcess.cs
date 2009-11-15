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

        private int threadPriority; // thread priority for running threads

       

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

        // Holds and sets thread priority value for each process
         public int Thread_Priority
         {
             get
             {
                 return threadPriority;
             }
             set
             {
                 threadPriority = value;
             }
         }

        // auto-implemented starttime variable of type string.
        // Holds and sets starttime value for each process
         public string StartTime { get; set; }

        // auto-implemented cputime variable of type string.
        // Holds and sets cputime value for each process
         public string CpuTime { get; set; }

        // auto-implemented threadcount variable of type string.
        // Holds and sets threadcount value for each process
         public string ThreadCount { get; set; }

    }
}
