using IngestGlobalPlugin.Dto.OldResponse;
using System;
using System.Collections.Generic;
using System.Text;

namespace IngestGlobalPlugin.Dto.Response
{
    class GlobalResponse
    {

    }

    public class GlobalTcResponse
    {
        public TC_TYPE TcType { get; set; }

        public int TC { get; set; }
    }

    public class DtoGlobalStateResponse
    {
        public string Label { get; set; }// = string.Empty;
        public string LastTime { get; set; }//DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss");
    }

    public class DtoUserTemplate
    {
        /// <summary>模板ID</summary>
        /// <example>-1</example>
        public int TemplateID { get; set; }//= -1;                    //模板ID
        /// <summary>用户名</summary>
        /// <example>8de083d45c614628b99516740d628e91</example>
        public string UserCode { get; set; }//= string.Empty;       //用户名
        /// <summary>模板名</summary>
        /// <example>tName</example>
        public string TemplateName { get; set; }//= string.Empty;   //模板名
        /// <summary>模板内容</summary>
        /// <example><window_positions>...</window_positions></example>
        public string TemplateContent { get; set; }//= string.Empty;//模板内容   
    }

    public class DtoCMUserInfo
    {
        public string CreateTime { get; set; }
        public bool Disabled { get; set; }
        public string Email { get; set; }
        public string Id { get; set; }
        public string LoginName { set; get; }
        public string NickName { get; set; }
    }

    public class EditUserTemplate
    {
        /// <summary>模板名</summary>
        /// <example>TName</example>
        public string TemplateName { get; set; }
        /// <summary>模板内容</summary>
        /// <example><window_positions>...</window_positions></example>
        public string TemplateContent { get; set; }
    }

    public class CapParamTemplate
    {
        public int ID { get; set; }
        public string TemplateName { get; set; }//采集参数模板
        public string ParamTemplate { get; set; }//参数模板内容采集
    }

    public class DtoSetUserSettingRequest
    {
        /// <summary>用户Code</summary>
        /// <example>06c70a52172d4393beb1bb6743ca6944</example>
        public string UserCode { get; set; }
        /// <summary>settingType</summary>
        /// <example>UserMoudleData</example>
        public string Settingtype { get; set; }
        /// <summary>内容</summary>
        /// <example>Content...</example>
        public string SettingText { get; set; }
    }

    public class DtoLockObjectParamIn
    {
        /// <summary>对象Id</summary>
        /// <example>3538</example>
        public int ObjectID { get; set; }
        /// <summary>对象类型Id</summary>
        /// <example>3</example>
        public OTID ObjectTypeID { get; set; }
        /// <summary>用户名</summary>
        /// <example>RecMQServer</example>
        public string UserName { get; set; }
        /// <summary>延时</summary>
        /// <example>30000</example>
        public int TimeOut { get; set; }
    }

}
