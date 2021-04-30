using System;
using System.Linq;
using System.Linq.Expressions;

namespace ShardingCore.Core.VirtualRoutes.TableRoutes
{
/*
* @Author: xjm
* @Description:
* @Date: Friday, 18 December 2020 14:15:02
* @Email: 326308290@qq.com
*/
    public class TableRouteConfig
    {
        private readonly IQueryable _queryable;
        private readonly IShardingTable _shardingTable;
        private readonly object _shardingKeyValue;
        private readonly Expression _predicate;

        private string _extraTail;
        public TableRouteConfig(IQueryable queryable=null,IShardingTable shardingTable=null,object shardingKeyValue=null,Expression predicate=null)
        {
            _queryable = queryable;
            _shardingTable = shardingTable;
            _shardingKeyValue = shardingKeyValue;
            _predicate = predicate;
        }

        public TableRouteConfig SetExtraTail(string extra) { _extraTail = extra; return this; }
        public string GetExtraTail() { return _extraTail; }

        public IQueryable GetQueryable()
        {
            return _queryable;
        }
        public object GetShardingKeyValue()
        {
            return _shardingKeyValue;
        }

        public IShardingTable GetShardingEntity()
        {
            return _shardingTable;
        }

        public Expression GetPredicate()
        {
            return _predicate;
        }

        public bool UseQueryable()
        {
            return _queryable != null;
        }

        public bool UseValue()
        {
            return _shardingKeyValue != null;
        }

        public bool UseEntity()
        {
            return _shardingTable != null;
        }

        public bool UsePredicate()
        {
            return _predicate != null;
        }
    }
}