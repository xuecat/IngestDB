using System;
using System.Collections.Generic;

namespace ShardingCore.DbContexts.Abstractions
{
/*
* @Author: xjm
* @Description:
* @Date: Friday, 12 March 2021 22:25:10
* @Email: 326308290@qq.com
*/
    public interface IDbContextCreateFilterManager
    {

        void RegisterFilter(IDbContextCreateFilter filter);
        List<IDbContextCreateFilter> GetFilters();
    }
}