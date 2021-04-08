﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ShardingCore.Exceptions
{
    /*
    * @Author: xjm
    * @Description:
    * @Date: 2021/3/5 15:18:55
    * @Ver: 1.0
    * @Email: 326308290@qq.com
    */
    public class ShardingCoreException: Exception
    {
        public ShardingCoreException()
        {
        }

        protected ShardingCoreException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ShardingCoreException(string message) : base(message)
        {
        }

        public ShardingCoreException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
