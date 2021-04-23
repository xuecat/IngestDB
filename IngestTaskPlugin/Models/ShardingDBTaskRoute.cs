using IngestTask.Dto;
using Microsoft.EntityFrameworkCore;
using ShardingCore.Core;
using ShardingCore.Core.VirtualRoutes;
using ShardingCore.Core.VirtualRoutes.TableRoutes;
using ShardingCore.Helpers;
using ShardingCore.VirtualRoutes.Months;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;

namespace IngestTaskPlugin.Models
{
    public class ShardTableInfo
    {
        public long Min { get; set; }
        public long Max { get; set; }
        public string Key { get; set; }
        public string Tail { get; set; }
        public bool NeedCreate { get; set; }
    }
    public static class RouteExtension
    {
        public static T QueryMysql<T>(this DbContext context, string querystring)
        {
            using (var command = context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = querystring;//"show tables";
                context.Database.OpenConnection();
                using (var result = command.ExecuteReader())
                {
                    while (result.Read())
                    {
                        return result.GetFieldValue<T>(0);
                    }
                }
            }
            return default(T);
        }

        public static List<T> QueryListMysql<T>(this DbContext context, string querystring)
        {
            List<T> lst = new List<T>();
            using (var command = context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = querystring;//"show tables";
                context.Database.OpenConnection();
                using (var result = command.ExecuteReader())
                {
                    while (result.Read())
                    {
                        lst.Add(result.GetFieldValue<T>(0));
                    }
                }
            }
            return lst;
        }
    }

    public abstract class AbstractShardingAutoIncreaseTableRoute<T, F> : AbstractShardingOperatorVirtualTableRoute<T, F> where T : class, IShardingTable
    {
        protected List<ShardTableInfo> _shardingTails = null;

        protected override F ConvertToShardingKey(object shardingKey)
        {
            return (F)Convert.ChangeType(shardingKey, typeof(F));
        }
        public override string ShardingKeyToTail(object shardingKey)
        {
            //var shardingKeyInt = ConvertToShardingKey(shardingKey);
            //var item = _shardingTails.Find(x => shardingKeyInt >= x.Min && shardingKeyInt <= x.Max);
            //return item.Tail;
            return string.Empty;
        }
        public override List<string> GetAllTails()
        {
            if (_shardingTails != null)
            {
                return _shardingTails.Select(x => x.Tail).ToList();
            }
            return new List<string>();
        }

        public override bool NeedCreateTable(string tail)
        {
            if (_shardingTails != null)
            {
                return _shardingTails.Find(x => x.Key.Contains(tail)).NeedCreate;
            }
            return false;
        }
    }

    public class ShardingDBTaskRoute : AbstractShardingAutoIncreaseTableRoute<DbpTask, DateTime>
    {
        private List<DbpTask> _shardingTaskList = null;

        public override void PrepareCreateTable(DbContext db, string tablename)
        {
            if (_shardingTails == null)
            {
                _shardingTails = db.QueryListMysql<string>("show tables").Where(x => x.Contains(tablename + "_"))
                .Select(k =>
                {
                    if (Regex.IsMatch(k, $"^({tablename})"+@"_(\d+_\d+)$"))
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
                }).Where(f => f!=null).ToList();

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
                    var date = DateTime.Now.AddDays(-7);
                    _shardingTaskList = db.Set<DbpTask>().Where(x => x.Endtime < date).OrderBy(x => x.Taskid).Take(50000).ToList();
                    
                    if (_shardingTaskList!= null && _shardingTaskList.Count >0)
                    {
                        db.RemoveRange(_shardingTaskList);
                        long mintime = ShardingCoreHelper.ConvertDateTimeToLong(_shardingTaskList.Min(x => x.Endtime));
                        long maxtime = ShardingCoreHelper.ConvertDateTimeToLong(_shardingTaskList.Max(x => x.Endtime));
                        _shardingTails.Add(new ShardTableInfo()
                        {
                            Min = mintime,
                            Max = maxtime,
                            Key = $"{tablename}_{mintime}_{maxtime}",
                            Tail = $"_{mintime}_{maxtime}",
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
                if (task != null && task.NeedCreate && _shardingTaskList!= null && _shardingTaskList.Count >0)
                {
                    db.AddRange(_shardingTaskList);
                    db.SaveChanges();
                }
            }
        }

        protected string TimeFormatToTail(DateTime time)
        {
            if (time == DateTime.MinValue || time == DateTime.MaxValue)//这么搞了,如果想用快速更新的都会走默认表
            {
                return "";
            }
            var datelong = ShardingCoreHelper.ConvertDateTimeToLong(time);
            var item = _shardingTails.Find(x => datelong >= x.Min && datelong <= x.Max);
            if (item != null)
            {
                return item.Tail;
            }
            return "";
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
                        var currentMonth = ShardingCoreHelper.GetCurrentMonday(shardingKey);
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

    public class ShardingDBTaskMetadataRoute : AbstractShardingAutoIncreaseTableRoute<DbpTaskMetadata, int>
    {
        private List<DbpTaskMetadata> _shardingTaskMetadataList = null;

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
                                Min = int.Parse(lst[lst.Length - 2]),
                                Max = int.Parse(lst[lst.Length - 1]),
                                Key = k,
                                Tail = $"_{lst[lst.Length - 2]}_{lst[lst.Length - 1]}",
                                NeedCreate = false
                            };
                        }
                    }
                    
                    return null;
                }).Where(l=>l!=null).ToList();

                _shardingTails.Add(new ShardTableInfo()
                {
                    Min = 0,
                    Max = 0,
                    Key = $"{tablename}",
                    Tail = $"",
                    NeedCreate = false
                });

                var count = db.QueryMysql<int>("select count(0) from " + tablename);
                var maxid = db.QueryMysql<int>("select max(taskid) from "+ tablename);
                if (count > 50000)
                {
                    var limittaskid = maxid/(count/50000);
                    var lst = db.Set<DbpTaskMetadata>().Where(x => x.Taskid <= limittaskid).OrderBy(x => x.Taskid);

                    if (lst != null)
                    {
                        _shardingTaskMetadataList = lst.ToList();
                        if (_shardingTaskMetadataList != null && _shardingTaskMetadataList.Count >0)
                        {
                            db.RemoveRange(_shardingTaskMetadataList);
                            db.SaveChanges();
                            _shardingTails.Add(new ShardTableInfo()
                            {
                                Min = _shardingTaskMetadataList.First().Taskid,
                                Max = _shardingTaskMetadataList.Last().Taskid,
                                Key = $"{tablename}_{_shardingTaskMetadataList.First().Taskid}_{_shardingTaskMetadataList.Last().Taskid}",
                                Tail = $"_{_shardingTaskMetadataList.First().Taskid}_{_shardingTaskMetadataList.Last().Taskid}",
                                NeedCreate = true
                            });
                        }
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
                if (task != null && task.NeedCreate)
                {
                    db.AddRange(_shardingTaskMetadataList);
                    db.SaveChanges();
                }
            }
            
        }

        public override string ShardingKeyToTail(object shardingKey)
        {
            var shardingKeyInt = ConvertToShardingKey(shardingKey);
            var item = _shardingTails.Find(x => shardingKeyInt >= x.Min && shardingKeyInt <= x.Max);
            if (item != null)
            {
                return item.Tail;
            }
            return "";
        }

        protected override Expression<Func<string, bool>> GetRouteToFilter(int shardingKey, ShardingOperatorEnum shardingOperator)
        {
            var t = ShardingKeyToTail(shardingKey);
            switch (shardingOperator)
            {
                //id查询应该只有equal吧
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
