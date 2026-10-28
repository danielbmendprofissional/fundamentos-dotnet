using System;
using System.Linq.Expressions;

namespace Onion.Domain.Common.Specifications
{
    internal static class ExpressionCombiner
    {
        public static Expression<Func<T, bool>>? Combine<T>(Expression<Func<T, bool>>? left, Expression<Func<T, bool>>? right, ExpressionType expressionType)
        {
            if (left == null) return right;
            if (right == null) return left;

            var parameter = Expression.Parameter(typeof(T));
            var leftBody = ParameterRebinder.Replace(left.Parameters[0], parameter, left.Body);
            var rightBody = ParameterRebinder.Replace(right.Parameters[0], parameter, right.Body);

            BinaryExpression? body = expressionType switch
            {
                ExpressionType.AndAlso => Expression.AndAlso(leftBody, rightBody),
                ExpressionType.OrElse => Expression.OrElse(leftBody, rightBody),
                _ => throw new NotSupportedException($"Unsupported expression type: {expressionType}")
            };

            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        private class ParameterRebinder : ExpressionVisitor
        {
            private readonly ParameterExpression _oldParam;
            private readonly ParameterExpression _newParam;

            private ParameterRebinder(ParameterExpression oldParam, ParameterExpression newParam)
            {
                _oldParam = oldParam;
                _newParam = newParam;
            }

            public static Expression Replace(ParameterExpression oldParam, ParameterExpression newParam, Expression exp)
            {
                return new ParameterRebinder(oldParam, newParam).Visit(exp);
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return node == _oldParam ? _newParam : base.VisitParameter(node);
            }
        }
    }
}