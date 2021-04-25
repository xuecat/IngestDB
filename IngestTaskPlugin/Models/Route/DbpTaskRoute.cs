using IngestTaskPlugin.Dto.OldResponse;
using Microsoft.EntityFrameworkCore;
using ShardingCore.Core.VirtualRoutes;
using ShardingCore.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;

namespace IngestTaskPlugin.Models.Route
{
    public class DbpTaskRoute : AbstractShardingAutoIncreaseTableRoute<DbpTask>
    {
        private List<DbpTask> _shardingTaskList = null;

        public override void PrepareCreateTable(DbContext db, string tablename)
        {
            if (_shardingTails == null)
            {
                _shardingTails = db.QueryListMysql<string>("show tables").Where(x => x.Contains(tablename + "_"))
                .Select(k =>
                {
                    if (Regex.IsMatch(k, $"^({tablename})" + @"_(\d+_\d+)_(\d+_\d+)$"))
                    {
                        var lst = k.Split("_");
                        if (lst.Length > 2)
                        {
                            return new ShardTableInfo()
                            {
                                Min = long.Parse(lst[lst.Length - 4]),
                                Max = long.Parse(lst[lst.Length - 3]),
                                Begin = long.Parse(lst[lst.Length - 2]),
                                End = long.Parse(lst[lst.Length - 1]),
                                Key = k,
                                Tail = $"_{lst[lst.Length - 4]}_{lst[lst.Length - 3]}_{lst[lst.Length - 2]}_{lst[lst.Length - 1]}",
                                NeedCreate = false
                            };
                        }
                    }

                    return null;
                }).Where(f => f != null).ToList();

                _shardingTails.Add(new ShardTableInfo()
                {
                    Min = 0,
                    Max = 0,
                    Key = $"{tablename}",
                    Tail = $"",
                    NeedCreate = false
                });

                var tasklst = db.QueryMysql<int>("select count(0) from " + tablename);
                if (tasklst > 50000)
                {
                    DateTime dt = DateTime.Now;
                    _shardingTaskList = db.Set<DbpTask>().Take(50000).ToList();
                    _shardingTaskList = _shardingTaskList.TakeWhile((x, z) => z < 41000 || (x.Tasktype != (int)TaskType.TT_PERIODIC || x.Endtime < dt)).ToList();

                    if (_shardingTaskList != null && _shardingTaskList.Count > 0)
                    {
                        db.RemoveRange(_shardingTaskList);
                        long mintime = ShardingCoreHelper.ConvertDateTimeToLong(_shardingTaskList.Min(x => x.Endtime));
                        long maxtime = ShardingCoreHelper.ConvertDateTimeToLong(_shardingTaskList.Max(x => x.Endtime));
                        int minid = _shardingTaskList.Min(x => x.Taskid);
                        int maxid = _shardingTaskList.Max(x => x.Taskid);
                        _shardingTails.Add(new ShardTableInfo()
                        {
                            Min = minid,
                            Max = maxid,
                            Begin = mintime,
                            End = maxtime,
                            Key = $"{tablename}_{minid}_{maxid}_{mintime}_{maxtime}",
                            Tail = $"_{minid}_{maxid}_{mintime}_{maxtime}",
                            NeedCreate = true
                        });
                        db.SaveChanges();
                    }
                }

                if (_shardingTails.Any())
                {
                    _shardingTails.OrderBy(x => x.Min);
                }
            }
        }

        public override void EndCreateTable(DbContext db, string tail)
        {
            if (_shardingTails != null)
            {
                var task = _shardingTails.Find(x => x.Key.IndexOf(tail) > 1);
                if (task != null && task.NeedCreate && _shardingTaskList != null && _shardingTaskList.Count > 0)
                {
                    db.AddRange(_shardingTaskList);
                    db.SaveChanges();
                }
            }
        }
        protected override Func<string, bool> GetShardingTabkeFilter(IQueryable queryable)
        {
            //尝试走2,没有就让源码走1
            QueryRouteShardingTableVisitorTwo<DateTime> visitor = new QueryRouteShardingTableVisitorTwo<DateTime>(
                new Tuple<Type, string>(typeof(DateTime), "Endtime"),
                );
            //QueryableRouteShardingTableDiscoverVisitor<int> visitor 
            //    = new QueryableRouteShardingTableDiscoverVisitor<int>(
            //        ShardingCore.Utils.ShardingKeyUtil.Parse(typeof(DbpTask)), ConvertToShardingKey, GetRouteToFilter);
            visitor.Visit(queryable.Expression);

            return visitor.GetStringFilterTail();
        }

        protected override Expression<Func<string, bool>> GetRouteToFilter(int shardingKey, ShardingOperatorEnum shardingOperator)
        {
            var t = ShardingKeyToTail(shardingKey);
            switch (shardingOperator)
            {
                case ShardingOperatorEnum.GreaterThan:
                case ShardingOperatorEnum.GreaterThanOrEqual:
                case ShardingOperatorEnum.LessThan:
                case ShardingOperatorEnum.LessThanOrEqual:
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
