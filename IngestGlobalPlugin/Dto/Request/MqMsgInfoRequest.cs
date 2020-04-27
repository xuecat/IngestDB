using System;
using System.Collections.Generic;
using System.Text;

namespace IngestGlobalPlugin.Dto
{
    public class MqMsgInfoRequest
    {
        /// <summary>消息的类型，是SDI的还是文件引入的</summary>
        /// <example>123345</example>
        public MsgSourceType Type { set; get; }    //
        /// <summary>消息ID,唯一</summary>
        /// <example>123345</example>
        public string MsgId { set; get; }//
        /// <summary>消息内容</summary>
        /// <example>123345</example>
        public string MsgContent { set; get; }//
        /// <summary>消息发送的时间</summary>
        /// <example>123345</example>
        public string MsgSendTime { set; get; }//
        /// <summary>消息到达的时间</summary>
        /// <example>123345</example>
        public string MsgRevTime { set; get; }//
        /// <summary>当前消息的状态</summary>
        /// <example>123345</example>
        public MqmsgStatus MsgStatus { set; get; }//
        /// <summary>消息最后一次被处理的时间</summary>
        /// <example>123345</example>
        public string MsgProcessTime { set; get; }//
        /// <summary>失败的次数</summary>
        /// <example>123345</example>
        public int FailedCount { set; get; }//
        /// <summary>下次重试时间</summary>
        /// <example>123345</example>
        public string NextRetry { set; get; }//
        /// <summary>加锁</summary>
        /// <example>123345</example>
        public string Lock { set; get; }//
        /// <summary>对于没有处理过的消息，这个ID是0，失败重试的消息，这个ID是策略ID</summary>
        /// <example>123345</example>
        public int ActionID { set; get; } = -1;//
    }
}
