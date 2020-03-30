using System;
using System.Collections.Generic;
using System.Text;

namespace IngestDBCore
{
    public class GlobalStateName
    {
        static public string ADDTASK { get { return "TASK_ADD"; } }
        static public string MODTASK { get { return "TASK_MOD"; } }
        static public string DELTASK { get { return "TASK_DEL"; } }
        static public string MLCRT { get { return "ML_CRT"; } }
        static public string MLCPT { get { return "ML_CPT"; } }
        static public string BACKUP { get { return "BACKUP"; } }
    }

    public enum FunctionType
    {
        SetGlobalState
    }

    public class GlobalInternals
    {
        public FunctionType funtype { get; set; }

        public string State { get; set; }
    }
}
