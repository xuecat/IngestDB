namespace IngestDevicePlugin.Dto.Request
{
    public class DeviceChannelExtdataRequest
    {        
        /// <summary>数据类型</summary>
        /// <example>1</example>
        public int Datatype { get; set; }
        /// <summary>扩展数据内容</summary>
        /// <example>扩展数据内容-测试信息</example>
        public string Extenddata { get; set; }
    }
}
