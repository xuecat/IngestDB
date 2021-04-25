using Microsoft.EntityFrameworkCore;
using ShardingCore.Core.VirtualRoutes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;

namespace IngestTaskPlugin.Models.Route
{
    public class DbpTaskMetadataRoute : AbstractShardingAutoIncreaseTableRoute<DbpTaskMetadata>
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
                }).Where(l => l != null).ToList();

                _shardingTails.Add(new ShardTableInfo()
                {
                    Min = 0,
                    Max = 0,
                    Key = $"{tablename}",
                    Tail = $"",
                    NeedCreate = false
                });

                var count = db.QueryMysql<int>("select count(0) from " + tablename);
                var maxid = db.QueryMysql<int>("select max(taskid) from " + tablename);
                if (count > 50000)
                {
                    var limittaskid = maxid / (count / 50000);
                    var lst = db.Set<DbpTaskMetadata>().Where(x => x.Taskid <= limittaskid).OrderBy(x => x.Taskid);

                    if (lst != null)
                    {
                        _shardingTaskMetadataList = lst.ToList();
                        if (_shardingTaskMetadataList != null && _shardingTaskMetadataList.Count > 0)
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
