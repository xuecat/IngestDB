using System;
using System.Collections.Generic;
using System.Text;

namespace IngestGlobalPlugin.Dto
{
    class GlobalResponse
    {

    }

    public class GlobalTcResponse
    {
        public TC_TYPE TcType { get; set; }

        public int TC { get; set; }
    }

    public class DtoUserTemplate
    {
        public int TemplateID = -1;                    //模板ID
        public string UserCode = string.Empty;       //用户名
        public string TemplateName = string.Empty;   //模板名
        public string TemplateContent = string.Empty;//模板内容   
    }

    public class DtoCMUserInfo
    {
        public string CreateTime;
        public bool Disabled;
        public string Email;
        public string Id;
        public string LoginName;
        public string NickName;
    }

    public class EUserTemplate
    {
        public string TemplateName { get; set; }
        public string TemplateContent { get; set; }
    }

    public class CapParamTemplate
    {
        public int ID { get; set; }
        public string TemplateName { get; set; }//采集参数模板
        public string ParamTemplate { get; set; }//参数模板内容采集
    }

    public class DtoGlobalState
    {
        public string Label = string.Empty;
        public string LastTime = DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss");
    }

    public class DtoSetUserSetting_IN
    {
        public string UserCode;
        public string Settingtype;
        public string SettingText;
    }

}
