using System;
using System.Collections.Generic;
using System.Text;

namespace IngestDBCore
{

    public class ResponseMessage
    {
        /// <summary>
        /// 返回是否成功 SuccessCode = "0"  ModelStateInvalid = "100" ArgumentNullError = "101" ObjectAlreadyExists = "102" PartialFailure = "103"  NotFound = "404"  NotAllow = "403" ServiceError = "500"
        /// </summary>
        /// <example>0</example>
        public string Code { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string Msg { get; set; }

        public ResponseMessage()
        {
            Code = ResponseCodeDefines.SuccessCode;
            Msg = "OK";
        }

        public bool IsSuccess()
        {
            if (Code == ResponseCodeDefines.SuccessCode)
            {
                return true;
            }
            return false;
        }
    }

    public class ResponseMessage<TEx> : ResponseMessage
    {
        /// <summary>返回数据</summary>
        public TEx Ext { get; set; }
    }

    //public class PagingResponseMessage<Tentity> : ResponseMessage<List<Tentity>>
    //{
    //    public int PageIndex { get; set; }

    //    public int PageSize { get; set; }

    //    public long TotalCount { get; set; }
    //}
}
