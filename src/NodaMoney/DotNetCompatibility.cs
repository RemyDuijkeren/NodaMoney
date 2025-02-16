// Declaring the assembly as CLS-compliant
[assembly: CLSCompliant(true)]

#if !NET5_0_OR_GREATER
// Init setters in C# 9 only works from .NET 5 and higher, see https://www.mking.net/blog/error-cs0518-isexternalinit-not-defined
namespace System.Runtime.CompilerServices
{
    [ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)]
    internal static class IsExternalInit;
}

#endif

#if !NETCOREAPP3_0_OR_GREATER && !NETSTANDARD2_1_OR_GREATER
// NotNullWhen attribute is introduced in .NET Core 3.0
namespace System.Diagnostics.CodeAnalysis
{
    [AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
    public sealed class NotNullWhenAttribute : Attribute
    {
        public NotNullWhenAttribute(bool returnValue)
        {
            ReturnValue = returnValue;
        }

        public bool ReturnValue { get; }
    }
}
#endif
