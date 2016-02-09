using System.Reflection;

namespace Moq.Proxy
{
	public interface IProxyFactoryDiscoverer
	{
		IProxyFactory DiscoverFactory (Assembly assembly);
	}
}