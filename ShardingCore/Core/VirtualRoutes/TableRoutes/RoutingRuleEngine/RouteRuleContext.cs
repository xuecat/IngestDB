using System.Linq;
using ShardingCore.Core.VirtualTables;

namespace ShardingCore.Core.VirtualRoutes.TableRoutes.RoutingRuleEngine
{
/*
* @Author: xjm
* @Description:
* @Date: Thursday, 28 January 2021 10:54:52
* @Email: 326308290@qq.com
*/
    public class RouteRuleContext<T>
    {
        private readonly IVirtualTableManager _virtualTableManager;

        public RouteRuleContext(string connectKey,IQueryable<T> queryable, IVirtualTableManager virtualTableManager)
        {
            ConnectKey = connectKey;
            Queryable = queryable;
            _virtualTableManager = virtualTableManager;
        }

        public string ConnectKey { get; }
        public IQueryable<T> Queryable { get; }

        ///// <summary>
        ///// 
        ///// </summary>
        //public readonly Dictionary<IVirtualTable, Expression> ManualPredicate = new Dictionary<IVirtualTable, Expression>();
        //public readonly Dictionary<IVirtualTable, ISet<string>> ManualTails = new Dictionary<IVirtualTable, ISet<string>>();
        
        //public bool AutoParseRoute = true;


        ///// <summary>
        ///// 启用自动路由
        ///// </summary>
        //public void EnableAutoRouteParse()
        //{
        //    AutoParseRoute = true;
        //}

        ///// <summary>
        ///// 禁用自动路由
        ///// </summary>
        //public void DisableAutoRouteParse()
        //{
        //    AutoParseRoute = false;
        //}

        ///// <summary>
        ///// 添加手动路由
        ///// </summary>
        ///// <param name="predicate"></param>
        ///// <typeparam name="TShardingEntity"></typeparam>
        //public void AddRoute<TShardingEntity>(Expression<Func<TShardingEntity, bool>> predicate) where TShardingEntity : class, IShardingTable
        //{
        //    var virtualTable = _virtualTableManager.GetVirtualTable<TShardingEntity>();
        //    if (!ManualPredicate.ContainsKey(virtualTable))
        //    {
        //        ShardingCore.Extensions.ExpressionExtension.And((Expression<Func<TShardingEntity, bool>>) ManualPredicate[virtualTable], predicate);
        //    }
        //    else
        //    {
        //        ManualPredicate.Add(virtualTable, predicate);
        //    }
        //}
        //public void AddRoute(Type shardingEntityType,string tail)
        //{
        //    var virtualTable = _virtualTableManager.GetVirtualTable(shardingEntityType);
        //    AddRoute(virtualTable, tail);
        //}
        
        //public void AddRoute<TShardingEntity>(string tail) where TShardingEntity : class, IShardingTable
        //{
        //    AddRoute(typeof(TShardingEntity), tail);
        //}
        
        //public void AddRoute(IVirtualTable virtualTable, string tail)
        //{
        //    if (ManualTails.ContainsKey(virtualTable))
        //    {
        //        var tails = ManualTails[virtualTable];
        //        if (!tails.Contains(tail))
        //        {
        //            tails.Add(tail);
        //        }
        //    }
        //    else
        //    {
        //        ManualTails.Add(virtualTable, new HashSet<string>()
        //        {
        //            tail
        //        });
        //    }
        //}
    }
}