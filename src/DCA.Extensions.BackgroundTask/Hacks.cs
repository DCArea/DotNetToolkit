#if NET5_0_OR_GREATER
// context: https://github.com/StackExchange/StackExchange.Redis/issues/2619
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.Runtime.CompilerServices.IsExternalInit))]
#else
// To support { get; init; } properties
using System.ComponentModel;

namespace System.Runtime.CompilerServices
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static class IsExternalInit { }
}
#endif

#if !NET7_0_OR_GREATER
namespace System.Runtime.CompilerServices
{
#pragma warning disable CA1018 // Mark attributes with AttributeUsageAttribute
    public class RequiredMemberAttribute : Attribute { }
    public class CompilerFeatureRequiredAttribute : Attribute
	{
#pragma warning disable IDE0290 // Use primary constructor
        public CompilerFeatureRequiredAttribute(string name) { }
    }
}

namespace System.Diagnostics.CodeAnalysis
{
	[AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
	public sealed class SetsRequiredMembersAttribute : Attribute
	{
	}
}
#endif
