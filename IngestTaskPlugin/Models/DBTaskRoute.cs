using IngestTask.Dto;
using Microsoft.EntityFrameworkCore;
using ShardingCore.Core.VirtualRoutes;
using ShardingCore.Helpers;
using ShardingCore.VirtualRoutes.Months;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace IngestTaskPlugin.Models
{
    public static class RouteExtension
    {
        public static List<T> QueryMysql<T>(this DbContext context, string querystring)
        {
            var backinfo = new List<T>();
            using (var command = context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = querystring;//"show tables";
                context.Database.OpenConnection();
                using (var result = command.ExecuteReader())
                {
                    while (result.Read())
                    {
                        backinfo.Add((T)Convert.ChangeType(result.GetString(0), typeof(T)));
                    }
                }
            }
            return backinfo;
        }
    }

    public class DBTaskRoute : AbstractSimpleShardingMonthKeyDateTimeVirtualTableRoute<DbpTask>
    {
        private List<DbpTask> _shardingTaskList = null;
        private Dictionary<string, bool> _shardingTails = null;
        public override DateTime GetBeginTime()
        {
            return new DateTime(2021, 1, 01);
        }

        public override void PrepareCreateTable(DbContext db, string tablename)
        {
            _shardingTails = db.QueryMysql<string>("show tables").FindAll(x => x.Contains(tablename + "_")).ToDictionary(k => k, g => false);

            var tasklst = db.QueryMysql<int>("select count(0) from " + tablename);
            if (tasklst != null && tasklst.Count > 0 && tasklst[0] > 50000)
            {
                var date = DateTime.Now.AddDays(-3);
                _shardingTaskList = db.Set<DbpTask>().AsNoTracking().Where(x => x.Endtime < date 
                && (x.State == (int)taskState.tsComplete || x.State == (int)taskState.tsDelete || x.State == (int)taskState.tsInvaild)).OrderBy(x => x.Taskid).Take(50000).ToList();

                db.RemoveRange(_shardingTaskList);
                db.SaveChanges();
                
                _shardingTails.Add($"{tablename}_{_shardingTaskList.First().Taskid}_{_shardingTaskList.Last().Taskid}", true);
            }
        }

        public override void EndCreateTable(DbContext db, string tail)
        {
            bool needcreate;
            if (_shardingTails.TryGetValue(tail, out needcreate))
            {
                db.AddRange(_shardingTaskList);
                db.SaveChanges();
            }
        }

        public override List<string> GetAllTails()
        {
            return _shardingTails.Keys.ToList();
            //var beginTime = ShardingCoreHelper.GetCurrentMonthFirstDay(GetBeginTime());

            //var tails = new List<string>();
            ////提前创建表
            //var nowTimeStamp = ShardingCoreHelper.GetNextMonthFirstDay(DateTime.Now);
            //if (beginTime > nowTimeStamp)
            //    throw new ArgumentException("起始时间不正确无法生成正确的表名");
            //var currentTimeStamp = beginTime;
            //while (currentTimeStamp <= nowTimeStamp)
            //{
            //    var tail = ShardingKeyToTail(currentTimeStamp);
            //    tails.Add(tail);
            //    currentTimeStamp = ShardingCoreHelper.GetNextMonthFirstDay(currentTimeStamp);
            //}
            //return tails;
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
