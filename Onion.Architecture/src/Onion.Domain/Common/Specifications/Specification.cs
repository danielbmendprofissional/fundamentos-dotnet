using System;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Onion.Domain.Common.Specifications
{
    // Implementação concreta e fluente para construir specifications no Application layer
    public sealed class Specification<T> : BaseSpecification<T>
    {
        public Specification() { }

        public Specification(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria;
        }

        public Specification<T> Where(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria;
            return this;
        }

        public Specification<T> And(Expression<Func<T, bool>> other)
        {
            Criteria = ExpressionCombiner.Combine(Criteria, other, ExpressionType.AndAlso);
            return this;
        }

        public Specification<T> Or(Expression<Func<T, bool>> other)
        {
            Criteria = ExpressionCombiner.Combine(Criteria, other, ExpressionType.OrElse);
            return this;
        }

        public Specification<T> Not()
        {
            if (Criteria == null) return this;
            var param = Criteria.Parameters[0];
            var body = Expression.Not(Criteria.Body);
            Criteria = Expression.Lambda<Func<T, bool>>(body, param);
            return this;
        }

        public Specification<T> WithInclude(Expression<Func<T, object>> includeExpression)
        {
            AddInclude(includeExpression);
            return this;
        }

        public Specification<T> WithOrderBy(Expression<Func<T, object>> orderByExpression)
        {
            ApplyOrderBy(orderByExpression);
            return this;
        }

        public Specification<T> WithOrderByDescending(Expression<Func<T, object>> orderByDescExpression)
        {
            ApplyOrderByDescending(orderByDescExpression);
            return this;
        }

        public Specification<T> WithPaging(int skip, int take)
        {
            ApplyPaging(skip, take);
            return this;
        }

        // Cria uma nova Specification combinando duas existentes (mantém includes/order/paging da primeira)
        public static Specification<T> Combine(Specification<T> a, Specification<T> b, ExpressionType op)
        {
            var result = new Specification<T>();
            result.Criteria = ExpressionCombiner.Combine(a?.Criteria, b?.Criteria, op);
            // copia includes/order/paging do 'a' (convenção) para que quem construir specs decida
            if (a != null)
            {
                foreach (var inc in a.Includes) result.WithInclude(inc);
                if (a.OrderBy != null) result.WithOrderBy(a.OrderBy);
                if (a.OrderByDescending != null) result.WithOrderByDescending(a.OrderByDescending);
                if (a.IsPagingEnabled && a.Skip.HasValue && a.Take.HasValue) result.WithPaging(a.Skip.Value, a.Take.Value);
            }
            return result;
        }
    }
}