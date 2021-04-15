using ShardingCore.Core.VirtualRoutes;
using ShardingCore.Helpers;
using ShardingCore.VirtualRoutes.Months;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace IngestTaskPlugin.Models
{
    public class DBTaskRoute : AbstractSimpleShardingMonthKeyDateTimeVirtualTableRoute<DbpTask>
    {
        public override DateTime GetBeginTime()
        {
            return new DateTime(2021, 1, 01);
        }

        public override List<string> GetAllTails()
        {
            var beginTime = ShardingCoreHelper.GetCurrentMonthFirstDay(GetBeginTime());

            var tails = new List<string>();
            //提前创建表
            var nowTimeStamp = ShardingCoreHelper.GetNextMonthFirstDay(DateTime.Now);
            if (beginTime > nowTimeStamp)
                throw new ArgumentException("起始时间不正确无法生成正确的表名");
            var currentTimeStamp = beginTime;
            while (currentTimeStamp <= nowTimeStamp)
            {
                var tail = ShardingKeyToTail(currentTimeStamp);
                tails.Add(tail);
                currentTimeStamp = ShardingCoreHelper.GetNextMonthFirstDay(currentTimeStamp);
            }
            return tails;
        }

        protected override Expression<Func<string, bool>> GetRouteToFilter(DateTime shardingKey, ShardingOperatorEnum shardingOperator)
        {
            var t = TimeFormatToTail(shardingKey);
            switch (shardingOperator)
            {
                case ShardingOperatorEnum.GreaterThan:
                case ShardingOperatorEnum.GreaterThanOrEqual:
                    return tail => String.Compare(tail, t, StringComparison.Ordinal) >= 0;
                case ShardingOperatorEnum.LessThan:
                    {
                        var currentMonth = ShardingCoreHelper.GetCurrentMonthFirstDay(shardingKey);
                        //处于临界值 o=>o.time < [2021-01-01 00:00:00] 尾巴20210101不应该被返回
                        if (currentMonth == shardingKey)
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

    public class DBTaskMetadataRoute : AbstractSimpleShardingMonthKeyDateTimeVirtualTableRoute<DbpTaskMetadata>
    {
        public override DateTime GetBeginTime()
        {
            return new DateTime(2021, 1, 01);
        }
    }
}
