using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Application.Common.Validations
{
    public abstract class BaseHandlerValidator<TDTO> : AbstractValidator<TDTO> where TDTO : class
    {
        protected BaseHandlerValidator()
        {
            ClassLevelCascadeMode = CascadeMode.Continue;
            RuleLevelCascadeMode = CascadeMode.Stop;
        }

        protected void RequiredRule<TProperty>(Expression<Func<TDTO, TProperty>> expression)
        {
            RuleFor(expression)
                .NotEmpty()
                .NotNull()
                .WithMessage("{PropertyName} is required.");
        }

        protected void MaxLengthRule(Expression<Func<TDTO, string>> expression, int max)
        {
            RuleFor(expression)
                .MaximumLength(max)
                .When(x => x != null);
        }
    }
}
