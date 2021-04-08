using System.Threading;
using ShardingCore.Core.VirtualTables;

namespace ShardingCore.Core.ShardingAccessors
{
/*
* @Author: xjm
* @Description:
* @Date: Tuesday, 22 December 2020 15:14:15
* @Email: 326308290@qq.com
*/
    /// <summary>
    /// 分表访问器
    /// </summary>
    public class ShardingAccessor : IShardingAccessor
    {
        private static AsyncLocal<ShardingContext> _shardingContext = new AsyncLocal<ShardingContext>();

        /// <inheritdoc />
        public ShardingContext ShardingContext
        {
            get => _shardingContext.Value;
            set => _shardingContext.Value = value;
        }
    }
}