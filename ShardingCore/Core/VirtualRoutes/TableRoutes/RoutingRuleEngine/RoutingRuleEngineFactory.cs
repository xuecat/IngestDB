using System;
using System.Collections.Generic;
using System.Linq;
using ShardingCore.Core.VirtualTables;

namespace ShardingCore.Core.VirtualRoutes.TableRoutes.RoutingRuleEngine
{
/*
* @Author: xjm
* @Description:
* @Date: Thursday, 28 January 2021 13:31:06
* @Email: 326308290@qq.com
*/
    public class RoutingRuleEngineFactory : IRoutingRuleEngineFactory
    {
        private readonly IRouteRuleEngine _routeRuleEngine;
        private readonly IVirtualTableManager _virtualTableManager;

        public RoutingRuleEngineFactory(IRouteRuleEngine routeRuleEngine,IVirtualTableManager virtualTableManager)
        {
            _routeRuleEngine = routeRuleEngine;
            _virtualTableManager = virtualTableManager;
        }

        public IRouteRuleEngine CreateEngine()
        {
            return _routeRuleEngine;
        }

        public RouteRuleContext<T> CreateContext<T>(string connectKey, IQueryable<T> queryable)
        {
            return new RouteRuleContext<T>(connectKey,queryable, _virtualTableManager);
        }

        public IEnumerable<RouteResult> Route<T>(string connectKey, IQueryable<T> queryable, Func<DateTime, DateTime, bool> tablefilter)
        {
            var engine = CreateEngine();
            var ruleContext = CreateContext<T>(connectKey,queryable);
            return engine.Route(ruleContext, tablefilter);
        }

        public IEnumerable<RouteResult> Route<T>(string connectKey, IQueryable<T> queryable, RouteRuleContext<T> ruleContext, Func<DateTime, DateTime, bool> tablefilter)
        {
            var engine = CreateEngine();
            return engine.Route(ruleContext, tablefilter);
        }
    }
}