using System;
using System.Linq;
using System.Linq.Expressions;

namespace ShardingCore.Core.VirtualDataSources
{
/*
* @Author: xjm
* @Description:
* @Date: Saturday, 06 February 2021 11:28:33
* @Email: 326308290@qq.com
*/
    public class VirutalDataSourceRouteConfig
    {
        
        private readonly IQueryable _queryable;
        private readonly IShardingDataSource _shardingDataSource;
        private readonly object _shardingKeyValue;
        private readonly Expression _predicate;


        public VirutalDataSourceRouteConfig(IQueryable queryable=null,IShardingDataSource shardingDataSource=null,object shardingKeyValue=null,Expression predicate=null)
        {
            _queryable = queryable;
            _shardingDataSource = shardingDataSource;
            _shardingKeyValue = shardingKeyValue;
            _predicate = predicate;
        }

        public IQueryable GetQueryable()
        {
            return _queryable;
        }
        public object GetShardingKeyValue()
        {
            return _shardingKeyValue;
        }

        public IShardingDataSource GetShardingDataSource()
        {
            return _shardingDataSource;
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
            return _shardingDataSource != null;
        }

        public bool UsePredicate()
        {
            return _predicate != null;
        }
    }
}