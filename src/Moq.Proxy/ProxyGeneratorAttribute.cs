using System;

namespace Moq.Proxy
{
	/// <summary>
	/// Flags a method as the one that determines proxies to 
	/// be generated if the static proxy package is used.
	/// </summary>
	/// <remarks>
	/// Should be used by the mocking library generated API 
	/// facade to signal the static proxy generator to inspect 
	/// usages of the annotated method to discover static proxies 
	/// to generate.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public class ProxyGeneratorAttribute : Attribute
	{
	}
}