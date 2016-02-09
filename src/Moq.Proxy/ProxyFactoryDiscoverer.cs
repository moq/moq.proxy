using System;
using System.Reflection;

namespace Moq.Proxy
{
	public static class ProxyFactoryDiscoverer
	{
		public static IProxyFactoryDiscoverer Instance { get; set; }

		static ProxyFactoryDiscoverer()
		{
			Instance = new DefaultFactoryDiscoverer ();
		}

		public static IProxyFactory DiscoverFactory (Assembly assembly)
		{
			return Instance.DiscoverFactory (assembly);
		}

		class DefaultFactoryDiscoverer : IProxyFactoryDiscoverer
		{
			public IProxyFactory DiscoverFactory (Assembly assembly)
			{
				var factoryType = assembly.GetCustomAttribute<ProxyFactoryAttribute>()?.FactoryType;
				if (factoryType == null)
					throw new NotSupportedException ();

				return (IProxyFactory)Activator.CreateInstance (factoryType);
			}
		}
	}
}
