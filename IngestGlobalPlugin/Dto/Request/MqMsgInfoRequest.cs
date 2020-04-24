using System;
using System.Collections.Generic;
using System.Text;

namespace IngestGlobalPlugin.Dto.Request
{
    public class MqMsgInfoRequest
    {
        public MsgSourceType Type { set; get; }    //消息的类型，是SDI的还是文件引入的
        public string MsgId { set; get; }//消息ID,唯一
        public string MsgContent { set; get; }//消息内容
        public string MsgSendTime { set; get; }//消息发送的时间
        public string MsgRevTime { set; get; }//消息到达的时间
        public MqmsgStatus MsgStatus { set; get; }//当前消息的状态
        public string MsgProcessTime { set; get; }//消息最后一次被处理的时间
        public int FailedCount { set; get; }//失败的次数
        public string NextRetry { set; get; }//下次重试时间
        public string Lock { set; get; }//加锁
        public int ActionID { set; get; } = -1;//对于没有处理过的消息，这个ID是0，失败重试的消息，这个ID是策略ID
    }
}
