using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using ShardingCore.Core.VirtualRoutes;
using ShardingCore.Core.VirtualTables;
using ShardingCore.Exceptions;
using ShardingCore.Extensions;

namespace IngestTaskPlugin.Models.Route
{
    public class QueryRouteShardingTableVisitorTwo<TKey, TV> : ExpressionVisitor
    {
        private int _useTwoShardingKey;
        private Tuple<Type, string> _shardingConfig2;
        private readonly Func<TKey, ShardingOperatorEnum, Expression<Func<TV, bool>>> _keyToTailWithFilter2;

        private readonly Tuple<Type, string> _shardingConfig1;
        private readonly Func<TKey, ShardingOperatorEnum, Expression<Func<TV, bool>>> _keyToTailWithFilter1;
        private readonly Func<object, TKey> _shardingKeyConvert;
        private Expression<Func<TV, bool>> _where = x => true;

        public QueryRouteShardingTableVisitorTwo(
            Tuple<Type, string> shardingConfig,
            Func<object, TKey> shardingKeyConvert,
            Func<TKey, ShardingOperatorEnum, Expression<Func<TV, bool>>> keyToTailWithFilter)
        {
            _useTwoShardingKey = 0;
            _shardingConfig1 = shardingConfig;
            _shardingKeyConvert = shardingKeyConvert;
            _keyToTailWithFilter1 = keyToTailWithFilter;
        }

        public QueryRouteShardingTableVisitorTwo(
            Tuple<Type, string> shardingConfig1,
            Tuple<Type, string> shardingConfig2,
            Func<object, TKey> shardingKeyConvert,
            Func<TKey, ShardingOperatorEnum, Expression<Func<TV, bool>>> keyToTailWithFilter1,
            Func<TKey, ShardingOperatorEnum, Expression<Func<TV, bool>>> keyToTailWithFilter2)
        {
            _useTwoShardingKey = 1;
            _shardingConfig1 = shardingConfig1;
            _shardingConfig2 = shardingConfig2;
            _shardingKeyConvert = shardingKeyConvert;
            _keyToTailWithFilter1 = keyToTailWithFilter1;
            _keyToTailWithFilter2 = keyToTailWithFilter2;
        }

        public Func<TV, bool> GetStringFilterTail()
        {
            if (_useTwoShardingKey == 1)
            {
                return null;
            }
            if (_where!= null)
            {
                return _where.Compile();
            }
            return null;
        }

        private bool IsShardingKey(Expression expression)
        {
            if (_useTwoShardingKey>0)
                return expression is MemberExpression member
                   && member.Expression.Type == _shardingConfig1.Item1
                   && (member.Member.Name == _shardingConfig1.Item2 || member.Member.Name == _shardingConfig2.Item2);
            else
                return expression is MemberExpression member
                   && member.Expression.Type == _shardingConfig1.Item1
                   && member.Member.Name == _shardingConfig1.Item2;
        }

        private bool IsShardingKey1(Expression expression)
        {
            return expression is MemberExpression member
                && member.Expression.Type == _shardingConfig1.Item1
                && member.Member.Name == _shardingConfig1.Item2;
        }
        private bool IsShardingKey2(Expression expression)
        {
            return expression is MemberExpression member
                && member.Expression.Type == _shardingConfig2.Item1
                && member.Member.Name == _shardingConfig2.Item2;
        }

        /// <summary>
        /// 方法是否包含shardingKey
        /// </summary>
        /// <param name="methodCallExpression"></param>
        /// <returns></returns>
        private bool IsMethodWrapShardingKey(MethodCallExpression methodCallExpression)
        {
            if (methodCallExpression.Arguments.IsNotEmpty())
            {
                for (int i = 0; i < methodCallExpression.Arguments.Count; i++)
                {
                    var isShardingKey = methodCallExpression.Arguments[i] is MemberExpression member
                                        && member.Expression.Type == _shardingConfig1.Item1
                                        && (member.Member.Name == _shardingConfig1.Item2 || (_useTwoShardingKey>0 && member.Member.Name == _shardingConfig2.Item2));
                    if (isShardingKey) return true;
                }
            }
            return false;
        }
        private bool IsMethodWrapShardingKey1(MethodCallExpression methodCallExpression)
        {
            if (methodCallExpression.Arguments.IsNotEmpty())
            {
                for (int i = 0; i < methodCallExpression.Arguments.Count; i++)
                {
                    var isShardingKey = methodCallExpression.Arguments[i] is MemberExpression member
                                        && member.Expression.Type == _shardingConfig1.Item1
                                        && member.Member.Name == _shardingConfig1.Item2;
                    if (isShardingKey) return true;
                }
            }
            return false;
        }
        private bool IsMethodWrapShardingKey2(MethodCallExpression methodCallExpression)
        {
            if (methodCallExpression.Arguments.IsNotEmpty())
            {
                for (int i = 0; i < methodCallExpression.Arguments.Count; i++)
                {
                    var isShardingKey = methodCallExpression.Arguments[i] is MemberExpression member
                                        && member.Expression.Type == _shardingConfig2.Item1
                                        && member.Member.Name == _shardingConfig2.Item2;
                    if (isShardingKey) return true;
                }
            }
            return false;
        }

        private bool IsConstantOrMember(Expression expression)
        {
            return expression is ConstantExpression
                   || (expression is MemberExpression member && (member.Expression is ConstantExpression || member.Expression is MemberExpression || member.Expression is MemberExpression));
        }

        private object GetFieldValue(Expression expression)
        {
            if (expression is ConstantExpression)
                return (expression as ConstantExpression).Value;
            if (expression is UnaryExpression)
            {
                UnaryExpression unary = expression as UnaryExpression;
                LambdaExpression lambda = Expression.Lambda(unary.Operand);
                Delegate fn = lambda.Compile();
                return fn.DynamicInvoke(null);
            }

            if (expression is MemberExpression member1Expression)
            {
                return Expression.Lambda(member1Expression).Compile().DynamicInvoke();
            }

            throw new ShardingKeyGetValueException("cant get value " + expression);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.Name == nameof(Queryable.Where))
            {
                if (node.Arguments[1] is UnaryExpression unaryExpression)
                {
                    if (unaryExpression.Operand is LambdaExpression lambdaExpression)
                    {
                        var newWhere = Resolve(lambdaExpression);
                        if (newWhere == null)
                        {
                            _where = null;
                        }
                        else
                            _where = newWhere;
                    }
                }
            }

            return base.VisitMethodCall(node);
        }


        private Expression<Func<TV, bool>> Resolve(Expression expression)
        {
            if (expression is LambdaExpression)
            {
                LambdaExpression lambda = expression as LambdaExpression;
                expression = lambda.Body;
                return Resolve(expression);
            }

            if (expression is BinaryExpression binaryExpression) //解析二元运算符
            {
                return ParseGetWhere(binaryExpression);
            }

            if (expression is UnaryExpression) //解析一元运算符
            {
                UnaryExpression unary = expression as UnaryExpression;
                if (unary.Operand is MethodCallExpression methodCall1Expression)
                {
                    // return ResolveLinqToObject(unary.Operand, false);
                    return ResolveInFunc(methodCall1Expression, unary.NodeType != ExpressionType.Not);
                }
            }

            if (expression is MethodCallExpression methodCallExpression) //解析扩展方法
            {
                return ResolveInFunc(methodCallExpression, true);
            }
            return x => true;
        }

        private Expression<Func<TV, bool>> ResolveInFunc(MethodCallExpression methodCallExpression, bool @in)
        {
            if (methodCallExpression.IsEnumerableContains(methodCallExpression.Method.Name) && IsMethodWrapShardingKey(methodCallExpression))
            {
                object arrayObject = null;
                if (methodCallExpression.Object != null)
                {
                    if (methodCallExpression.Object is MemberExpression member1Expression)
                    {
                        arrayObject = Expression.Lambda(member1Expression).Compile().DynamicInvoke();
                    }
                    else if (methodCallExpression.Object is ListInitExpression member2Expression)
                    {
                        arrayObject = Expression.Lambda(member2Expression).Compile().DynamicInvoke();
                    }
                }
                else if (methodCallExpression.Arguments[0] is MemberExpression member2Expression)
                {
                    arrayObject = Expression.Lambda(member2Expression).Compile().DynamicInvoke();
                }
                else if (methodCallExpression.Arguments[0] is NewArrayExpression member3Expression)
                {
                    arrayObject = Expression.Lambda(member3Expression).Compile().DynamicInvoke();
                }

                if (arrayObject != null)
                {
                    var enumerable = (IEnumerable)arrayObject;
                    Expression<Func<TV, bool>> contains = x => false;
                    if (!@in)
                        contains = x => true;
                    foreach (var item in enumerable)
                    {
                        var keyValue = _shardingKeyConvert(item);
                        if (IsMethodWrapShardingKey1(methodCallExpression))
                        {
                            var eq = _keyToTailWithFilter1(keyValue, @in ? ShardingOperatorEnum.Equal : ShardingOperatorEnum.NotEqual);
                            if (@in)
                                contains = contains.Or(eq);
                            else
                                contains = contains.And(eq);
                        }
                        else if (IsMethodWrapShardingKey2(methodCallExpression))
                        {
                            var eq = _keyToTailWithFilter2(keyValue, @in ? ShardingOperatorEnum.Equal : ShardingOperatorEnum.NotEqual);
                            if (@in)
                                contains = contains.Or(eq);
                            else
                                contains = contains.And(eq);
                        }
                    }
                    return contains;
                }
            }

            return x => true;
        }

        private Expression<Func<TV, bool>> ParseGetWhere(BinaryExpression binaryExpression)
        {
            Expression<Func<TV, bool>> left = x => true;
            Expression<Func<TV, bool>> right = x => true;

            //递归获取
            if (binaryExpression.Left is BinaryExpression)
                left = ParseGetWhere(binaryExpression.Left as BinaryExpression);
            if (binaryExpression.Left is MethodCallExpression methodCallExpression)
                left = Resolve(methodCallExpression);

            if (binaryExpression.Left is UnaryExpression unaryExpression)
                left = Resolve(unaryExpression);

            if (binaryExpression.Right is BinaryExpression)
                right = ParseGetWhere(binaryExpression.Right as BinaryExpression);

            //组合
            if (binaryExpression.NodeType == ExpressionType.AndAlso)
            {
                return left.And(right);
            }
            else if (binaryExpression.NodeType == ExpressionType.OrElse)
            {
                return left.Or(right);
            }
            //单个
            else
            {
                bool paramterAtLeft;
                object value = null;
                bool shardingkey1 = false;

                if (IsShardingKey(binaryExpression.Left) && IsConstantOrMember(binaryExpression.Right))
                {
                    paramterAtLeft = true;
                    value = GetFieldValue(binaryExpression.Right);
                    shardingkey1 = IsShardingKey1(binaryExpression.Left);
                }
                else if (IsConstantOrMember(binaryExpression.Left) && IsShardingKey(binaryExpression.Right))
                {
                    paramterAtLeft = false;
                    value = GetFieldValue(binaryExpression.Left);
                    shardingkey1 = IsShardingKey1(binaryExpression.Right);
                }
                else
                    return x => true;

                var op = binaryExpression.NodeType switch
                {
                    ExpressionType.GreaterThan => paramterAtLeft ? ShardingOperatorEnum.GreaterThan : ShardingOperatorEnum.LessThan,
                    ExpressionType.GreaterThanOrEqual => paramterAtLeft ? ShardingOperatorEnum.GreaterThanOrEqual : ShardingOperatorEnum.LessThanOrEqual,
                    ExpressionType.LessThan => paramterAtLeft ? ShardingOperatorEnum.LessThan : ShardingOperatorEnum.GreaterThan,
                    ExpressionType.LessThanOrEqual => paramterAtLeft ? ShardingOperatorEnum.LessThanOrEqual : ShardingOperatorEnum.GreaterThanOrEqual,
                    ExpressionType.Equal => ShardingOperatorEnum.Equal,
                    ExpressionType.NotEqual => ShardingOperatorEnum.NotEqual,
                    _ => ShardingOperatorEnum.UnKnown
                };

                if (value == null)
                    return x => true;


                var keyValue = _shardingKeyConvert(value);
                if (shardingkey1)
                {
                    if (_useTwoShardingKey == 1)
                    {
                        _useTwoShardingKey = 2;
                    }
                    return _keyToTailWithFilter1(keyValue, op);
                }
                else
                {
                    if (_useTwoShardingKey == 1)
                    {
                        _useTwoShardingKey = 2;
                    }
                    
                    return _keyToTailWithFilter2(keyValue, op);
                }
                    
            }
        }
    }
}
