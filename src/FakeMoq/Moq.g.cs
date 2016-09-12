using System.Reflection;
using Moq.Proxy;

/// <summary>
/// Entry point for creating mocks.
/// </summary>
public static class Mock
{
	/// <summary>
	/// Creates a mock for type <see cref="T"/>.
	/// </summary>
	[ProxyGenerator]
	public static T Of<T>()
	{
		var factory = ProxyFactoryDiscoverer.DiscoverFactory(typeof(Mock).GetTypeInfo().Assembly);

		return FakeMoq.Mock.Of<T>(factory);
	}
}