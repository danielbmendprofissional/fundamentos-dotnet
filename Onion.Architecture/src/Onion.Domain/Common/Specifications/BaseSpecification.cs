using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Onion.Domain.Common.Specifications
{
    public abstract class BaseSpecification<T> : ISpecification<T>
    {
        public Expression<Func<T, bool>>? Criteria { get; protected set; }

        private readonly List<Expression<Func<T, object>>> _includes = new();
        public IReadOnlyCollection<Expression<Func<T, object>>> Includes => _includes.AsReadOnly();

        public Expression<Func<T, object>>? OrderBy { get; protected set; }
        public Expression<Func<T, object>>? OrderByDescending { get; protected set; }
        public int? Skip { get; protected set; }
        public int? Take { get; protected set; }
        public bool IsPagingEnabled { get; protected set; }

        protected void AddInclude(Expression<Func<T, object>> includeExpression) => _includes.Add(includeExpression);
        protected void ApplyOrderBy(Expression<Func<T, object>> orderByExpression) => OrderBy = orderByExpression;
        protected void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescExpression) => OrderByDescending = orderByDescExpression;
        protected void ApplyPaging(int skip, int take)
        {
            Skip = skip;
            Take = take;
            IsPagingEnabled = true;
        }
    }
}