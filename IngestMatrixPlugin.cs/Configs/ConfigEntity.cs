using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IngestMatrixPlugin.Configs
{
    public class ConfigEntity
    {
        [XmlRoot("PublicSetting")]
        public class ConfigEntity
        {
            private SystemNode _systemNode;
            private Other _other;

            [XmlElement("Other")]
            public Other other
            {
                set { _other = value; }
                get { return _other; }
            }

            [XmlElement("System")]
            public SystemNode systemNode
            {
                set { _systemNode = value; }
                get { return _systemNode; }
            }

            //         public DBConfig dbConfig
            //         {
            //             set { _dbConfig = value; }
            //             get { return _dbConfig; }
            //         }

            private List<DatabaseConfig> dataBase = new List<DatabaseConfig>();

            [XmlArray("DBConfig")]
            [XmlArrayItem("DatabaseConfig")]
            public List<DatabaseConfig> DBConfig
            {
                get { return dataBase; }
            }


        }

        public class Other
        {
            [XmlElement("Encoding")]
            public int encoding { set; get; }
        }

        public class SystemNode
        {
            [XmlElement("Sys_VIP")]
            public string sysVip { set; get; }
            [XmlElement("DB_VIP")]
            public string dbVip { set; get; }
            [XmlElement("CMServer")]
            public string cmServer { set; get; }
            [XmlElement("CMserver_windows")]
            public string cmSerWin { set; get; }
            [XmlElement("CMweb")]
            public string cmWeb { set; get; }
            [XmlElement("Fls")]
            public string fls { set; get; }
            [XmlElement("FLSvr")]
            public string flsSvr { set; get; }
            [XmlElement("Jove")]
            public string jove { set; get; }
            [XmlElement("PLS_Windows")]
            public string plsWin { set; get; }
            [XmlElement("IngestDBSvr")]
            public string ingestDbSvr { set; get; }
            [XmlElement("IngestDEVCTL")]
            public string ingestDevCtl { set; get; }
            [XmlElement("IngestTaskSvr")]
            public string ingestTaskSvr { set; get; }
            [XmlElement("IngestMsgSvr")]
            public string ingestMsgSvr { set; get; }
            [XmlElement("Otc")]
            public string otc { set; get; }
            [XmlElement("SangHa")]
            public string sangHa { set; get; }
            [XmlElement("SangHaSvr")]
            public string sangHaSvr { set; get; }

            [XmlElement("SangHaWeb")]
            public string sangHaWeb { set; get; }

            [XmlElement("SNSServer")]
            public string snsSvr { set; get; }
        }
    }
}
