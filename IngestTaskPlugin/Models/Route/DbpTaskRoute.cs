using IngestTaskPlugin.Dto.OldResponse;
using Microsoft.EntityFrameworkCore;
using ShardingCore.Core.PhysicTables;
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
                    if (Regex.IsMatch(k, $"^({tablename})" + @"_(\d+_\d+)$"))
                    {
                        var lst = k.Split("_");
                        if (lst.Length > 2)
                        {
                            return new ShardTableInfo()
                            {
                                Min = long.Parse(lst[lst.Length - 2]),
                                Max = long.Parse(lst[lst.Length - 1]),
                                Key = k,
                                Tail = $"_{lst[lst.Length - 2]}_{lst[lst.Length - 1]}",
                                NeedCreate = false
                            };
                        }
                    }

                    return null;
                }).Where(f => f != null).ToList();

                var tasklst = db.QueryMysql<int>("select count(0) from " + tablename);
                if (tasklst > 50000)
                {
                    DateTime dt = DateTime.Now.AddDays(-30);
                    _shardingTaskList = db.Set<DbpTask>().AsNoTracking().Where(x => x.Endtime < dt
                   && (x.State == (int)taskState.tsComplete || x.State == (int)taskState.tsDelete || x.State == (int)taskState.tsInvaild))
                        .OrderBy(x => x.Taskid).Take(50000).ToList();

                    if (_shardingTaskList != null && _shardingTaskList.Count > 30000)
                    {
                        db.RemoveRange(_shardingTaskList);
                        int minid = _shardingTaskList.Min(x => x.Taskid);
                        int maxid = _shardingTaskList.Max(x => x.Taskid);
                        _shardingTails.Add(new ShardTableInfo()
                        {
                            Min = minid,
                            Max = maxid,
                            Key = $"{tablename}_{minid}_{maxid}",
                            Tail = $"_{minid}_{maxid}",
                            NeedCreate = true
                        });
                        db.SaveChanges();
                    }
                }

                if (_shardingTails.Any())
                {
                    _shardingTails.Add(new ShardTableInfo()
                    {
                        Min = 0,
                        Max = 0,
                        Key = $"{tablename}",
                        Tail = $"",
                        NeedCreate = false
                    }); ;

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

        /*
         * 二元解析endtime和starttime过滤对我来说太难了，写不动了，只能通过func过滤，后面往有能力的人可以完善
         */
        protected override List<IPhysicTable> DoRouteWithWhere(List<IPhysicTable> allPhysicTables, IQueryable queryable)
        {
            //尝试走2,没有就让源码走1
            QueryRouteShardingTableVisitorTwo<int, string> visitor1 = new QueryRouteShardingTableVisitorTwo<int, string>(
                new Tuple<Type, string>(typeof(DbpTask), "Taskid"),
                ConvertToShardingKey,
                GetRouteToFilter
            );
            visitor1.Visit(queryable.Expression);

            if (visitor1.GetHotOrCloudTable())
            {
                return allPhysicTables.FindAll(x => x.Tail == "");
            }
            return allPhysicTables;
        }

        //protected Expression<Func<ShardTableInfo, bool>> GetRouteToFilter1(DateTime shardingKey, ShardingOperatorEnum shardingOperator)
        //{
        //    long time = ShardingCoreHelper.ConvertDateTimeToLong(shardingKey);

        //    switch (shardingOperator)
        //    {
        //        case ShardingOperatorEnum.GreaterThan:
        //        case ShardingOperatorEnum.GreaterThanOrEqual:
        //            return tail => time >= tail.Begin;
        //        case ShardingOperatorEnum.LessThan:
        //        case ShardingOperatorEnum.LessThanOrEqual:
        //            return tail => time <= tail.End;
        //        case ShardingOperatorEnum.Equal: return tail => time >= tail.Begin && time <= tail.End;
        //        default:
        //            {
        //                return tail => true;
        //            }
        //    }
        //}

        protected override Expression<Func<string, bool>> GetRouteToFilter(int shardingKey, ShardingOperatorEnum shardingOperator)
        {
            //var t = ShardingKeyToTail(shardingKey);
            switch (shardingOperator)
            {
                case ShardingOperatorEnum.GreaterThan:
                case ShardingOperatorEnum.GreaterThanOrEqual:
                case ShardingOperatorEnum.LessThan:
                case ShardingOperatorEnum.LessThanOrEqual:
                case ShardingOperatorEnum.Equal: 
                    return tail => true;
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
