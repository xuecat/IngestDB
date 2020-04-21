using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using IngestMatrixPlugin.Dto;
using Sobey.Core.Log;

namespace IngestMatrixPlugin.Configs
{
    public class DataConfig
    {
        private static readonly ILogger Logger = LoggerManager.GetLogger(nameof(DataConfig));
        //数据平台对外发布ip port,外部调用
        //mysql相关
        public static string mysql_ip;
        public static string mysql_port;
        public static string database_name;
        public static string database_user;
        public static string database_password;
        //设备相关
        public static string IngestMatrixServerIP;
        public static string IngestMatrixServerPort;
        public static string IngestMsvServerIP;
        public static string IngestMsvServerPort;
        public static string IngestVtrServerIP;
        public static string IngestVtrServerPort;
        public static string IngestOmneonServerIP;
        public static string IngestOmneonServerPort;





        //本地应用，数据平台自身发布ip
        public static string dbserver_ip;
        public static string dbserver_port;
        //cmserver地址
        public static string cmserver_ip;
        public static string cmserver_port;
        private static string userToken = null;


        //获取配置
        public static void InitDataConfig()
        {
            //加载xml得到cmserver
            LoadXmlConfig();
            //得到usertoken
            userToken = GetUserToken();

            //数据平台唯二需要在网管获取的参数
            if (IngestMatrixServerIP == "" || IngestMatrixServerIP == null)
                IngestMatrixServerIP = GetGlobalParam("IngestDeviceCtrlIP");
            if (IngestMatrixServerPort == "" || IngestMatrixServerPort == null)
                IngestMatrixServerPort = GetGlobalParam("IngestDeviceCtrlPort");

            Logger.Info("mysql_ip：" + mysql_ip);
            Logger.Info("mysql_port：" + mysql_port);
            Logger.Info("database_name：" + database_name);
            Logger.Info("database_user：" + database_user);
            Logger.Info("database_password：" + database_password);
            Logger.Info("IngestMatrixServerIP：" + IngestMatrixServerIP);
            Logger.Info("IngestMatrixServerPort：" + IngestMatrixServerPort);
        }



        //获取usertoken
        private static void LoadXmlConfig()
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                //doc.Load("Config.xml");
                //XmlNode root = doc.DocumentElement;
                //MOSConfig.initConfig(AppDomain.CurrentDomain.BaseDirectory);
                PulicSetting.InitConfig(@"publicsetting.xml");
                //if (MOSConfig.CMAPiHost != null && !string.IsNullOrEmpty(MOSConfig.CMAPiHost))
                if (PulicSetting.cmSerIp != null && !string.IsNullOrEmpty(PulicSetting.cmSerIp))
                {
                    cmserver_ip = PulicSetting.cmSerIp;
                    Logger.Error(string.Format("Get cmserver_ip By PublicSetting.xml , cmserver_ip:{0}", PulicSetting.cmSerIp));
                }
                else
                {
                    //                     XmlNode CMServerIP = root.SelectSingleNode("cmserver_ip");
                    //                     if (CMServerIP != null)
                    //                         cmserver_ip = CMServerIP.InnerText;
                    //                     Logger.Error(string.Format("Get cmserver_ip By LocalConfig.xml , cmserver_ip:{0}", cmserver_ip));

                }

                //if (MOSConfig.CMAPiHost != null && MOSConfig.CMAPIPORT != 0)
                if (PulicSetting.cmSerPort != 0)
                {
                    cmserver_port = PulicSetting.cmSerPort.ToString();
                }
                else
                {
                    //                     XmlNode CMServerPORT = root.SelectSingleNode("cmserver_port");
                    //                     if (CMServerPORT != null)
                    //                         cmserver_port = CMServerPORT.InnerText;

                }

                if (!string.IsNullOrEmpty(PulicSetting.dbIp))
                {
                    mysql_ip = PulicSetting.dbIp;
                }

                if (PulicSetting.mySqlPort != 0)
                {
                    mysql_port = PulicSetting.mySqlPort.ToString();
                }

                if (PulicSetting.mySqlInstance != null && !string.IsNullOrEmpty(PulicSetting.mySqlInstance))
                {
                    database_name = PulicSetting.mySqlInstance;
                }

                if (PulicSetting.mySqlUserName != null && !string.IsNullOrEmpty(PulicSetting.mySqlUserName))
                {
                    database_user = PulicSetting.mySqlUserName;
                }


                if (PulicSetting.mySqlPassWord != null && !string.IsNullOrEmpty(PulicSetting.mySqlPassWord))
                {
                    bool bEncoding = PulicSetting.isEncoding;

                    if (bEncoding)
                    {
                        database_password = cspwd.Base64ex.Base64_Decode(PulicSetting.mySqlPassWord);
                        Logger.Error("Decoding password:" + PulicSetting.mySqlPassWord + "," + database_password);
                    }
                    else
                    {
                        database_password = PulicSetting.mySqlPassWord;
                        Logger.Error("database_password:" + PulicSetting.mySqlPassWord + "," + database_password);
                    }
                }

                if (PulicSetting.ingestDeCtlIp != null && !string.IsNullOrEmpty(PulicSetting.ingestDeCtlIp))
                {
                    IngestMatrixServerIP = PulicSetting.ingestDeCtlIp;
                }

                if (PulicSetting.ingestDeCtlPort != 0)
                {
                    IngestMatrixServerPort = PulicSetting.ingestDeCtlPort.ToString();
                }

                if (PulicSetting.ingestDbSvrIp != null && !string.IsNullOrEmpty(PulicSetting.ingestDbSvrIp))
                {

                    if (Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX)
                    {
                        dbserver_ip = "+";//PulicSetting.ingestDbSvrIp;
                        //dbserver_ip = PulicSetting.ingestDbSvrIp;
                    }
                    else
                    {
                        dbserver_ip = PulicSetting.ingestDbSvrIp;
                    }
                }

                if (PulicSetting.ingestDbSvrPort != 0)
                {
                    dbserver_port = "9000";//PulicSetting.ingestDbSvrPort.ToString();
                }
                Logger.Info("mysql_ip：" + mysql_ip);
                Logger.Info("mysql_port：" + mysql_port);
                Logger.Info("database_name：" + database_name);
                Logger.Info("database_user：" + database_user);
                Logger.Info("database_password：" + database_password);
                Logger.Info("IngestMatrixServerIP：" + IngestMatrixServerIP);
                Logger.Info("IngestMatrixServerPort：" + IngestMatrixServerPort);
            }
            catch (Exception ex)
            {
                Logger.Error("LoadXmlConfig" + ex.Message);
            }

            //读取公共配置中数据库相关内容
            //                 string str = "";
            //                 if ((Environment.OSVersion.Platform == PlatformID.Unix) || (Environment.OSVersion.Platform == PlatformID.MacOSX))
            //                 {
            //                     str = "/publicsetting.xml";
            //                 }
            //                 else
            //                 {
            //                     str = string.Format(@"{0}publicsetting.xml", AppDomain.CurrentDomain.BaseDirectory);
            //                 }
            //                 Logger.Error("Loading path...., path:"  + str);
            //                 if(!File.Exists(str))
            //                 {
            //                     Logger.Error("Path is not exists!");
            //                     return;
            //                 }
            //                 string s = File.ReadAllText(str);
            //                 Logger.Error("s:" + s);
            //                 doc.LoadXml(s);
            //                 bool bFind = false;
            //                 bool bEncoding = false;
            //                 root = doc.DocumentElement;
            //                 if (root != null)
            //                 {
            //                     XmlNode nodeSystem = root.SelectSingleNode("System");
            //                     if (nodeSystem != null)
            //                     {
            //                         string strTemp = "";
            //                         XmlNode dbIP = nodeSystem.SelectSingleNode("Sys_VIP");
            //                         if (dbIP != null)
            //                             mysql_ip = dbIP.InnerText;
            // 
            //                         XmlNode matrixIpPort = nodeSystem.SelectSingleNode("IngestDEVCTL");
            //                         if (matrixIpPort != null)
            //                         {
            //                             strTemp = matrixIpPort.InnerText;
            //                             int pos = strTemp.IndexOf(':');
            //                             if (pos != -1)
            //                             {
            //                                 IngestMatrixServerIP = strTemp.Substring(0, pos);
            //                                 IngestMatrixServerPort = strTemp.Substring(pos + 1, strTemp.Length - pos - 1);
            //                             }
            //                         }
            //                     }
            //                     //读取other节点
            //                     XmlNode nodeOther = root.SelectSingleNode("Other");
            //                     if (nodeOther != null)
            //                     {
            //                         XmlNode nodeEncoding = nodeOther.SelectSingleNode("Encoding");
            //                         if (nodeEncoding != null)
            //                         {
            //                             if(nodeEncoding.InnerText == "1")
            //                                 bEncoding = true;
            //                         }
            //                     }
            //                 }
            //                 XmlNodeList nodeList = doc.DocumentElement.GetElementsByTagName("DatabaseConfig");
            //                 if (nodeList != null)
            //                 {
            //                     foreach (XmlNode node in nodeList)
            //                     {
            //                         if (node.Attributes["module"].InnerText == "INGESTDB")
            //                         {
            //                             bFind = true;
            //                             XmlNode dbBaseName = node.SelectSingleNode("Instance");
            //                             if (dbBaseName != null)
            //                                 database_name = dbBaseName.InnerText;
            //                             XmlNode dbBaseUser = node.SelectSingleNode("Username");
            //                             if (dbBaseUser != null)
            //                                 database_user = dbBaseUser.InnerText;
            //                             XmlNode dbBasePW = node.SelectSingleNode("Password");
            //                             if (dbBasePW != null)
            //                             {
            //                                 //如果密码被加密，在此处解密
            //                                 if (bEncoding)
            //                                 {
            //                                     database_password = cspwd.Base64ex.Base64_Decode(dbBasePW.InnerText);
            //                                     Logger.Error("Decoding password:" + dbBasePW.InnerText + "," + database_password);
            //                                 }
            //                                 else
            //                                     database_password = dbBasePW.InnerText;
            //                             }
            //                             XmlNode dataPort = node.SelectSingleNode("Port");
            //                             if (dataPort != null)
            //                                 mysql_port = dataPort.InnerText;
            // 
            //                             break;
            //                         }
            //                     }
            //                 }
            //                 else
            //                 {
            //                     Logger.Error("DatabaseConfig xmlNodeList is not exit");
            //                 }
            //                 if(bFind)
            //                 {
            //                     Logger.Error("find INGESTDB");
            //                 }
            //                 else
            //                 {
            //                     Logger.Error("not find INGESTDB");
            //                 }
            //             }
            //             catch(XmlException ex)
            //             {
            //                 Logger.Error("LoadXmlConfig" + ex.Message);
            //             }
            //             catch (System.Exception ex)
            //             {
            //                 Logger.Error("LoadXmlConfig" + ex.Message);
            //             }


        }
        private static string GetUserToken()
        {
            return "ingest";
            loginData login = new loginData();

            login.LOGINNAME = "admin";
            login.LOGINPWD = "21232f297a57a5a743894a0e4a801fc3";
            login.LOGINSUBSYSTEM = "sys";
            login.LOGINIP = "127.0.0.1";

            string strLogin = JsonConvert.SerializeObject(login);

            StringContent strJsonIn = new StringContent(strLogin);
            strJsonIn.Headers.ContentType.MediaType = "application/json";

            string uri = string.Format("http://{0}:{1}/CMApi/api/basic/account/login", cmserver_ip, cmserver_port);
            HttpClient client = new HttpClient();

            try
            {
                var response = client.PostAsync(uri, strJsonIn).Result;
                string strJson = response.Content.ReadAsStringAsync().Result;
                ResponseMessageN<SmmUserlogininfo> info = new ResponseMessageN<SmmUserlogininfo>();
                info = JsonConvert.DeserializeObject<ResponseMessageN<SmmUserlogininfo>>(strJson);
                if (info.Code == "0")
                {
                    return info.ext.usertoken;
                }
                else
                {
                    //Logger.Error("获取usertoken失败" + ": " + info.msg);
                    return null;
                }
            }
            catch (Exception exp)
            {
                string strLog = "";
                strLog = exp.ToString();
                return null;
            }
        }

        /// <summary>
        /// 获取用户绑定的采集参数模板ID
        /// </summary>
        /// <param name="szUserToken"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public static int GetUserParamTemplateID(bool busetokencode, string szUserToken, string username)//nFlag：
        {
            int nCaptureParamID = -1;

            parameter param = new parameter();
            param.tool = "DEFAULT";
            param.paramname = "HIGH_RESOLUTION";
            param.system = "INGEST";
            try
            {
                string strLogin = JsonConvert.SerializeObject(param);
                StringContent strJsonIn = new StringContent(strLogin);
                strJsonIn.Headers.ContentType.MediaType = "application/json";
                string uri = string.Format("http://{0}:{1}/CMApi/api/basic/config/getuserparam/", cmserver_ip, cmserver_port);
                HttpClient client = new HttpClient();

                if (busetokencode)//使用code来请求
                    client.DefaultRequestHeaders.Add("sobeyhive-http-token", szUserToken);
                else
                    client.DefaultRequestHeaders.SetHeaderValue(szUserToken);

                var strReturn = client.PostAsync(uri, strJsonIn).Result.Content.ReadAsStringAsync().Result;

                ResponseMessageN<etparam> res = new ResponseMessageN<etparam>();
                res = JsonConvert.DeserializeObject<ResponseMessageN<etparam>>(strReturn);
                if (res.Code == "0")
                {
                    nCaptureParamID = Convert.ToInt32(res.ext.paramvalue);
                    return nCaptureParamID;
                }
                else
                {
                    Logger.Error(string.Format("DataConfig GetUserParamTemplateID 失败：{0} {1}", busetokencode, szUserToken));
                    return 1;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("DataConfig GetUserParamTemplateID 异常：{0} ", ex.Message));
                return 1;
            }
        }

        private static string GetGlobalParam(string key)
        {
            parameter param = new parameter();
            param.tool = "DEFAULT";
            param.system = "INGEST";
            param.paramname = key;

            string strJson = JsonConvert.SerializeObject(param);
            StringContent strContent = new StringContent(strJson);

            string uri = string.Format("http://{0}:{1}/CMApi/api/basic/config/getsysparam/", cmserver_ip, cmserver_port);
            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.SetHeaderValue();
            client.DefaultRequestHeaders.Add("sobeyhive-http-token", userToken);
            var response = client.PostAsync(uri, strContent).Result;
            string strRet = response.Content.ReadAsStringAsync().Result;

            ResponseMessageN<etparam> res = new ResponseMessageN<etparam>();
            res = JsonConvert.DeserializeObject<ResponseMessageN<etparam>>(strRet);
            if (res.Code == "0")
            {
                return res.ext.paramvalue;
            }
            else
            {
                Logger.Error(string.Format("GetGlobalParam发生错误 key：{0},错误信息：{1}", key, res.msg));
                return null;
            }
        }
        public static string GetUserPath(bool busetokencode, string userToken, string storagetype, string storagemark, ref string path)
        {
            try
            {
                string uri = string.Format("http://{0}:{1}/CMApi/api/basic/user/getcurrentusercanwritepathbycondition?storagetype={2}&storagemark={3}", cmserver_ip, cmserver_port, storagetype, storagemark);
                Logger.Error("DataConfig::GetUserPath uri: " + uri + " userToken:" + userToken, "");
                HttpClient client = new HttpClient();

                if (busetokencode)
                {
                    client.DefaultRequestHeaders.Add("sobeyhive-http-token", userToken);
                }
                else
                    client.DefaultRequestHeaders.SetHeaderValue(userToken);

                var response = client.GetAsync(uri).Result;
                string strRet = response.Content.ReadAsStringAsync().Result;
                Logger.Error("DataConfig::GetUserPath strRet: " + strRet, "");
                ResponseMessageG<ext> res = new ResponseMessageG<ext>();
                res = JsonConvert.DeserializeObject<ResponseMessageG<ext>>(strRet);
                if (res.code == "0")
                {
                    path = res.ext.path;
                    return path;
                }
                else
                {
                    //Logger.Error("GetMysqlIP发生错误 ");
                    Logger.Error("", "DataConfig::GetUserPath error,get cmsrv info error!errorinfo={1}", res.msg);
                    return "";
                }
            }
            catch (Exception ex)
            {
                Logger.Error("DataConfig::GetUserPath error,get cmsrv info error!errorinfo=" + ex.ToString(), "");
                throw ex;
                //return null;
            }
        }
    }
}
