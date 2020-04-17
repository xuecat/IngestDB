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

    /// <summary>
    /// 很多重复的类字段一样，进行整合：PostLockObject_param_out,SetUserSetting_OUT
    /// </summary>
    public class OldPostParam_Out
    {
        public string errStr;
        public bool bRet;
    }

    public class SetUserSetting_IN
    {
        public string strUserCode;
        public string strSettingtype;
        public string strSettingText;
    }

    public class GetGlobalState_OUT
    {
        public List<GlobalState> arrGlobalState;
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

    public class OldResponseMessage<T> : OldResponseMessage
    {
        public T extention;
    }

    public class GetDefaultSTC_param
    {
        public TC_TYPE tcType;
        public int nTC;
        public string errStr;
        public bool bRet;
    }

    public class GlobalState
    {
        public string strLabel = string.Empty;
        public string dtLastTime = DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss");
    }

    public class ObjectContent
    {
        public int ObjectID = -1;//对象ID
        public OTID ObjectType = OTID.OTID_ALL;//对象类型
        public string UserName = string.Empty;//用户名
        public DateTime BeginTime = DateTime.MinValue;//开始时间
        public int TimeOut = 0;//超时时间
    }

    public class GetParamTemplateByID_out
    {
        public string strCaptureParam;
        public string errStr;
        public bool bRet;
    }

    public class OldUserTemplate
    {
        public int nTemplateID = -1;                    //模板ID
        public string strUserCode = string.Empty;       //用户名
        public string strTemplateName = string.Empty;   //模板名
        public string strTemplateContent = string.Empty;//模板内容   
    }

    public class OldCMUserInfo
    {
        public string createtime;
        public bool disabled;
        public string email;
        public string id;
        public string loginname;
        public string nickname;
    }

    public class ResponseMessageN
    {
        public string Code { get; set; }
        public string msg { get; set; }
        public ResponseMessageN()
        {
            Code = "0";
        }
    }
    public class ResponseMessageN<TEx> : ResponseMessageN
    {
        public TEx ext { get; set; }
    }


    public class etparam
    {
        public string paramname { get; set; }
        public string paramvalue { get; set; }
        public string paramvaluedef { get; set; }
        public string paramdescription { get; set; }
    }
    
    public class parameter : etparam
    {
        public string system { get; set; }
        public string tool { get; set; }
    }

    public class userparameter : parameter
    {
        public string loginname { get; set; }
    }

    public class OldCapParamTemplate
    {
        public int nID = 0;     //采集参数模板ID
        public string strTemplateName = "";//采集参数模板
        public string strParamTemplate = string.Empty;//参数模板内容采集
    }

    public class UserParmMap
    {
        public int nCapatureParamID = 0;
        public string szClassCode = "";
    }

}
