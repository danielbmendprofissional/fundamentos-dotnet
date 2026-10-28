namespace System.Diagnostics.CodeAnalysis
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue, Inherited = false)]
    internal sealed class AllowNullAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue, Inherited = false)]
    internal sealed class MaybeNullAttribute : Attribute { }

#if !NETCOREAPP3_0_OR_GREATER && !NETSTANDARD2_1_OR_GREATER
    namespace System.Diagnostics.CodeAnalysis
    {
        [AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
        internal sealed class MaybeNullWhenAttribute : Attribute
        {
            public MaybeNullWhenAttribute(bool returnValue) => ReturnValue = returnValue;
            public bool ReturnValue { get; }
        }
    }
#endif
}
