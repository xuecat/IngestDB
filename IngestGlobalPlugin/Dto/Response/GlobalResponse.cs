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
        public TC_TYPE tcType { get; set; }

        public int nTC { get; set; }
    }

    public class UserTemplate
    {
        public int TemplateID = -1;                    //模板ID
        public string UserCode = string.Empty;       //用户名
        public string TemplateName = string.Empty;   //模板名
        public string TemplateContent = string.Empty;//模板内容   
    }

    public class CMUserInfo
    {
        public string CreateTime;
        public bool Disabled;
        public string Email;
        public string Id;
        public string LoginName;
        public string NickName;
    }

}
