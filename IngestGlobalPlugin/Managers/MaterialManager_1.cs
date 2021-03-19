using IngestGlobalPlugin.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngestGlobalPlugin.Managers
{
    public partial class MaterialManager
    {
        #region v3.0

        public List<string> GetKafkaInfoByCreateDefault(int taskid, int kafkacmd)
        {
            List<string> result = new List<string>();
            string sendtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            if ((kafkacmd & IngestDBCore.Notify.IngestCmd.StartCapture) > 0)
            {
                string starttime = DateTime.Now.TimeOfDay.ToString().Split('.')[0] + ":00";

                string startcmd = $"<?xml version=\"1.0\" encoding=\"UTF-8\" ?><Notify><NotifyType>StartCapture</NotifyType><TaskID>{taskid}</TaskID><StartTime>{starttime}</StartTime><TCMode>1</TCMode><NameV0><![CDATA[\\\\172.16.0.202\\hivefiles\\sobeyhive\\buckets\\u-x583i30f58xanx8w\\hv_res\\2021-03-03\\Untitled_Ingest_2021-03-03_11_49_37_20210303114844_0_0_224_5504__000__high.mxf]]></NameV0><NameV1><![CDATA[\\\\172.16.0.202\\hivefiles\\sobeyhive\\buckets\\u-x583i30f58xanx8w\\lv_res\\2021-03-03\\Untitled_Ingest_2021-03-03_11_49_37_20210303114844_0_0_224_5504__000__low.mpd]]></NameV1><NameA0><![CDATA[\\\\172.16.0.202\\hivefiles\\sobeyhive\\buckets\\u-x583i30f58xanx8w\\hv_res\\2021-03-03\\Untitled_Ingest_2021-03-03_11_49_37_20210303114844_0_0_224_5504__000__high.mxf]]></NameA0><NameA1><![CDATA[\\\\172.16.0.202\\hivefiles\\sobeyhive\\buckets\\u-x583i30f58xanx8w\\lv_res\\2021-03-03\\Untitled_Ingest_2021-03-03_11_49_37_20210303114844_0_0_224_5504__000__low.a3.wav]]></NameA1><GOPCount>15</GOPCount><IFrame>1</IFrame><BFrame>2</BFrame><PFrame>4</PFrame><CloseGOP>0</CloseGOP><ErrorCode>0</ErrorCode><Winter_TaskID></Winter_TaskID><GUID>{Guid.NewGuid().ToString("N")}</GUID><SendTime>{sendtime}</SendTime></Notify>";

                result.Add(startcmd);
            }

            if ((kafkacmd & IngestDBCore.Notify.IngestCmd.StopCapture) > 0)
            {
                string stopcmd = $"<?xml version=\"1.0\" encoding=\"UTF-8\" ?><Notify><NotifyType>StopCapture</NotifyType><TaskID>{taskid}</TaskID><ClipSum>1</ClipSum><StreamEndTime>-1</StreamEndTime><ErrorCode>0</ErrorCode><GUID>{Guid.NewGuid().ToString("N")}</GUID><SendTime>{sendtime}</SendTime></Notify>";

                result.Add(stopcmd);
            }

            return result;
        }

        public async Task<List<string>> GetKafkaInfoFromDb(int taskid, int kafkacmd)
        {
            return await Store.GetMsmqmsgsContentByCmd(taskid, kafkacmd);
        }


        #endregion

    }
}
