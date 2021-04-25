using Microsoft.EntityFrameworkCore;
using ShardingCore.Core;
using ShardingCore.Core.VirtualRoutes.TableRoutes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IngestTaskPlugin.Models.Route
{
    public class ShardTableInfo
    {
        public long Min { get; set; }
        public long Max { get; set; }
        public long Begin { get; set; }
        public long End { get; set; }
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

    public abstract class AbstractShardingAutoIncreaseTableRoute<T> : AbstractShardingOperatorVirtualTableRoute<T, int> where T : class, IShardingTable
    {
        protected List<ShardTableInfo> _shardingTails = null;

        protected override int ConvertToShardingKey(object shardingKey)
        {
            return (int)shardingKey;
            //return (F)Convert.ChangeType(shardingKey, typeof(F));
        }

        public override List<string> GetAllTails()
        {
            if (_shardingTails != null)
            {
                return _shardingTails.Select(x => x.Tail).ToList();
            }
            return new List<string>();
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

        public override bool NeedCreateTable(string tail)
        {
            if (_shardingTails != null)
            {
                return _shardingTails.Find(x => x.Key.Contains(tail)).NeedCreate;
            }
            return false;
        }

        protected override Func<string, bool> GetShardingTabkeFilter(IQueryable queryable)
        {
            return null;
        }
    }
    
}
