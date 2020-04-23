namespace IngestDevicePlugin.Dto.Request
{
    public class DeviceChannelExtdataRequest
    {        
        /// <summary>数据类型</summary>
        /// <example>2</example>
        public int Datatype { get; set; }
        /// <summary>扩展数据内容</summary>
        /// <example>\\\\storage.test.com\\hivefiles\\sobeyhive\\buckets\\u-qhbyb6s9mp763brm\\lv_res\\2020-02-19\\Untitled_Ingest_2020-02-19_18_14_45_20200219181447_0_0_221_4080__000__low.mp4</example>
        public string Extenddata { get; set; }
    }
}
