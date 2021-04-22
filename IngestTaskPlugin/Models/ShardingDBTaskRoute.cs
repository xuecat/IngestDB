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
        public int Min { get; set; }
        public int Max { get; set; }
        public string Key { get; set; }
        public bool NeedCreate { get; set; }
    }
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
                        backinfo.Add(result.GetFieldValue<T>(0));
                    }
                }
            }
            return backinfo;
        }
    }

    public abstract class AbstractShardingAutoIncreaseTableRoute<T> : AbstractShardingOperatorVirtualTableRoute<T, int> where T : class, IShardingTable
    {
        protected List<ShardTableInfo> _shardingTails = null;

        protected override int ConvertToShardingKey(object shardingKey)
        {
            return Convert.ToInt32(shardingKey);
        }
        public override string ShardingKeyToTail(object shardingKey)
        {
            var shardingKeyInt = ConvertToShardingKey(shardingKey);
            var item = _shardingTails.Find(x => shardingKeyInt >= x.Min && shardingKeyInt <= x.Max);
            return $"{item.Min}_{item.Max}";
        }
        public override List<string> GetAllTails()
        {
            if (_shardingTails != null)
            {
                return _shardingTails.Select(x => x.Key).ToList();
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

    public class ShardingDBTaskRoute : AbstractShardingAutoIncreaseTableRoute<DbpTask>
    {
        private List<DbpTask> _shardingTaskList = null;

        public override void PrepareCreateTable(DbContext db, string tablename)
        {
            if (_shardingTails == null)
            {
                _shardingTails = db.QueryMysql<string>("show tables").Where(x => x.Contains(tablename + "_"))
                .Select(k =>
                {
                    if (Regex.IsMatch(k, @"^\w+_(\d+_\d+)$"))
                    {
                        var lst = k.Split("_");
                        if (lst.Length > 2)
                        {
                            return new ShardTableInfo()
                            {
                                Min = int.Parse(lst[lst.Length - 2]),
                                Max = int.Parse(lst[lst.Length - 1]),
                                Key = k,
                                NeedCreate = false
                            };
                        }
                    }
                    
                    return null;
                }).Where(f => f!=null).ToList();

                var tasklst = db.QueryMysql<int>("select count(0) from " + tablename);
                if (tasklst != null && tasklst.Count > 0 && tasklst[0] > 50000)
                {
                    var date = DateTime.Now.AddDays(-3);
                    _shardingTaskList = db.Set<DbpTask>().AsNoTracking().Where(x => x.Endtime < date
                    && (x.State == (int)taskState.tsComplete || x.State == (int)taskState.tsDelete || x.State == (int)taskState.tsInvaild)).OrderBy(x => x.Taskid).Take(50000).ToList();

                    db.RemoveRange(_shardingTaskList);
                    db.SaveChanges();

                    _shardingTails.Add(new ShardTableInfo()
                    {
                        Min = _shardingTaskList.First().Taskid,
                        Max = _shardingTaskList.Last().Taskid,
                        Key = $"{tablename}_{_shardingTaskList.First().Taskid}_{_shardingTaskList.Last().Taskid}",
                        NeedCreate = true
                    });

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
                    db.AddRange(_shardingTaskList);
                    db.SaveChanges();
                }
            }
            
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

    public class ShardingDBTaskMetadataRoute : AbstractShardingAutoIncreaseTableRoute<DbpTaskMetadata>
    {
        private List<DbpTaskMetadata> _shardingTaskMetadataList = null;

        public override void PrepareCreateTable(DbContext db, string tablename)
        {
            if (_shardingTails == null)
            {
                _shardingTails = db.QueryMysql<string>("show tables").Where(x => x.Contains(tablename + "_"))
                .Select(k =>
                {
                    if (Regex.IsMatch(k, @"^\w+_(\d+_\d+)$"))
                    {
                        var lst = k.Split("_");
                        if (lst.Length > 2)
                        {
                            return new ShardTableInfo()
                            {
                                Min = int.Parse(lst[lst.Length - 2]),
                                Max = int.Parse(lst[lst.Length - 1]),
                                Key = k,
                                NeedCreate = false
                            };
                        }
                    }
                    
                    return null;
                }).Where(l=>l!=null).ToList();

                var tasklst = db.QueryMysql<int>("select count(0), max(taskid), min(taskid) from " + tablename);
                if (tasklst != null && tasklst.Count > 0 && tasklst[0] > 50000)
                {
                    var date = DateTime.Now.AddDays(-3);
                    var limittaskid = tasklst[1]- 1000;
                    _shardingTaskMetadataList = db.Set<DbpTaskMetadata>().AsNoTracking().Where(x => x.Taskid <= limittaskid).OrderBy(x => x.Taskid).ToList();

                    db.RemoveRange(_shardingTaskMetadataList);
                    db.SaveChanges();

                    _shardingTails.Add(new ShardTableInfo()
                    {
                        Min = _shardingTaskMetadataList.First().Taskid,
                        Max = _shardingTaskMetadataList.Last().Taskid,
                        Key = $"{tablename}_{_shardingTaskMetadataList.First().Taskid}_{_shardingTaskMetadataList.Last().Taskid}",
                        NeedCreate = true
                    });

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
