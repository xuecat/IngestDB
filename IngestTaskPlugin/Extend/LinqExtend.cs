using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngestTaskPlugin.Extend
{
    public static class LinqExtend
    {
        public static TSource MaxItem<TSource, TCompareValue>(this IEnumerable<TSource> sourceList, Func<TSource, TCompareValue> CompareExpression)
        {
            var comparer = Comparer<TCompareValue>.Default;
            return sourceList.Aggregate((max, item) =>
            {
                var res = comparer.Compare(CompareExpression(max), CompareExpression(item));//小于0 max小于item, 0 max等于item，大于0 max大于item。
                return res < 0 ? item : max;//max小于item时 取 item
            });
        }

        public static TSource MinItem<TSource, TCompareValue>(this IEnumerable<TSource> sourceList, Func<TSource, TCompareValue> CompareExpression)
        {
            var comparer = Comparer<TCompareValue>.Default;
            return sourceList.Aggregate((min, item) =>
            {
                var res = comparer.Compare(CompareExpression(min), CompareExpression(item));//小于0 min小于item, 0 min等于item，大于0 min大于item。
                return res > 0 ? item : min;//min大于item时 取 item
            });
        }
    }
}
