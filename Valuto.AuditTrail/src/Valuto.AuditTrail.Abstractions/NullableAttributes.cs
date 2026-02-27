using System;
using System.Collections.Generic;
using System.Text;

namespace System.Diagnostics.CodeAnalysis
{
    namespace System.Diagnostics.CodeAnalysis
    {
        [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue, Inherited = false)]
        internal sealed class AllowNullAttribute : Attribute { }

        [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue, Inherited = false)]
        internal sealed class MaybeNullAttribute : Attribute { }

        [AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
        internal sealed class MaybeNullWhenAttribute : Attribute
        {
            public MaybeNullWhenAttribute(bool returnValue) => ReturnValue = returnValue;
            public bool ReturnValue { get; }
        }
    }
}
