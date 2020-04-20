using System;
using System.Collections.Generic;
using System.Text;

namespace IngestDBCore.Dto
{
    public class CmParam
    {
        public string paramname { get; set; }
        public string paramvalue { get; set; }
        public string paramvaluedef { get; set; }
        public string paramdescription { get; set; }
    }

    public class DefaultParameter : CmParam
    {
        public string system { get; set; }
        public string tool { get; set; }
        //public string key { get; set; }
        //public bool autocreate { get; set; }
        //public string defaults { get; set; }
        //public string note { get; set; }
        //用户参数


    }

    public class CMUserInfo
    {
        public string createtime;
        public bool disabled;
        public string email;
        public string id;
        public string loginname;
        public string nickname;
    }

    public class ExtParam
    {
        public string accessPWD { get; set; }
        public string accessUser { get; set; }
        public string path { get; set; }
        public string pathType { get; set; }
        public List<string> storageMarks { get; set; }


        public long? storageSize { get; set; }
        public string storageType { get; set; }
        public long? usedSize { get; set; }
    }
}
