using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ShardingCore.Core;
using ShardingCore.Core.VirtualRoutes;
using ShardingCore.Helpers;
using ShardingCore.VirtualRoutes.Abstractions;

namespace ShardingCore.VirtualRoutes.Days
{
/*
* @Author: xjm
* @Description:
* @Date: Wednesday, 27 January 2021 08:56:38
* @Email: 326308290@qq.com
*/
    public abstract class AbstractSimpleShardingDayKeyLongVirtualTableRoute<T>:AbstractShardingTimeKeyLongVirtualTableRoute<T> where T:class,IShardingTable
    {

        public abstract DateTime GetBeginTime();
        public override List<string> GetAllTails()
        {
            var beginTime = GetBeginTime();
         
            var tails=new List<string>();
            //提前创建表
            var nowTimeStamp = DateTime.Now.AddDays(1).Date;
            if (beginTime > nowTimeStamp)
                throw new ArgumentException("起始时间不正确无法生成正确的表名");
            var currentTimeStamp = beginTime;
            while (currentTimeStamp <= nowTimeStamp)
            {
                var currentTimeStampLong=ShardingCoreHelper.ConvertDateTimeToLong(currentTimeStamp);
                var tail = TimeFormatToTail(currentTimeStampLong);
                tails.Add(tail);
                currentTimeStamp = currentTimeStamp.AddDays(1);
            }
            return tails;
        }
        protected override string TimeFormatToTail(long time)
        {
            var dateTime=ShardingCoreHelper.ConvertLongToDateTime(time);
            return $"{dateTime:yyyyMMdd}";
        }

        protected override Expression<Func<string, bool>> GetRouteToFilter(long shardingKey, ShardingOperatorEnum shardingOperator)
        {
            var t = TimeFormatToTail(shardingKey);
            switch (shardingOperator)
            {
                case ShardingOperatorEnum.GreaterThan:
                case ShardingOperatorEnum.GreaterThanOrEqual:
                    return tail => String.Compare(tail, t, StringComparison.Ordinal) >= 0;
                case ShardingOperatorEnum.LessThan:
                {
                    var dateTime = ShardingCoreHelper.ConvertLongToDateTime(shardingKey);
                    var shardingKeyDate = dateTime.Date;
                    //处于临界值 o=>o.time < [2021-01-01 00:00:00] 尾巴20210101不应该被返回
                    if (shardingKeyDate == dateTime)
                        return tail => String.Compare(tail, t, StringComparison.Ordinal) < 0;
                    return tail => String.Compare(tail, t, StringComparison.Ordinal) <= 0;
                }
                case ShardingOperatorEnum.LessThanOrEqual:
                    return tail => String.Compare(tail, t, StringComparison.Ordinal) <= 0;
                case ShardingOperatorEnum.Equal: return tail => tail == t;
                default:
                {
#if DEBUG
                    Console.WriteLine($"shardingOperator is not equal scan all table tail");
#endif
                    return tail => true;
                }
            }
        }

    }
}