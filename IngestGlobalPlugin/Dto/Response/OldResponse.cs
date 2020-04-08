using IngestGlobalPlugin.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace IngestGlobalPlugin.Dto
{
    class OldResponse
    {
    }

    public enum TC_TYPE
    {
        emPresetTC = 0,
        emOriginalTC,
        emHouseTC
    }

    public enum TC_MODE
    {
        emForLine,
        emForOther
    }

    public enum OTID//ObjectTypeID
    {
        OTID_ALL = 0,//所有的zmj2008-9-3
        OTID_VTR = 1,//VTR
        OTID_CHANNEL,//通道
        OTID_OTHER,//其他
    }


    public class PostLockObject_param_in
    {
        public int ObjectID;
        public OTID ObjectTypeID;
        public string userName;
        public int TimeOut;
    }

    public class PostLockObject_param_out
    {
        public string errStr;
        public bool bRet;
    }

    public class GetGlobalState_OUT
    {
        public DbpGlobalState[] arrGlobalState;
        public string strErr;
        public bool bRet;
    }
    
    public class OldResponseMessage
    {
        public int nCode { get; set; }
        public string message { get; set; }
        public OldResponseMessage()
        {
            nCode = 1;          //1代表成功，0代表失败
            message = "OK";
        }
    }

}
