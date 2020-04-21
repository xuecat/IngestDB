using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngestMatrixPlugin.Dto
{
    public class PulicSetting
    {

        public static string vipIp = null;
        public static string dbIp = null;
        public static string cmSerIp = null;
        public static int cmSerPort = 0;
        public static string cmSerWinIp = null;
        public static int cmSerWinPort = 0;
        public static string ingestDbSvrIp = null;
        public static int ingestDbSvrPort = 0;
        public static string ingestDeCtlIp = null;
        public static int ingestDeCtlPort = 0;
        public static string ingestTaskIp = null;
        public static int ingestTaskPort = 0;
        public static string ingestMsgSvrIp = null;
        public static int ingestMsgSvrPort = 0;

        public static string mySqlInstance = null;
        public static string mySqlUserName = null;
        public static string mySqlPassWord = null;
        public static int mySqlPort = 0;
        public static bool isEncoding = false;

        public static Dictionary<moduleType, DatabaseConfig> mySqlConf = new Dictionary<moduleType, DatabaseConfig>();
        private static void SplitIpAndPort(string str, ref string ip, ref int port)
        {
            if (str == null)
            {
                return;
            }
            string[] strArr = str.Split(':');
            ip = strArr[0];
            port = int.Parse(strArr[1]);
        }
        public static void InitConfig(string fileName = @"publicsetting.xml")
        {
            try
            {
                string str = "";
                if ((Environment.OSVersion.Platform == PlatformID.Unix) || (Environment.OSVersion.Platform == PlatformID.MacOSX))
                {
                    //str = string.Format(@"{0}/{1}", System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase, fileName);
                    str = '/' + fileName;
                }
                else
                {
                    str = string.Format(@"{0}{1}", System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase, fileName);
                }

                if (!File.Exists(str))
                {
                    //此处加日志
                    return;
                }

                string s = File.ReadAllText(str);
                ConfigEntity en = XmlSerializeUtil.Deserialize(typeof(ConfigEntity), s) as ConfigEntity;
                if (en == null)
                {
                    //此处加日志
                    return;
                }
                if (en.systemNode != null)
                {
                    vipIp = en.systemNode.sysVip;
                    dbIp = en.systemNode.dbVip;
                    SplitIpAndPort(en.systemNode.cmServer, ref cmSerIp, ref cmSerPort);
                    SplitIpAndPort(en.systemNode.cmSerWin, ref cmSerWinIp, ref cmSerWinPort);
                    SplitIpAndPort(en.systemNode.ingestDbSvr, ref ingestDbSvrIp, ref ingestDbSvrPort);
                    SplitIpAndPort(en.systemNode.ingestDevCtl, ref ingestDeCtlIp, ref ingestDeCtlPort);
                    SplitIpAndPort(en.systemNode.ingestTaskSvr, ref ingestTaskIp, ref ingestTaskPort);
                    SplitIpAndPort(en.systemNode.ingestMsgSvr, ref ingestMsgSvrIp, ref ingestMsgSvrPort);

                }

                if (en.other != null)
                {
                    isEncoding = Convert.ToBoolean(en.other.encoding);
                }
                if (en.DBConfig != null)
                {
                    for (int i = 0; i < en.DBConfig.Count; i++)
                    {
                        if (en.DBConfig[i] != null && en.DBConfig[i].moduleName != null)
                        {
                            moduleType _type = (moduleType)Enum.Parse(typeof(moduleType), en.DBConfig[i].moduleName, true);
                            DatabaseConfig dbBase = new DatabaseConfig();
                            dbBase.mySqlInstance = en.DBConfig[i].instance;
                            dbBase.mySqlUserName = en.DBConfig[i].userName;
                            dbBase.mySqlPassWord = en.DBConfig[i].passWord;
                            dbBase.mySqlPort = en.DBConfig[i].port;
                            mySqlConf.Add(_type, dbBase);

                            if (_type == moduleType.INGESTDB)
                            {
                                mySqlInstance = en.DBConfig[i].instance;
                                mySqlUserName = en.DBConfig[i].userName;
                                mySqlPassWord = en.DBConfig[i].passWord;
                                mySqlPort = en.DBConfig[i].port;
                                if (!String.IsNullOrEmpty(en.DBConfig[i].server))
                                    dbIp = en.DBConfig[i].server;
                            }
                        }
                    }
                }

                //如果没有配置任何的数据库IP，让数据库IP指向sys_vip
                if (String.IsNullOrEmpty(dbIp))
                    dbIp = vipIp;
            }
            catch (System.Exception e)
            {

            }

        }
    }
}
